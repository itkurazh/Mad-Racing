using UnityEngine;

public class LocalPlayer
{
    private Player _player;
    
    private SphereCollider _triggerCollider;
    
    public LocalPlayer(Player player)
    {
        _player = player;
        _triggerCollider = player.GetComponent<SphereCollider>();
    }
    
    public void Start()
    {
        Services.Game.Context.CreateCamera();
            
        SwitchState(UnitData.ModeID.Character);
            
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        switch (_player.Unit.Data.Mode)
        {
            case UnitData.ModeID.Character: CharacterLocomotion(); break;
            case UnitData.ModeID.Vehicle: VehicleLocomotion(); break;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_player.Vehicle)
            {
                if(_player.Vehicles.Count > 0)
                {
                    var targetVehicle = _player.Vehicles[0];
                    
                    if(!targetVehicle.IsFree)
                        return;
                        
                    _player.EnterToVehicleRpc(_player.Network.OwnerClientId, targetVehicle.Network.NetworkObjectId);
                            
                    _player.Vehicle = targetVehicle;
                    SwitchState(UnitData.ModeID.Vehicle);
                }
            }
            else
            {
                _player.Unit.Data.Controller.transform.position = _player.Vehicle.Data.Position + -_player.Vehicle.Data.Controller.transform.right;
                _player.Unit.Data.Direction = _player.Vehicle.Data.Controller.transform.forward;
                    
                _player.Unit.View.transform.position = _player.Unit.Data.Controller.transform.position;
    
                _player.ExitToVehicleRpc(_player.Vehicle.Network.NetworkObjectId);
                    
                _player.Vehicle = null;
                    
                SwitchState(UnitData.ModeID.Character);
            }
        }
            
        _triggerCollider.center = _player.Unit.Data.Position;
    }
    
    public void SwitchState(UnitData.ModeID modeID)
    {
        switch (modeID)
        {
            case UnitData.ModeID.Character:
                Services.Game.Context.Camera.SetTarget(_player.Unit.Data.Controller.transform);
                Services.Game.Context.Camera.ChangeState(CameraController.StateID.Character);
                
                _player.Unit.gameObject.SetActive(true);
                _player.Unit.Data.Controller.gameObject.SetActive(true);
                break;
            
            case UnitData.ModeID.Vehicle:
                Services.Game.Context.Camera.SetTarget(_player.Vehicle.Data.Controller.transform);
                Services.Game.Context.Camera.ChangeState(CameraController.StateID.Vehicle);
                
                _player.Unit.gameObject.SetActive(false);
                _player.Unit.Data.Controller.gameObject.SetActive(false);
                break;
        }
        
        _player.Unit.Data.Mode = modeID;
    }

    private void VehicleLocomotion()
    {
        _player.Vehicle.Move(Input.GetAxisRaw("Vertical"));
        _player.Vehicle.Rotate(Input.GetAxis("Horizontal"));
        _player.Vehicle.Brake(Input.GetKey(KeyCode.Space));
    }

    private void CharacterLocomotion()
    {
        var right = Vector3.Cross(Services.Game.Context.Camera.Direction, Vector3.up);
        var forward = Vector3.Cross(right, Vector3.up);
        var inputDirection = -forward * Input.GetAxisRaw("Vertical") + 
                                    -right * Input.GetAxisRaw("Horizontal");
        
        if(inputDirection == Vector3.zero)
        {
            _player.Unit.Data.Velocity = 0;
            _player.Unit.Data.VelocityState = UnitData.VelocityStateID.Idle;
            return;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            _player.Unit.Data.Velocity = _player.UnitConfig.RunSpeed;
            _player.Unit.Data.VelocityState = UnitData.VelocityStateID.Running;
        }
        else
        {
            _player.Unit.Data.Velocity = _player.UnitConfig.WalkSpeed;
            _player.Unit.Data.VelocityState = UnitData.VelocityStateID.Walking;
        }
        
        var moveDirection = inputDirection.normalized * (_player.Unit.Data.Velocity * Time.deltaTime);
        
        _player.Unit.Data.Controller.Move(moveDirection);
        _player.Unit.Data.Direction = Vector3.Lerp(_player.Unit.Data.Direction, moveDirection, UnitConstants.LERP_VALUE * Time.deltaTime);
    }
}