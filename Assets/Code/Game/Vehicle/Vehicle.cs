using System;
using Unity.Netcode;
using UnityEngine;
using Object = System.Object;

public partial class Vehicle : Entity
{
    public VehicleData Data => _data;
    
    [SerializeField] private VehicleView _view;
    [SerializeField] private VehicleController _controller;
    private VehicleData _data;
    
    private LocalVehicle _localVehicle;
    private NetworkVehicle _networkVehicle;
    
    public void Move(float direction) => _localVehicle.Move(direction);
    public void Rotate(float direction) => _localVehicle.Rotate(direction);
    public void Brake(bool brake) => _localVehicle.Brake(brake);

    private bool _isSubscribeProperty;
    
    private Vector3 _lastPosition;
    
    protected override void Awake()
    {
        _data = new VehicleData();
        _data.Controller = _controller;
        _data.Controller.Init(this);
        _data.Position = _controller.transform.position;
        _data.Rotation = _controller.transform.forward;
        
        _localVehicle = new LocalVehicle(this);
        _networkVehicle = new NetworkVehicle(this);
        
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
        
        if (Network.IsOwner)
        {
            _localVehicle.Update();
            
            if(!_isSubscribeProperty)
            {
                _data.OnChangedProperty += OnChangedProperty;
                _isSubscribeProperty = true;
            }
        }
        else
        {
            _networkVehicle.Update();
            
            if(_isSubscribeProperty)
            {
                _data.OnChangedProperty -= OnChangedProperty;
                _isSubscribeProperty = false;
            }
        }
    }

    private void OnChangedProperty(VehicleData.Property property, Object value)
    {
        switch (property)
        {
            case VehicleData.Property.Position:
                
                var position = (Vector3)value;

                if(Vector3.Distance(position, _lastPosition) > NetworkService.THERHOLD_SLEEP_VALUE)
                {
                    OnChangePositionRpc(position);
                    _lastPosition = position;
                }
                
                break;
            case VehicleData.Property.Rotation: OnChangeRotationRpc((Vector3)value); break;
            case VehicleData.Property.CurrentVelocity: OnChangeCurrentVelocityRpc((float)value); break;
            case VehicleData.Property.InputDirection: OnChangeInputDirectionRpc((float)value); break;
            case VehicleData.Property.InputSide: OnChangeInputSideRpc((float)value); break;
        }
    }

    [Rpc(SendTo.NotOwner)]
    private void OnChangePositionRpc(Vector3 position) => Data.Position = position;

    [Rpc(SendTo.NotOwner)]
    private void OnChangeRotationRpc(Vector3 rotation) =>  Data.Rotation = rotation;

    [Rpc(SendTo.NotOwner)]
    private void OnChangeCurrentVelocityRpc(float velocity) => Data.CurrentVelocity = velocity;
    
    [Rpc(SendTo.NotOwner)]
    private void OnChangeInputDirectionRpc(float inputDirection) => Data.InputDirection = inputDirection;
    
    [Rpc(SendTo.NotOwner)]
    private void OnChangeInputSideRpc(float inputSide) => Data.InputSide = inputSide;
}