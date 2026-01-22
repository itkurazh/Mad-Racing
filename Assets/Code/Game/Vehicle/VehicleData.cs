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
    
    private Vector3 _direction;
    public Vector3 Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            OnChangedProperty?.Invoke(Property.Direction, _direction);
        }
    }

    public float AccelationTime;
    public float TargetVelocity;
    public float CurrentVelocity;
    
    public enum Property : int
    {
        Position = 1,
        Rotation = 2,
        Direction = 3,
    }
}