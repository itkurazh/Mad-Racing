using System;
using UnityEngine;
using Object = System.Object;

public class UnitData
{
    public UnitController Controller;
    
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

    private float _velocity;
    public float Velocity
    {
        get => _velocity;
        set
        {
            _velocity = value;
            OnChangedProperty?.Invoke(Property.Velocity, _velocity);
        }
    }
    
    private VelocityStateID _velocityStateID;
    public VelocityStateID VelocityState
    {
        get => _velocityStateID;
        set
        {
            if(_velocityStateID == value)
                return;
            
            _velocityStateID = value;
            OnChangedProperty?.Invoke(Property.VelocityState, _velocityStateID);
        }
    }

    private ModeID _modeID;

    public ModeID Mode
    {
        get => _modeID;
        set
        {
            if(_modeID == value)
                return;
            
            _modeID = value;
            OnChangedProperty?.Invoke(Property.Mode, _modeID);
        }
    }
    
    public enum Property : int
    {
        Position = 1,
        Direction = 2,
        Velocity = 3,
        VelocityState = 4,
        Mode = 5
    }
    
    public enum VelocityStateID
    {
        Idle = 0,
        Walking = 1,
        Running = 2,
    }
    
    public enum ModeID
    {
        Character = 0,
        Vehicle = 1,
    }
}