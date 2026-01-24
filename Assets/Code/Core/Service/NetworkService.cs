using System;
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
    public const ulong UNKNOWD_ID = 4004;
    
    public static INetworkService Instance { get; private set; }

    private NetworkManager _network => NetworkManager.Singleton;
    
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
            _network.OnClientConnectedCallback += OnOnClientConnectedCallback;
            
            SpawnPlayer(_network.LocalClientId);
            
            Debug.Log("Host started");
        }
        
        return result;
    }

    public void StopHost()
    {
        _network.OnClientConnectedCallback -= OnOnClientConnectedCallback;
        
        _network.Shutdown();
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
        
        Debug.Log($"Spawned player id:{clientId}");
    }
}