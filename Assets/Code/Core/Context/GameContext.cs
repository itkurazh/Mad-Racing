using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameContext : ICoreSystem
{
    private GameConfig _config => Configs.Get<GameConfig>();
    
    public Player Player { get; private set; }
    public CameraController Camera { get; private set; }
    
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
    
    public bool TryEnterVehicle(Player player, Vehicle vehicle)
    {
        player.SwitchState(PlayerModeID.Vehicle);
        return true;
    }

    public bool TryExitVehicle(Player player, Vehicle vehicle)
    {
        player.SwitchState(PlayerModeID.Character);
        return true;
    }
}