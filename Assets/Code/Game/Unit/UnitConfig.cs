using UnityEngine;

[CreateAssetMenu(menuName = AssetDefineConstants.CONFIGS + "UnitConfig", fileName = "UnitConfig")]
public class UnitConfig : Config
{
    [Header("Locomotion")]
    public float WalkSpeed = 1.35f;
    public float RunSpeed = 3.4f;
}