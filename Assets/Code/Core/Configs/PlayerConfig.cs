using UnityEngine;

[CreateAssetMenu(menuName = AssetDefineConstants.CONFIGS + "PlayerConfig", fileName = "PlayerConfig")]
public class PlayerConfig : Config
{
    public float WalkSpeed = 1.35f;
    public float RunSpeed = 3.4f;
}