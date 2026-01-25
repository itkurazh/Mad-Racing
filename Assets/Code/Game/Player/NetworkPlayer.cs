using UnityEngine;

public class NetworkPlayer
{
    private Player _player;
    
    public NetworkPlayer(Player player)
    {
        _player = player;
    }
    
    public void Start()
    {
        
    }

    public void Update()
    {
        if(Vector3.Distance(_player.Unit.Data.Controller.transform.position , _player.Unit.Data.Position) > 1f)
            _player.Unit.View.transform.position = _player.Unit.Data.Position;
            
        _player.Unit.Data.Controller.transform.position = _player.Unit.Data.Position;
            
        if(_player.Unit.Data.Direction != Vector3.zero)
            _player.Unit.View.transform.rotation = Quaternion.LookRotation(_player.Unit.Data.Direction);
            
        _player.Unit.View.gameObject.SetActive(_player.Unit.Data.Mode == UnitData.ModeID.Character);
    }
}