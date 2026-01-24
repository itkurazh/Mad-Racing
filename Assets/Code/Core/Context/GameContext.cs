using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameContext : ICoreSystem
{
    private GameConfig _config => Configs.Get<GameConfig>();
    
    public Player Player { get; private set; }
    public CameraController Camera { get; private set; }
    
    private List<Vehicle> _vehicles = new ();
    
    public void Init()
    {
        
    }

    public void DeInit()
    {
        
    }

    public void Execute()
    {
        
    }

    public Player CreatePlayer()
    {
        Player = Object.Instantiate(_config.PlayerPrefab);
        return Player;
    }

    public CameraController CreateCamera()
    {
        Camera = Object.Instantiate(_config.CameraPrefab);
        return Camera;
    }

    public void RegisterVehicle(Vehicle vehicle)
    {
        if(!_vehicles.Contains(vehicle))
            _vehicles.Add(vehicle);
    }

    public void EnterToVehicle(ulong clientId, ulong vehicleId)
    {
        foreach (var vehicle in _vehicles)
        {
            if (vehicle.Network.NetworkObjectId.Equals(vehicleId))
            {
                Services.Network.EnterToVehicle(clientId, vehicle);
                break;
            }
        }
    }

    public void ExitToVehicle(ulong vehicleId)
    {
        foreach (var vehicle in _vehicles)
        {
            if (vehicle.Network.NetworkObjectId.Equals(vehicleId))
            {
                Services.Network.ExitToVehicle(vehicle);
                break;
            }
        }
    }
}