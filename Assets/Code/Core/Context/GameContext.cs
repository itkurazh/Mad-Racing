using System;
using System.Collections.Generic;
using UnityEngine;

public class GameContext : ICoreSystem
{
    public void Init()
    {
        
    }

    public void DeInit()
    {
        
    }

    public void Execute()
    {
        
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