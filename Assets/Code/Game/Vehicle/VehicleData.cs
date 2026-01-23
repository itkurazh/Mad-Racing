using System;
using Unity.AppUI.UI;
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
            _position = value;
            OnChangedProperty?.Invoke(Property.Position, _position);
        }
    }
    
    private Vector3 _rotation;
    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            OnChangedProperty?.Invoke(Property.Rotation, _rotation);
        }
    }
    
    public Vector3 Direction;

    public float DirectionDot;
    public float AccelationTime;
    public float TargetVelocity;
    public float CurrentVelocity;
    public float AngularVelocity;
    public float LerpVelocity;
    
    public float InputDirection;
    public float InputSide;

    public float Traction;
    
    public bool IsBrake;
    
    public enum Property : int
    {
        Position = 1,
        Rotation = 2,
    }
}