using System;
using UnityEngine;
using Object = System.Object;

public class VehicleView : MonoBehaviour
{
    [SerializeField] private Transform _root;
    [SerializeField] private Transform _pivot;
    [SerializeField] private Transform[] _wheels;
    
    private VehicleData _data;
    
    private Vector3 _position;
    private Quaternion _rotation;
    
    public void SetData(VehicleData data)
    {
        _data = data;
        
        _data.OnChangedProperty += OnChangedProperty;
    }

    public void Free()
    {
        _data.OnChangedProperty -= OnChangedProperty;

        _data = null;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _position, VehicleConstants.LERP_VALUE * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, VehicleConstants.LERP_VALUE * Time.deltaTime);

        WheelsControl();
        Suspension();
    }

    private void OnChangedProperty(VehicleData.Property property, Object value)
    {
        switch (property)
        {
            case VehicleData.Property.Position: _position = (Vector3)value; break;
            case VehicleData.Property.Rotation: _rotation = Quaternion.LookRotation((Vector3)value, Vector3.up); break;
        }
    }

    private void Suspension()
    {
        _root.localRotation = Quaternion.Euler(Vector3.forward * 2 * _data.InputSide);
        _pivot.localRotation = Quaternion.Euler(Vector3.left * 2 * _data.InputDirection);
    }

    public void WheelsControl()
    {
        for (int i = 0; i < _wheels.Length; i++)
        {
            if(i < 2)
            {
                var rotation = Vector3.up * (45 * _data.InputSide);

                _wheels[i].localRotation = Quaternion.Lerp(
                    _wheels[i].localRotation,
                    Quaternion.Euler(rotation),
                    VehicleConstants.LERP_VALUE * Time.deltaTime);
                
                _wheels[i].GetChild(0).Rotate(Vector3.right, _data.InputDirection * _data.CurrentVelocity);
            }
            else
            {
                _wheels[i].GetChild(0).Rotate(Vector3.right, _data.RigidbodyVelocity);
            }
        }
    }
}