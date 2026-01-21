using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneService
{
    void LoadSceneCoroutine(int buildIndex);
    IEnumerator LoadScene(int buildIndex);
}

public class SceneService : MonoBehaviour, ISceneService
{
    public static ISceneService Instance { get; private set; }

    private int _currentBuildIndex = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (Services.Boot == null)
            _currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadSceneCoroutine(int buildIndex)
    {
        StartCoroutine(LoadScene(buildIndex));
    }

    public IEnumerator LoadScene(int buildIndex)
    {
        if (_currentBuildIndex != -1)
            yield return StartCoroutine(UnloadScene(_currentBuildIndex));

        if (!Application.CanStreamedLevelBeLoaded(buildIndex))
            yield break;

        var task = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);

        while (!task.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(buildIndex));

        _currentBuildIndex = buildIndex;
    }

    private IEnumerator UnloadScene(int buildIndex)
    {
        var task = SceneManager.UnloadSceneAsync(buildIndex);

        while (!task.isDone)
            yield return null;

        _currentBuildIndex = -1;
    }
}
