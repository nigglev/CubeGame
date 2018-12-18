using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStateType {Idle, Accelerate, GetSlow, Stick}

public interface ICameraStateOwner
{
    Vector3 GetVectorBetweenCameraAndPoint();
    Vector3 GetCameraVelocity();
    Vector3 GetPointVelocity();
    float GetCameraAcceleration();
    float GetCameraDeceleration();
    bool IsAngleObtuse();

    void StopAllCameraMovements();
    void SetCameraToAcceleration();
    void SetCameraToDeceleration();

}

public interface ICameraState
{
    EStateType Update(float in_time);
    EStateType GetStateType();
}

public class CCameraState_Idle : ICameraState
{
    ICameraStateOwner _owner;

    public CCameraState_Idle(ICameraStateOwner in_owner)
    {
        _owner = in_owner;
        //_owner.StopAllCameraMovements();
    }

    public EStateType Update(float in_time)
    {

        Vector3 cam_velocity_project = Vector3.Project(_owner.GetCameraVelocity(), _owner.GetVectorBetweenCameraAndPoint().normalized);
        Vector3 point_velocity_project = Vector3.Project(_owner.GetPointVelocity(), _owner.GetVectorBetweenCameraAndPoint().normalized);

        Debug.Log(string.Format("IDLE: Camera Velocity Project = {0} , Point Velocity Project = {1}, Distance = {2}", 
            cam_velocity_project.magnitude, point_velocity_project.magnitude, _owner.GetVectorBetweenCameraAndPoint().magnitude));

        if (cam_velocity_project.magnitude < point_velocity_project.magnitude)
        {
            Debug.Log(string.Format("GO TO ACCELERATE; Distance = {0}", _owner.GetVectorBetweenCameraAndPoint().magnitude));
            return EStateType.Accelerate;
        }
        
        return EStateType.Idle;
    }

    public EStateType GetStateType()
    {
        return EStateType.Idle;
    }
}

public class CCameraState_Accelerate : ICameraState
{
    ICameraStateOwner _owner;

    public CCameraState_Accelerate(ICameraStateOwner in_owner)
    {
        _owner = in_owner;
        //_owner.SetAccelerationType(EAccelerationType.Accelerate);
        _owner.SetCameraToAcceleration();
    }
    
    public EStateType Update(float in_time)
    {
        float cur_velocity = _owner.GetCameraVelocity().magnitude;
        float deceleration = _owner.GetCameraDeceleration();
        float break_distance = -(cur_velocity * cur_velocity) / (2 * deceleration);

        Debug.Log(string.Format("ACCELERATE: Break distance = {0}, Current Camera Velocity = {1}, Current Point Velocity = {2}, Distance Between Camera and Point = {3}", 
            break_distance, cur_velocity, _owner.GetPointVelocity().magnitude, _owner.GetVectorBetweenCameraAndPoint().magnitude));

        if (break_distance < _owner.GetVectorBetweenCameraAndPoint().magnitude)
            return EStateType.Accelerate;
        else
        {
            Debug.Log(string.Format("GO TO GETSLOW; Distance = {0}", _owner.GetVectorBetweenCameraAndPoint().magnitude));
            return EStateType.GetSlow;
        }
            

    }

    public EStateType GetStateType()
    {
        return EStateType.Accelerate;
    }
}

public class CCameraState_GetSlow : ICameraState
{
    ICameraStateOwner _owner;
    float _deceleration;

    public CCameraState_GetSlow(ICameraStateOwner in_owner)
    {
        _owner = in_owner;
        _owner.SetCameraToDeceleration();
    }

    public EStateType Update(float in_time)
    {
        //if(Mathf.Approximately(_owner.GetVectorBetweenCameraAndPoint().magnitude, 0f))
        //if(_owner.GetVectorBetweenCameraAndPoint().magnitude < 0.001f)
        if(_owner.IsAngleObtuse())
        {
            _owner.StopAllCameraMovements();
            Debug.Log(string.Format("GO TO IDLE; Distance = {0}", _owner.GetVectorBetweenCameraAndPoint().magnitude));
            return EStateType.Idle;
        }


        Vector3 cam_velocity_project = Vector3.Project(_owner.GetCameraVelocity(), _owner.GetVectorBetweenCameraAndPoint().normalized);
        Vector3 point_velocity_project = Vector3.Project(_owner.GetPointVelocity(), _owner.GetVectorBetweenCameraAndPoint().normalized);

        if (cam_velocity_project.magnitude < point_velocity_project.magnitude)
        {
            Debug.Log(string.Format("GO TO ACCELERATE; Distance = {0}", _owner.GetVectorBetweenCameraAndPoint().magnitude));
            return EStateType.Accelerate;
        }

        Debug.Log(string.Format("GETSLOW: Current Camera Velocity = {0}, Current Point Velocity = {1}, Distance Between Camera and Point = {2}", 
            _owner.GetCameraVelocity().magnitude, _owner.GetPointVelocity().magnitude, _owner.GetVectorBetweenCameraAndPoint().magnitude));

        return EStateType.GetSlow;
    }

    public EStateType GetStateType()
    {
        return EStateType.GetSlow;
    }
}

public class DesiredCameraPoint
{
    Vector3 _current_pos;
    Vector3 _prev_pos;
    Vector3 _current_velocity;
    Vector3 _direction;
    float _time_passed;
    float _distance_between_positions;

