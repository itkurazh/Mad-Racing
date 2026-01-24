using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Vehicle[] _vehicles;
    
    private void Start()
    {
        Services.Game.Load();
        Services.UI.Get<LobbyUI>().Show();
        
        foreach (var vehicle in _vehicles)
            Services.Game.Context.RegisterVehicle(vehicle);
    }
}