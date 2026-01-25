using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface INetworkService
{
    bool StartClient();
    bool StartHost();
    void EnterToVehicle(ulong clientId, Vehicle vehicle);
    void ExitToVehicle(Vehicle vehicle);
}

public class NetworkService : MonoBehaviour, INetworkService
{
    public const float THERHOLD_SLEEP_VALUE = 0.1f;
    public const int NETWORK_RATE_UPDATE = 15;
    
    public static INetworkService Instance { get; private set; }

    private NetworkManager _network => NetworkManager.Singleton;
    
    private List<Player> _players = new ();
    
    private void Awake()
    {
        Instance = this;
    }

    public bool StartClient()
    {
        var result = _network.StartClient();
        
        if(result)
        {
            Debug.Log("Client started");
        }
        
        return result;
    }

    public bool StartHost()
    {
        var result = _network.StartHost();
        
        if(result)
        {
            Debug.Log("Host started");
            
            _network.OnClientConnectedCallback += OnOnClientConnectedCallback;
            
            SpawnPlayer(_network.LocalClientId);
        }
        
        return result;
    }

    public void StopHost()
    {
        _network.OnClientConnectedCallback -= OnOnClientConnectedCallback;
        _network.Shutdown();
        _players.Clear();
        
        Debug.Log("Host stoped");
    }

    public void EnterToVehicle(ulong clientId, Vehicle vehicle)
    {
        vehicle.Network.ChangeOwnership(clientId);
    }
    
    public void ExitToVehicle(Vehicle vehicle)
    {
        vehicle.Network.ChangeOwnership(0);
    }

    private void OnOnClientConnectedCallback(ulong clientId)
    {
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        var player = Services.Game.Context.CreatePlayer();
        player.Network.SpawnAsPlayerObject(clientId);
        _players.Add(player);
        
        Debug.Log($"Spawned player ID:[{clientId}]");
    }
}