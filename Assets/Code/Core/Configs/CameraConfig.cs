using UnityEngine;

[CreateAssetMenu(menuName = AssetDefineConstants.CONFIGS + "CameraConfig", fileName = "CameraConfig")]
public class CameraConfig : Config
{
    public Vector2 MinMaxAngle;
    [Range(0f, 5f)] public float Sensitivity;
}