using System;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Vector3 Position => transform.position;
    
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector3 velocity)
    {
        _characterController.Move(velocity);
    }
}