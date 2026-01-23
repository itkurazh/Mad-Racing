using System;
using UnityEngine;
using Object = System.Object;

public class VehicleView : MonoBehaviour
{
    [Header("Suspension")]
    [SerializeField] private Transform _root;
    [SerializeField] private Transform _pivotForward;
    [SerializeField] private Transform _pivotSide;
    
    [Header("Wheels")]
    [SerializeField] private Transform[] _wheels;
    
    [Header("Effect")]
    [SerializeField] private ParticleSystem[] _effectsSmoke;
    [SerializeField] private ParticleSystem[] _effectsTrailVFX;
    
    private VehicleData _data;
    
    private Vector3 _position;
    private Vector3 _lastPosition;
    private Vector3 _massDirection;
    private Quaternion _rotation;
    private float _massVelocity;
    private float _lerpVelocity;

    public float Output;
    
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
        
        _massDirection = transform.InverseTransformVector(transform.position - _lastPosition).normalized;
        float distance = Vector3.Distance(transform.position, _lastPosition);
        _massVelocity = distance / Time.deltaTime;
        _lastPosition = transform.position;
        _lerpVelocity = Mathf.Lerp(_lerpVelocity, _massVelocity, VehicleConstants.LERP_VALUE * Time.deltaTime);

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
        {
            var sideValue = Mathf.Clamp(_massVelocity * _massDirection.x, -VehicleConstants.VIEW_SIDE_MAX, VehicleConstants.VIEW_SIDE_MAX);
            sideValue *= VehicleConstants.VIEW_SIDE_POWER;

            var sideRotation = Quaternion.Euler(Vector3.back * sideValue);
            _pivotSide.localRotation = Quaternion.Lerp(_pivotSide.localRotation, sideRotation,
                VehicleConstants.LERP_VALUE * Time.deltaTime);

            var sideLerpPos = Mathf.Lerp(0, 0.1f, sideValue / VehicleConstants.VIEW_SIDE_MAX);
            _pivotSide.localPosition = Vector3.Lerp(_pivotSide.localPosition, Vector3.down * sideLerpPos, VehicleConstants.LERP_VALUE * Time.deltaTime);
        }

        {
            var forwardValue = Mathf.Clamp(_massVelocity - _lerpVelocity, -VehicleConstants.VIEW_FORWARD_MAX, VehicleConstants.VIEW_FORWARD_MAX);
            forwardValue *= VehicleConstants.VIEW_FORWARD_POWER;

            var forwardRotation = Quaternion.Euler(Vector3.left * forwardValue);
            _pivotForward.localRotation = Quaternion.Lerp(_pivotForward.localRotation, forwardRotation,
                VehicleConstants.LERP_VALUE * Time.deltaTime);
        }
        
        {
            var forceRoot = Vector3.forward * (_data.LerpVelocity * -_massDirection.z); 
            _root.localPosition = Vector3.Lerp(_root.localPosition, forceRoot, VehicleConstants.LERP_VALUE * Time.deltaTime);
        }
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
            }
            else
            {
                var wVelocity = Mathf.Max(Mathf.Abs(_data.CurrentVelocity), _massVelocity);
                _wheels[i].GetChild(0).Rotate(Vector3.right, _data.InputDirection * wVelocity * VehicleConstants.VIEW_WHEEL_RAD);
            }
            
            _wheels[i].GetChild(0).Rotate(Vector3.right, _massDirection.z * _massVelocity * VehicleConstants.VIEW_WHEEL_RAD);
        }
        
        Output = _massVelocity - _data.CurrentVelocity;
        
        if(Mathf.Abs(_massDirection.x) * _massVelocity > 0.2f && _data.CurrentVelocity > 1)
        {
            if(!_effectsTrailVFX[0].isPlaying)
                _effectsTrailVFX.Play();
        }
        else
        {
            if(_effectsTrailVFX[0].isPlaying)
                _effectsTrailVFX.Stop();
        }
        
        if(_data.CurrentVelocity > _massVelocity && _data.CurrentVelocity > 1)
        {
            if(!_effectsSmoke[0].isPlaying)
                _effectsSmoke.Play();
        }
        else
        {
            if(_effectsSmoke[0].isPlaying)
                _effectsSmoke.Stop();
        }
        
        //print($"Mass: {_massVelocity} && {_data.CurrentVelocity}");
    }
}