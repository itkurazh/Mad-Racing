using UnityEngine;

public interface IGameService
{
    GameContext Context { get; }

    void Load();
    void StartGame();
    void ConnectToGame();
}

public class GameService : MonoBehaviour, IGameService
{
    public static IGameService Instance { get; private set; }

    public GameContext Context { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        Context = new GameContext();
        Context.Init();
    }

    private void Update()
    {
        Context.Execute();
    }

    public void Load()
    {
        Context.Init();
    }

    public void StartGame()
    {
        if(Services.Network.StartHost())
            Services.UI.Get<LobbyUI>().Hide();
    }

    public void ConnectToGame()
    {
        if(Services.Network.StartClient())
            Services.UI.Get<LobbyUI>().Hide();
    }
}