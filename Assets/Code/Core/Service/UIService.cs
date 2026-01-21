using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUIController
{
    T Get<T>() where T : BaseCanvasUI;
}

public class UIService : MonoBehaviour, IUIController
{
    public static IUIController Instance { get; private set; }
    
    [SerializeField]
    private Canvas _canvasPrefab;

    [SerializeField]
    private BaseCanvasUI[] _content;

    private Dictionary<Type, BaseCanvasUI> _screens;

    private int _currentCanvasOrder;

    private void Awake()
    {
        Instance = this;
        _screens = new Dictionary<Type, BaseCanvasUI>();
        InitScreens();
    }

    private void InitScreens()
    {
        foreach (var screen in _content)
        {
            var obj = CreateScreen(screen);
            obj.SetActive(false);

            Register(obj);
        }
    }

    public T Get<T>() where T : BaseCanvasUI
    {
        return _screens[typeof(T)] as T;
    }

    private void Register<T>(T screen) where T : BaseCanvasUI
    {
        var screenType = screen.GetType();
        Debug.Log($"UI \"{screenType.Name}\" screen registered.");

        _screens[screenType] = screen.GetComponent<T>();
    }

    public void HideAll()
    {
        foreach (var screen in _screens.Values)
            screen.SetActive(false);
    }

    private BaseCanvasUI CreateScreen(BaseCanvasUI screenPrefab)
    {
        var canvas = Instantiate(_canvasPrefab);
        var screen = Instantiate(screenPrefab, canvas.transform);
        screen.gameObject.name = screenPrefab.name;

        screen.Init(canvas);
        canvas.gameObject.name = $"{screenPrefab.name} Canvas";
        canvas.sortingOrder = _currentCanvasOrder;
        _currentCanvasOrder++;

        return screen;
    }
}
