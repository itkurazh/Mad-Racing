using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform LookAt => _lookAt;
    
    [SerializeField] private Transform _lookAt;

    private Transform _target;
    
    private Vector3 _rotation;
    
    private CameraConfig _config => Configs.Get<CameraConfig>();

    private void Update()
    {
        if(!_target) return;
        
        _lookAt.position = _target.position;
        
        _rotation += (Input.GetAxis("Mouse Y") * Vector3.left + Input.GetAxis("Mouse X") * Vector3.up) * _config.Sensitivity;
        _rotation.x = Mathf.Clamp(_rotation.x, _config.MinMaxAngle.x, _config.MinMaxAngle.y);
        
        _lookAt.rotation = Quaternion.Euler(_rotation);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}