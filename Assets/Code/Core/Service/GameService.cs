using UnityEngine;

public interface IGameService
{
    GameContext Context { get; }

    void Load();
}

public class GameService : MonoBehaviour, IGameService
{
    public static IGameService Instance { get; private set; }

    public GameContext Context { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        Context = new GameContext();
    }

    private void Update()
    {
        Context.Execute();
    }

    public void Load()
    {
        Context.Init();
    }
}