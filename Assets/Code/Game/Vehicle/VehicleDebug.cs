using UnityEngine;

public partial class Vehicle
{
    private void Debug()
    {
        var startPosition = _data.Position + Vector3.up;
        UnityEngine.Debug.DrawRay(startPosition, _data.Direction, Color.red);
        UnityEngine.Debug.DrawRay(startPosition, _data.Controller.transform.forward, Color.green);
    }
}