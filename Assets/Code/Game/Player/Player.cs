using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public sealed class Player : Entity
{
    public Unit Unit => _unit;

    [SerializeField] private Unit _unit;
    [SerializeField] private Vehicle _vehicle;
    
    private SphereCollider _triggerCollider;
    private List<Vehicle> _vehicles = new();
    
    private UnitConfig UnitConfig => Configs.Get<UnitConfig>();

    protected override void Awake()
    {
        _triggerCollider =  GetComponent<SphereCollider>();
    }
    
    private async void Start()
    {
        while (!Network.IsSpawned)
            await Task.Yield();
        
        if (Network.IsOwner)
        {
            Services.Game.Context.CreateCamera();
            
            SwitchState(UnitData.ModeID.Character);
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        _triggerCollider.enabled = Network.IsOwner;
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
            if(Vector3.Distance(_unit.Data.Controller.transform.position , _unit.Data.Position) > 1f)
                _unit.View.transform.position = _unit.Data.Position;
            
            _unit.Data.Controller.transform.position = _unit.Data.Position;
            
            if(_unit.Data.Direction != Vector3.zero)
                _unit.View.transform.rotation = Quaternion.LookRotation(_unit.Data.Direction);
            
            _unit.View.gameObject.SetActive(_unit.Data.Mode == UnitData.ModeID.Character);
        }
        else
        {
            switch (_unit.Data.Mode)
            {
                case UnitData.ModeID.Character: CharacterLocomotion(); break;
                case UnitData.ModeID.Vehicle: VehicleLocomotion(); break;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!_vehicle)
                {
                    if(_vehicles.Count > 0)
                    {
                        var targetVehicle = _vehicles[0];
                        
                        EnterToVehicleRpc(Network.OwnerClientId, targetVehicle.Network.NetworkObjectId);
                            
                        _vehicle = targetVehicle;
                        SwitchState(UnitData.ModeID.Vehicle);
                    }
                }
                else
                {
                    _unit.Data.Controller.transform.position = _vehicle.Data.Position + -_vehicle.Data.Controller.transform.right;
                    _unit.Data.Direction = _vehicle.Data.Controller.transform.forward;
                    
                    _unit.View.transform.position = _unit.Data.Controller.transform.position;
    
                    ExitToVehicleRpc(_vehicle.Network.NetworkObjectId);
                    
                    _vehicle = null;
                    
                    SwitchState(UnitData.ModeID.Character);
                }
            }
            
            _triggerCollider.center = _unit.Data.Position;
            
            UpdateDataRpc(_unit.Data);
        }
    }

    [Rpc(SendTo.Server)]
    private void EnterToVehicleRpc(ulong clientId, ulong vehicleId)
    {
        Services.Game.Context.EnterToVehicle(clientId, vehicleId);
    }
    
    [Rpc(SendTo.Server)]
    private void ExitToVehicleRpc(ulong vehicleId)
    {
        Services.Game.Context.ExitToVehicle(vehicleId);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out VehicleController controller) && !_vehicles.Contains(controller.Vehicle))
            _vehicles.Add(controller.Vehicle);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out VehicleController controller) && _vehicles.Contains(controller.Vehicle))
            _vehicles.Remove(controller.Vehicle);
    }

    public void SwitchState(UnitData.ModeID modeID)
    {
        switch (modeID)
        {
            case UnitData.ModeID.Character:
                Services.Game.Context.Camera.SetTarget(_unit.Data.Controller.transform);
                Services.Game.Context.Camera.ChangeState(CameraController.StateID.Character);
                
                _unit.gameObject.SetActive(true);
                break;
            
            case UnitData.ModeID.Vehicle:
                Services.Game.Context.Camera.SetTarget(_vehicle.Data.Controller.transform);
                Services.Game.Context.Camera.ChangeState(CameraController.StateID.Vehicle);
                
                _unit.gameObject.SetActive(false);
                break;
        }
        
        _unit.Data.Mode = modeID;
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
    
    [Rpc(SendTo.NotOwner)]
    private void UpdateDataRpc(UnitData data)
    {
        _unit.Data.Position = data.Position;
        _unit.Data.Direction = data.Direction;
        _unit.Data.VelocityState = data.VelocityState;
        _unit.Data.Mode = data.Mode;
    }
}
