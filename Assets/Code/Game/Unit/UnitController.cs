using System;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Vector3 Position => _characterController.transform.position;
    
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector3 velocity)
    {
        _characterController.Move(velocity);
        _characterController.transform.position = Position.WithY(0);
    }
}