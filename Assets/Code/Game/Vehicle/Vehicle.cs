using System;
using UnityEngine;

public partial class Vehicle : Entity
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
        _data.CurrentVelocity = Mathf.Lerp(_data.CurrentVelocity, _data.TargetVelocity, VehicleConstants.LERP_VALUE * Time.deltaTime);
        
        _data.Position = _controller.transform.position;
        _data.Rotation = _controller.transform.forward;
    }

    public void Move(float direction)
    {
        if (direction != 0)
        {
            _data.AccelationTime += Time.deltaTime;

            if (!direction.Equals(_data.InputDirection))
                _data.AccelationTime = 0;
            
            _data.InputDirection = direction;
        }
        else
        {
            _data.AccelationTime -= Time.deltaTime;
        }

        _data.AccelationTime = Mathf.Clamp01(_data.AccelationTime);
        _data.TargetVelocity = _data.InputDirection * GetSpeed();
        
        _data.Direction = Vector3.Lerp(_data.Direction, _controller.transform.forward, VehicleConstants.LERP_VALUE * Time.deltaTime);
        
        _controller.Move(_data.Direction * _data.CurrentVelocity);
    }

    public void Rotate(float axis)
    {
        _data.InputSide = axis;
        var lerpVelocity = Mathf.Abs(_data.CurrentVelocity) / (_config.LinearSpeed * _config.AccelerationMultiplier);
        _data.AngularVelocity = _config.AngularAcceleration.Evaluate(lerpVelocity) * _config.AngularSpeed * Mathf.Clamp(_data.CurrentVelocity, -1, 1);
        
        _controller.Rotate(_controller.transform.up * (_data.AngularVelocity * _data.InputSide));
    }

    public void Brake(bool activeSelf)
    {
        _data.IsBrake = activeSelf;
        
        if(activeSelf)
        {
            _data.AccelationTime = Mathf.Lerp(_data.AccelationTime, 0f, 3f * Time.deltaTime);
            _controller.AdditiveSide();
        }
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