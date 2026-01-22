using System;
using UnityEngine;

public abstract class Entity :  MonoBehaviour
{
    protected virtual void Awake()
    {
        Subscribe();
    }

    protected abstract void Subscribe();
    protected abstract void Unsubscribe();
    
    private void OnDestroy()
    {
        Unsubscribe();
    }
}