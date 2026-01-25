using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Object = System.Object;

public class Player : Entity
{
    public Unit Unit => _unit;
    public Vehicle Vehicle { get; set; }
    public List<Vehicle> Vehicles { get; set; }
    public UnitConfig UnitConfig => Configs.Get<UnitConfig>();
    
    [SerializeField] private Unit _unit;
    
    private LocalPlayer _localPlayer;
    private NetworkPlayer _networkPlayer;
    
    private Vector3 _lastPosition;
    private Vector3 _lastDirection;
    
    private async void Start()
    {
        while (!Network.IsSpawned)
            await Task.Yield();

        if (Network.IsOwner)
        {
            _localPlayer = new LocalPlayer(this);
            _localPlayer.Start();
            Vehicles = new();
            
            Unit.Data.OnChangedProperty += OnChangedProperty;
        }
        else
        {
            _networkPlayer = new NetworkPlayer(this);
            _networkPlayer.Start();
        }
    }

    protected override void Subscribe()
    {
        
    }

    protected override void Unsubscribe()
    {
        
    }

    private void Update()
    {
        _localPlayer?.Update();
        _networkPlayer?.Update();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out VehicleController controller) && !Vehicles.Contains(controller.Vehicle))
            Vehicles.Add(controller.Vehicle);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out VehicleController controller) && Vehicles.Contains(controller.Vehicle))
            Vehicles.Remove(controller.Vehicle);
    }
    
    private void OnChangedProperty(UnitData.Property property, Object value)
    {
        switch (property)
        {
            case UnitData.Property.Position:
                
                var position = (Vector3)value;

                if(Vector3.Distance(position, _lastPosition) > NetworkService.THERHOLD_SLEEP_VALUE)
                {
                    OnChangePositionRpc(position);
                    _lastPosition = position;
                }
                
                break;
            case UnitData.Property.Direction: OnChangeDirectionRpc((Vector3)value); break;
            case UnitData.Property.VelocityState: OnChangeVelocityStateRpc((UnitData.VelocityStateID)value); break;
        }
    }

    [Rpc(SendTo.NotOwner)]
    private void OnChangePositionRpc(Vector3 position)
    {
        Unit.Data.Position = position;
    }

    [Rpc(SendTo.NotOwner)]
    private void OnChangeDirectionRpc(Vector3 direction)
    {
        Unit.Data.Direction = direction;
    }

    [Rpc(SendTo.NotOwner)]
    private void OnChangeVelocityStateRpc(UnitData.VelocityStateID state)
    {
        Unit.Data.VelocityState = state;
    }

    [Rpc(SendTo.Server)]
    private void EnterToVehicleRpc(ulong clientId, ulong vehicleId)
    {
        Services.Game.Context.EnterToVehicle(clientId, vehicleId);
    }
    
    [Rpc(SendTo.Server)]
    private void ExitToVehicleRpc(ulong vehicleId)
    {
        Services.Game.Context.ExitToVehicle(vehicleId);
    }
    
    [Rpc(SendTo.NotOwner)]
    private void UpdateDataRpc(UnitData data)
    {
        _unit.Data.Position = data.Position;
        _unit.Data.Direction = data.Direction;
        _unit.Data.VelocityState = data.VelocityState;
        _unit.Data.Mode = data.Mode;
    }
}
