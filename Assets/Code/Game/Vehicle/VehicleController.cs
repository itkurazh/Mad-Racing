using System;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 velocity)
    {
        _rigidbody.linearVelocity = velocity;
    }
    
    public void Rotate(Vector3 velocity)
    {
        _rigidbody.angularVelocity = velocity;
    }

    public void AdditiveSide()
    {
        _rigidbody.linearVelocity += -transform.right * _rigidbody.angularVelocity.y;
    }
}