using System;
using UnityEngine;
using Object = System.Object;

public class VehicleView : MonoBehaviour
{
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
        transform.position = Vector3.Lerp(transform.position, _position, 5f * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, 4f * Time.deltaTime);
    }

    private void OnChangedProperty(VehicleData.Property property, Object value)
    {
        switch (property)
        {
            case VehicleData.Property.Position: _position = (Vector3)value; break;
            case VehicleData.Property.Rotation: _rotation = Quaternion.LookRotation((Vector3)value, Vector3.up); break;
        }
    }
}