using UnityEngine;
using UnityEngine.SceneManagement;

public static class Runtime
{
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        SceneManager.LoadScene(CoreConstants.Scene.SERVICES_LEVEL_INDEX, LoadSceneMode.Additive);
        SceneManager.LoadScene(CoreConstants.Scene.UI_LEVEL_INDEX, LoadSceneMode.Additive);
    }
}