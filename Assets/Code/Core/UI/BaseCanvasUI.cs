using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public abstract class BaseCanvasUI : MonoBehaviour
{
    public uint windowID { get; protected set; }
    
    protected Canvas _canvas;
    protected CanvasScaler _canvasScaler;

    public virtual void Init(Canvas canvas)
    {
        _canvas = canvas;
        _canvasScaler = _canvas.GetComponent<CanvasScaler>();
    }

    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public virtual bool ActiveSelf() => gameObject.activeSelf;

    public virtual Vector2 ScreenPointToUIPoint(Vector2 point)
    {
        return point / _canvas.scaleFactor;
    }

    public virtual void Subscribe() {}
}