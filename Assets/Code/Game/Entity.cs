using System;
using Unity.Netcode;
using UnityEngine;

public abstract class Entity :  NetworkBehaviour
{
    private NetworkObject _network;
    public NetworkObject Network => GetNetwork();
    
    protected virtual void Awake()
    {
        Subscribe();
    }

    protected abstract void Subscribe();
    protected abstract void Unsubscribe();

    public override void OnDestroy()
    {
        Unsubscribe();
    }
    
    private NetworkObject GetNetwork()
    {
        if(!_network)
            _network = GetComponent<NetworkObject>();
        
        return _network;
    }
}