using System;
using Unity.Netcode;
using UnityEngine;
using Object = System.Object;

public class UnitData : INetworkSerializable
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

    public ModeID Mode;
    
    public enum Property : int
    {
        Position = 1,
        Direction = 2,
        Velocity = 3,
        VelocityState = 4
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

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _position);
        serializer.SerializeValue(ref _direction);
        serializer.SerializeValue(ref _velocityStateID);
        serializer.SerializeValue(ref Mode);
    }
}