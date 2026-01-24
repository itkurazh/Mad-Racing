using System;
using Unity.Netcode;
using UnityEngine;

public abstract class Entity :  NetworkBehaviour
{
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
}