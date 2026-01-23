using System;
using UnityEngine;

public sealed class Player : Entity
{
    [SerializeField] private Unit _unit;
    [SerializeField] private Vehicle _vehicle;
    
    public CameraController CameraController;
    
    private UnitConfig UnitConfig => Configs.Get<UnitConfig>();

    private void Start()
    {
        //CameraController.SetTarget(_unit.Data.Controller.transform);
        //CameraController.ChangeState(CameraController.StateID.Character);
        
        CameraController.SetTarget(_vehicle.Data.Controller.transform);
        CameraController.ChangeState(CameraController.StateID.Vehicle);
    }

    protected override void Subscribe()
    {
        
    }

    protected override void Unsubscribe()
    {
        
    }

    private void Update()
    {
        VehicleLocomotion();
        //CharacterLocomotion();
    }

    private void VehicleLocomotion()
    {
        _vehicle.Move(Input.GetAxisRaw("Vertical"));
        _vehicle.Rotate(Input.GetAxis("Horizontal"));
        _vehicle.Brake(Input.GetKey(KeyCode.Space));
    }

    private void CharacterLocomotion()
    {
        var right = Vector3.Cross(CameraController.Direction, Vector3.up);
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
        
        var moveDirection = inputDirection.normalized * _unit.Data.Velocity * Time.deltaTime;
        var position = _unit.Data.Position + moveDirection;
        
        _unit.Data.Controller.Move(position);
        _unit.Data.Direction = Vector3.Lerp(_unit.Data.Direction, moveDirection, UnitConstants.LERP_VALUE * Time.deltaTime);
    }
}
