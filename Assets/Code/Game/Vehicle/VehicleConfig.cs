using UnityEngine;

[CreateAssetMenu(menuName = AssetDefineConstants.CONFIGS + "VehicleConfig", fileName = "VehicleConfig")]
public class VehicleConfig : Config
{
    [Header("Vehicle Settings")]
    public AnimationCurve Acceleration;
    public float AccelerationMultiplier;
    public float MaximumSpeed = 7;
    
    public AnimationCurve AngularAcceleration;
    public float AngularSpeed = 7;

    public int Traction;
}