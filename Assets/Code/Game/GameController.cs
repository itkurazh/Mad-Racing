using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Start()
    {
        Services.Game.Load();
        Services.UI.Get<LobbyUI>().Show();
    }
}