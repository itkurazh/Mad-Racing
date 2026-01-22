using UnityEngine;

[CreateAssetMenu(menuName = AssetDefineConstants.CONFIGS + "VehicleConfig", fileName = "VehicleConfig")]
public class VehicleConfig : Config
{
    public AnimationCurve Acceleration;
    public float AccelerationMultiplier;
    public float LinearSpeed = 7;
    
    public AnimationCurve AngularAcceleration;
    public float AngularSpeed = 7;
}