    public DesiredCameraPoint(Vector3 in_start_pos, Vector3 in_start_velocity)
    {
        _current_pos = in_start_pos;
        _current_velocity = in_start_velocity;
    }

    public Vector3 GetCurrentPosition() { return _current_pos; }
    public Vector3 GetCurrentVelocity() { return _current_velocity; }
    public Vector3 GetDirection() { return _direction; }
    public float GetDistanceBetweenPositions() { return _distance_between_positions; }

    public void SetCurrentPosition(Vector3 in_cur_pos, float in_time_passed)
    {
        _time_passed = in_time_passed;
        _prev_pos = _current_pos;
        _current_pos = in_cur_pos;
        _distance_between_positions = (_current_pos - _prev_pos).magnitude;
        _direction = (_current_pos - _prev_pos).normalized;
        _current_velocity = _direction * _distance_between_positions / in_time_passed;
    }

}



public class CameraController4 : MonoBehaviour, ICameraStateOwner
{
    private Camera _cam;
    private Transform _cam_transform;
    private Transform _player;  
    
    private float _distance = 2f;
    private Vector3 _camera_shift;


    private DesiredCameraPoint _desired_camera_point;
    private ICameraState _current_state;

    private Vector3 _velocity;
    private float _fvel;
    [SerializeField]
    private float _acceleration_value = 1.5f;
    [SerializeField]
    private float _deceleration_value = -0.5f;
    private float _acc;
    bool _is_obtuse_angle;



    

    void Start ()
    {

        CWorld.Instance.SetCamera4(this);
        _cam = Camera.main;
        _cam_transform = transform;
        _camera_shift = new Vector3(0, 1f, -_distance);

        CWorld.Instance.OnNewPlayer += OnNewPlayer;
        if (CWorld.Instance.GetPlayer() != null)
            OnNewPlayer(this, EventArgs.Empty);

        _current_state = new CCameraState_Idle(this);
        _desired_camera_point = new DesiredCameraPoint(DesiredCameraPos(), Vector3.zero);
        _cam_transform.position = DesiredCameraPos();

        //_acceleration_value = 10f;
        //_deceleration_value = -0.5f;
        _acc = 0f;
        _fvel = 0f;
        _is_obtuse_angle = false;
    }


    //void Update() { } 


    public void CameraHandler(float in_time)
    {
        if (_player == null)
            return;

        _desired_camera_point.SetCurrentPosition(DesiredCameraPos(), in_time);
        UpdateCameraVelocityAndPosition(in_time);

        EStateType new_state = _current_state.Update(in_time);
        if (new_state != _current_state.GetStateType())
        {
            ICameraState old_state = _current_state;
            _current_state = CreateStateByType(new_state);
            _current_state.Update(0f);
        }
    }


    private ICameraState CreateStateByType(EStateType in_state)
    {
        switch (in_state)
        {
            case EStateType.Idle: return new CCameraState_Idle(this);
            case EStateType.Accelerate: return new CCameraState_Accelerate(this);
            case EStateType.GetSlow: return new CCameraState_GetSlow(this);
        }

        return null;
    }

    public void StopAllCameraMovements()
    {
        _velocity = Vector3.zero;
        _fvel = 0f;
        _acc = 0f;
    }

    public void UpdateCameraVelocityAndPosition(float in_time)
    {

        Vector3 dir_norm = GetVectorBetweenCameraAndPoint().normalized;
        _velocity = Vector3.Project(_velocity, dir_norm);

        Vector3 next_camera_pos = _cam_transform.position + _velocity * in_time + dir_norm * _acc * in_time * in_time / 2;
        Vector3 next_vector = _desired_camera_point.GetCurrentPosition() - next_camera_pos;
        float angle = Vector3.Angle(dir_norm, next_vector);

        

        if (angle < 90f)
        {
            _cam_transform.position = next_camera_pos;
            _is_obtuse_angle = false;
        }
        else
            _is_obtuse_angle = true;
        //_cam_transform.position += _velocity * in_time + dir_norm * in_acceleration * in_time * in_time / 2;
        _fvel += _acc * in_time;
        _velocity = _fvel * dir_norm;        

        Vector3 relative_position = _player.position - _cam_transform.position;
        _cam_transform.rotation = Quaternion.LookRotation(relative_position);
    }

    public Vector3 DesiredCameraPos()
    {
        return _player.position + _player.rotation * _camera_shift;
    }

    public Vector3 GetVectorBetweenCameraAndPoint()
    {
        return _desired_camera_point.GetCurrentPosition() - _cam_transform.position;
    }

    public Vector3 GetCameraVelocity()
    {
        return _velocity;
    }

    public bool IsAngleObtuse()
    {
        return _is_obtuse_angle;
    }

    public Vector3 GetPointVelocity()
    {
        return _desired_camera_point.GetCurrentVelocity();
    }

    public float GetCameraAcceleration()
    {
        return _acceleration_value;
    }

    public float GetCameraDeceleration()
    {
        return _deceleration_value;
    }

    public void SetCameraToAcceleration()
    {
        _acc = _acceleration_value;
    }

    public void SetCameraToDeceleration()
    {
        _acc = _deceleration_value;
    }

    private void OnNewPlayer(object sender, EventArgs e)
    {
        _player = CWorld.Instance.GetPlayer();
    }

    private void OnDrawGizmos() 
    {
        if (_player == null)
            return;

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(_player.position + _player.rotation * _camera_shift, 0.1f);
    }
}
