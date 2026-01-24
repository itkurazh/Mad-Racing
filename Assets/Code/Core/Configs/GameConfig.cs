using UnityEngine;

[CreateAssetMenu(menuName = AssetDefineConstants.CONFIGS + "GameConfig", fileName = "GameConfig")]
public class GameConfig : Config
{
    [Header("Prefabs")]
    public Player PlayerPrefab;
    public CameraController CameraPrefab;
}