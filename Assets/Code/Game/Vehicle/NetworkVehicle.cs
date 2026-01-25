using UnityEngine;

public class NetworkVehicle
{
    private Vehicle _vehicle;
    
    private VehicleData _data => _vehicle.Data;
    
    private VehicleConfig _config => Configs.Get<VehicleConfig>();
    
    public NetworkVehicle(Vehicle vehicle)
    {
        _vehicle = vehicle;
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        _data.VelocityClamp = Mathf.Abs(_data.CurrentVelocity) / (_config.MaximumSpeed * _config.AccelerationMultiplier);
        _data.Controller.transform.position = Vector3.Lerp(_data.Controller.transform.position, _data.Position, NetworkService.NETWORK_RATE_UPDATE * Time.deltaTime);
        _data.Controller.transform.rotation = Quaternion.Lerp(_data.Controller.transform.rotation, Quaternion.LookRotation(_data.Rotation), NetworkService.NETWORK_RATE_UPDATE * Time.deltaTime);
    }
}