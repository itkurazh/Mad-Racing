using UnityEngine;

public class NetworkPlayer
{
    private Player _player;
    
    private SphereCollider _triggerCollider;
    
    public NetworkPlayer(Player player)
    {
        _player = player;
        _triggerCollider = player.GetComponent<SphereCollider>();
        _triggerCollider.enabled = false;
    }
    
    public void Start()
    {
        
    }

    public void Update()
    {
        if(Vector3.Distance(_player.Unit.Data.Controller.transform.position , _player.Unit.Data.Position) > 1f)
            _player.Unit.View.transform.position = _player.Unit.Data.Position;
            
        _player.Unit.Data.Controller.transform.position = Vector3.Lerp(_player.Unit.Data.Controller.transform.position, _player.Unit.Data.Position, NetworkService.NETWORK_RATE_UPDATE * Time.deltaTime);
            
        if(_player.Unit.Data.Direction != Vector3.zero)
            _player.Unit.View.transform.rotation = Quaternion.Lerp(_player.Unit.View.transform.rotation, Quaternion.LookRotation(_player.Unit.Data.Direction), NetworkService.NETWORK_RATE_UPDATE * Time.deltaTime);
        
        _player.Unit.View.gameObject.SetActive(_player.Unit.Data.Mode == UnitData.ModeID.Character);
    }
}