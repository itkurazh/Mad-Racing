using System;
using UnityEngine;
using Object = System.Object;

public class UnitView : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private UnitData _data;
    
    private Vector3 _position;
    private Vector3 _direction;
    private UnitData.VelocityStateID _velocityID;
    
    public void SetData(UnitData data)
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
        Locomotion();
        Velocity();
    }

    private void Locomotion()
    {
        transform.position = Vector3.Lerp(
            transform.position, 
            _position, 
            UnitConstants.LERP_VALUE * Time.deltaTime);
        
        if(_direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(_direction, Vector3.up),
                UnitConstants.LERP_VALUE * Time.deltaTime);
        }
    }

    private void Velocity()
    {
        float velocityFrom = _animator.GetFloat(UnitConstants.ANIMATOR_VELOCITY);
        float velocityTo = 0;
        
        switch (_velocityID)
        {
            case UnitData.VelocityStateID.Idle: velocityTo = 0f; break;
            case UnitData.VelocityStateID.Walking: velocityTo = 0.7f; break;
            case UnitData.VelocityStateID.Running: velocityTo = 1f; break;
        }
        
        float lerp = Mathf.Lerp(velocityFrom, velocityTo, UnitConstants.LERP_VALUE *  Time.deltaTime);
        _animator.SetFloat(UnitConstants.ANIMATOR_VELOCITY, lerp);
    }

    private void OnChangedProperty(UnitData.Property property, Object value)
    {
        switch (property)
        {
            case UnitData.Property.Position:
                _position = (Vector3)value;
                break;
            
            case UnitData.Property.Direction:
                _direction = (Vector3)value;
                break;
            
            case UnitData.Property.VelocityState:
                _velocityID = (UnitData.VelocityStateID)value;
                break;
        }
    }
}