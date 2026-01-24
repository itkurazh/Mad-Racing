using System;
using Unity.Netcode;
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
        _data.Controller.Init(this);
        _data.Position = _controller.transform.position;
        _data.Rotation = _controller.transform.forward;
        
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
        Debug();
        
        if(!Network.IsSpawned)
            return;
        
        if (!Network.IsOwner)
        {
            _data.Controller.transform.position = _data.Position;
            _data.Controller.transform.rotation = Quaternion.LookRotation(_data.Rotation);
        }
        else
        {
            _data.LerpVelocity = Mathf.Abs(_data.CurrentVelocity) / (_config.MaximumSpeed * _config.AccelerationMultiplier);
            _data.Position = _controller.transform.position;
            _data.Rotation = _controller.transform.forward;
            
            UpdateDataRpc(_data);
        }
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
            _data.AccelationTime = Mathf.Lerp(_data.AccelationTime, 0f, _config.Traction * Time.deltaTime);
        }

        _data.DirectionDot = Vector3.Dot(_data.Direction, _controller.transform.forward);
        
        _data.AccelationTime = Mathf.Clamp(_data.AccelationTime, 0f, 5f);
        _data.TargetVelocity = _data.InputDirection * GetSpeed();
        _data.TargetVelocity = Mathf.Clamp(_data.TargetVelocity * _data.DirectionDot, -_config.MaximumSpeed, _config.MaximumSpeed);
        _data.CurrentVelocity = Mathf.Lerp(_data.CurrentVelocity, _data.TargetVelocity, _data.DirectionDot);
        
        _data.Traction = _data.IsBrake ? 1f : _config.Traction;
        
        _data.Direction = Vector3.Lerp(_data.Direction, _controller.transform.forward, _data.Traction * Time.deltaTime);
        
        _controller.Move(_data.Direction * _data.CurrentVelocity);
    }

    public void Rotate(float axis)
    {
        _data.InputSide = axis;
        
        var force = _config.AngularAcceleration.Evaluate(_data.LerpVelocity) * _config.AngularSpeed * Mathf.Clamp(_data.CurrentVelocity, -1, 1);
        force *= _data.IsBrake ? 5f * (1f - _data.DirectionDot) : 1;
        
        _data.AngularVelocity = Mathf.Lerp(_data.AngularVelocity, force, _data.Traction * Time.deltaTime);
        
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
        return _config.MaximumSpeed * (_config.AccelerationMultiplier * GetAcceleration(_data.AccelationTime));
    }
    
    private float GetAcceleration(float evaluate)
    {
        return _config.Acceleration.Evaluate(evaluate);
    }
    
    [Rpc(SendTo.NotOwner)]
    private void UpdateDataRpc(VehicleData data)
    {
        Data.Position = data.Position;
        Data.Rotation = data.Rotation;
        Data.CurrentVelocity = data.CurrentVelocity;
        Data.LerpVelocity = data.LerpVelocity;
    }
}