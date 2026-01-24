using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public sealed class Player : Entity
{
    private NetworkObject _network;
    public NetworkObject Network => GetNetwork();

    [SerializeField] private Unit _unit;
    [SerializeField] private Vehicle _vehicle;

    private PlayerModeID _modeID;
    
    private UnitConfig UnitConfig => Configs.Get<UnitConfig>();
    
    private async void Start()
    {
        while (!Network.IsSpawned)
            await Task.Yield();
        
        if (Network.IsOwner)
        {
            Services.Game.Context.CreateCamera();
            
            SwitchState(PlayerModeID.Character);
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    protected override void Subscribe()
    {
        
    }

    protected override void Unsubscribe()
    {
        
    }

    private void Update()
    {
        if (!Network.IsOwner)
        {
            _unit.Data.Controller.transform.position = _unit.Data.Position;
            _unit.View.transform.rotation = Quaternion.LookRotation(_unit.Data.Direction);
        }
        else
        {
            switch (_modeID)
            {
                case PlayerModeID.Character: CharacterLocomotion(); break;
                case PlayerModeID.Vehicle: VehicleLocomotion(); break;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_modeID == PlayerModeID.Character)
                    Services.Game.Context.TryEnterVehicle(this, _vehicle);
                else
                    Services.Game.Context.TryExitVehicle(this, _vehicle);
            }
            
            UpdateDataRpc(_unit.Data);
        }
    }

    public void SwitchState(PlayerModeID modeID)
    {
        switch (modeID)
        {
            case PlayerModeID.Character:
                Services.Game.Context.Camera.SetTarget(_unit.Data.Controller.transform);
                Services.Game.Context.Camera.ChangeState(CameraController.StateID.Character);
                
                _unit.gameObject.SetActive(true);
                //_unit.Data.Controller.transform.position = _vehicle.Data.Position + -_vehicle.Data.Controller.transform.right;
                //_unit.View.transform.position = _unit.Data.Controller.transform.position;
                break;
            
            case PlayerModeID.Vehicle:
                Services.Game.Context.Camera.SetTarget(_vehicle.Data.Controller.transform);
                Services.Game.Context.Camera.ChangeState(CameraController.StateID.Vehicle);
                
                _unit.gameObject.SetActive(false);
                break;
        }
        
        _modeID = modeID;
    }

    private void VehicleLocomotion()
    {
        _vehicle.Move(Input.GetAxisRaw("Vertical"));
        _vehicle.Rotate(Input.GetAxis("Horizontal"));
        _vehicle.Brake(Input.GetKey(KeyCode.Space));
    }

    private void CharacterLocomotion()
    {
        var right = Vector3.Cross(Services.Game.Context.Camera.Direction, Vector3.up);
        var forward = Vector3.Cross(right, Vector3.up);
        var inputDirection = -forward * Input.GetAxisRaw("Vertical") + 
                                    -right * Input.GetAxisRaw("Horizontal");
        
        if(inputDirection == Vector3.zero)
        {
            _unit.Data.Velocity = 0;
            _unit.Data.VelocityState = UnitData.VelocityStateID.Idle;
            return;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            _unit.Data.Velocity = UnitConfig.RunSpeed;
            _unit.Data.VelocityState = UnitData.VelocityStateID.Running;
        }
        else
        {
            _unit.Data.Velocity = UnitConfig.WalkSpeed;
            _unit.Data.VelocityState = UnitData.VelocityStateID.Walking;
        }
        
        var moveDirection = inputDirection.normalized * (_unit.Data.Velocity * Time.deltaTime);
        
        _unit.Data.Controller.Move(moveDirection);
        _unit.Data.Direction = Vector3.Lerp(_unit.Data.Direction, moveDirection, UnitConstants.LERP_VALUE * Time.deltaTime);
    }

    private NetworkObject GetNetwork()
    {
        if(!_network)
            _network = GetComponent<NetworkObject>();
        
        return _network;
    }
    
    [Rpc(SendTo.NotOwner)]
    private void UpdateDataRpc(UnitData data)
    {
        _unit.Data.Position = data.Position;
        _unit.Data.Direction = data.Direction;
        _unit.Data.VelocityState = data.VelocityState;
    }
}
