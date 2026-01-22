using System;
using UnityEngine;

public sealed class Player : Entity
{
    [SerializeField] private Unit _unit;
    
    public CameraController CameraController;
    
    private PlayerConfig _config => Configs.Get<PlayerConfig>();

    private void Start()
    {
        CameraController.SetTarget(_unit.Data.Controller.transform);
    }

    protected override void Subscribe()
    {
        
    }

    protected override void Unsubscribe()
    {
        
    }

    private void Update()
    {
        Locomotion();
    }

    private void Locomotion()
    {
        var forward = Vector3.Cross(CameraController.LookAt.right, Vector3.up);
        var right = Vector3.Cross(forward, Vector3.up);
        var inputDirection = forward * Input.GetAxisRaw("Vertical") + 
                                    -right * Input.GetAxisRaw("Horizontal");
        
        if(inputDirection == Vector3.zero)
        {
            _unit.Data.Velocity = 0;
            _unit.Data.VelocityState = UnitData.VelocityStateID.Idle;
            return;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            _unit.Data.Velocity = _config.RunSpeed;
            _unit.Data.VelocityState = UnitData.VelocityStateID.Running;
        }
        else
        {
            _unit.Data.Velocity = _config.WalkSpeed;
            _unit.Data.VelocityState = UnitData.VelocityStateID.Walking;
        }
        
        var moveDirection = inputDirection.normalized * _unit.Data.Velocity * Time.deltaTime;
        var position = _unit.Data.Position + moveDirection;
        
        _unit.Data.Controller.Move(position);
        _unit.Data.Direction = Vector3.Lerp(_unit.Data.Direction, moveDirection, UnitConstans.LERP_VALUE * Time.deltaTime);
    }
}
