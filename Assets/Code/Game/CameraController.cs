using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 Direction => _direction;

    [SerializeField] private DataState[] _states;

    private StateID _currentState;
    private Transform _lookAt;
    private Transform _target;
    private Vector3 _rotation;
    private Vector3 _direction;
    
    private CinemachineBrain _brain;
    
    private CameraConfig _config => Configs.Get<CameraConfig>();

    private void Awake()
    {
        _lookAt = new GameObject("LookAt").transform;
        
        foreach (var data in _states)
            data.VirtualCamera.Follow = _lookAt;
        
        _brain = GetComponentInChildren<CinemachineBrain>();
    }

    private void Update()
    {
        if(!_target) return;
        
        if(_currentState == StateID.Character)
            FollowCharacter();
        
        _direction = Vector3.Cross(_lookAt.right, Vector3.up);
    }

    private void LateUpdate()
    {
        if(_currentState == StateID.Vehicle)
            FollowVehicle();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void ChangeState(StateID id)
    {
        if(_currentState == id) return;;

        foreach (var data in _states)
            data.VirtualCamera.gameObject.SetActive(id == data.ID);
        
        _currentState = id;
    }

    private void FollowCharacter()
    {
        _lookAt.position = _target.position;
        
        _rotation += Vector3.left * (Input.GetAxis("Mouse Y") * _config.Sensitivity);
        _rotation += Vector3.up * (Input.GetAxis("Mouse X") * _config.Sensitivity);
        _rotation.x = Mathf.Clamp(_rotation.x, _config.MinMaxAngle.x, _config.MinMaxAngle.y);
        
        _lookAt.rotation = Quaternion.Euler(_rotation);
        
        _brain.ManualUpdate();
    }

    private void FollowVehicle()
    {
        _lookAt.position = Vector3.Lerp(_lookAt.position, _target.position, 5f * Time.deltaTime);
        _lookAt.rotation = Quaternion.Lerp(_lookAt.rotation, _target.rotation, 5f * Time.deltaTime);
        
        _brain.ManualUpdate();
    }
    
    public enum StateID
    {
        None = 0,
        Character = 1,
        Vehicle = 2
    }
    
    [System.Serializable]
    public class DataState
    {
        public StateID ID;
        public CinemachineCamera VirtualCamera;
    }
}