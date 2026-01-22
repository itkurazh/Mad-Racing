using System;
using UnityEngine;

public class Vehicle : Entity
{
    public VehicleData Data => _data;
    
    [SerializeField] private VehicleView _view;
    [SerializeField] private VehicleController _controller;
    private VehicleData _data;
    
    private VehicleConfig _config => Configs.Get<VehicleConfig>();
    
    protected override void Awake()
    {
        _data = new VehicleData();
        _data.Controller = _controller;
        
        _view.SetData(_data);
    }
    
    protected override void Subscribe()
    {
        
    }

    protected override void Unsubscribe()
    {
        
    }

    private void Update()
    {
        _data.CurrentVelocity = Mathf.Lerp(_data.CurrentVelocity, _data.TargetVelocity, 5f * Time.deltaTime);
        
        _controller.Move(_controller.transform.forward * _data.CurrentVelocity);
        
        _data.Position = _controller.transform.position;
        _data.Rotation = _controller.transform.forward;
    }

    public void Move(float direction)
    {
        _data.AccelationTime = Mathf.Abs(direction);
        _data.TargetVelocity = direction * GetSpeed();
    }

    public void Rotate(float axis)
    {
        var lerpVelocity = Mathf.Abs(_data.CurrentVelocity) / (_config.LinearSpeed * _config.AccelerationMultiplier);
        var angularSpeed = _config.AngularAcceleration.Evaluate(lerpVelocity) * _config.AngularSpeed * Mathf.Clamp(_data.CurrentVelocity, -1, 1);
        
        _controller.Rotate(_controller.transform.up * (angularSpeed * axis));
    }

    public void Brake()
    {
        _data.TargetVelocity = 0;
    }

    private float GetSpeed()
    {
        return _config.LinearSpeed * (_config.AccelerationMultiplier * GetAcceleration(_data.AccelationTime));
    }
    
    private float GetAcceleration(float evaluate)
    {
        return _config.Acceleration.Evaluate(evaluate);
    }
}