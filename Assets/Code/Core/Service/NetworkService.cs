using System;
using Unity.Netcode;
using UnityEngine;

public interface INetworkService
{
    bool StartClient();
    bool StartHost();
}

public class NetworkService : MonoBehaviour, INetworkService
{
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