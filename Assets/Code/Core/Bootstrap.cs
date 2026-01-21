using System.Collections;
using UnityEngine;

public interface IBootstrap { }

public class Bootstrap : MonoBehaviour, IBootstrap
{
    public static IBootstrap Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(LoadSplashScene());
    }

    private IEnumerator LoadSplashScene()
    {
        yield return StartCoroutine(Services.Scene.LoadScene(CoreConstants.Scene.SPLASH_LEVEL_INDEX));
        yield return new WaitForSeconds(1f);
        
        SetupTargetFramerate();
        
        Services.Scene.LoadSceneCoroutine(CoreConstants.Scene.GAME_LEVEL_INDEX);
    }

    private void SetupTargetFramerate()
    {
        Application.targetFrameRate = 60;
    }
}
