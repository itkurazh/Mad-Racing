using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUIController
{
    T Get<T>() where T : WindowUI;
}

public class UIService : MonoBehaviour, IUIController
{
    public static IUIController Instance { get; private set; }
    
    [SerializeField]
    private Canvas _canvasPrefab;

    [SerializeField]
    private WindowUI[] _content;

    private Dictionary<Type, WindowUI> _screens;

    private int _currentCanvasOrder;

    private void Awake()
    {
        Instance = this;
        _screens = new Dictionary<Type, WindowUI>();
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

    public T Get<T>() where T : WindowUI
    {
        return _screens[typeof(T)] as T;
    }

    private void Register<T>(T screen) where T : WindowUI
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

    private WindowUI CreateScreen(WindowUI screenPrefab)
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
