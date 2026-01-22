using System;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 position)
    {
        _rigidbody.linearVelocity = position;
    }
    
    public void Rotate(Vector3 rotation)
    {
        _rigidbody.angularVelocity = rotation;
    }
}