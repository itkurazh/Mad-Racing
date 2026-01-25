using System;
using Unity.Netcode;
using UnityEngine;
using Object = System.Object;

public class VehicleData
{
    public VehicleController Controller;
    
    public Action<Property, Object> OnChangedProperty;
    
    private Vector3 _position;
    public Vector3 Position
    {
        get => _position;
        set
        {
            if(Vector3.Distance(_position, value) > 0.01f)
            {
                _position = value;
                OnChangedProperty?.Invoke(Property.Position, value);
            }
        }
    }
    
    private Vector3 _rotation;
    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            if(_rotation != value)
            {
                _rotation = value;
                OnChangedProperty?.Invoke(Property.Rotation, value);
            }
        }
    }
    
    private float _currentVelocity;
    public float CurrentVelocity
    {
        get => _currentVelocity;
        set
        {
            var result = Mathf.Abs(_currentVelocity - value);
            
            if(result > 0.01f)
            {
                _currentVelocity = value;
                OnChangedProperty?.Invoke(Property.CurrentVelocity, value);
            }
        }
    }
    
    private float _inputDirection;
    public float InputDirection
    {
        get => _inputDirection;
        set
        {
            var result = Mathf.Abs(_inputDirection - value);
            
            if(result > 0.01f)
            {
                _inputDirection = value;
                OnChangedProperty?.Invoke(Property.InputDirection, value);
            }
        }
    }
    
    private float _inputSide;
    public float InputSide
    {
        get => _inputSide;
        set
        {
            var result = Mathf.Abs(_inputSide - value);
            
            if(result > 0.01f)
            {
                _inputSide = value;
                OnChangedProperty?.Invoke(Property.InputSide, value);
            }
        }
    }
    
    public Vector3 Direction;

    public float DirectionDot;
    public float AccelationTime;
    public float TargetVelocity;
    public float AngularVelocity;
    public float VelocityClamp;

    public float Traction;
    
    public bool IsBrake;

    public enum Property : int
    {
        Position = 1,
        Rotation = 2,
        CurrentVelocity = 3,
        InputDirection = 4,
        InputSide = 5,
    }
}