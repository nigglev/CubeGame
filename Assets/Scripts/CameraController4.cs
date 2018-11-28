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
    float GetCameraDeceleration();
    float GetPrevFrameDistance();

    void TurnOnOffAcceleration(bool is_turn_on);
    void SetCameraPositionToDesired();
    void SetStopAccelerating(bool is_to_stop);
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
    }

    public EStateType Update(float in_time)
    {

        Vector3 cam_velocity_project = Vector3.Project(_owner.GetCameraVelocity(), _owner.GetVectorBetweenCameraAndPoint().normalized);
        Vector3 point_velocity_project = Vector3.Project(_owner.GetPointVelocity(), _owner.GetVectorBetweenCameraAndPoint().normalized);

        Debug.Log(string.Format("IDLE: Camera Velocity Project = {0} , Point Velocity Project = {1}", 
            cam_velocity_project.magnitude, point_velocity_project.magnitude));

        if (cam_velocity_project.magnitude < point_velocity_project.magnitude)
        {
            Debug.Log("GO TO ACCELERATE");
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
        _owner.TurnOnOffAcceleration(true);
        _owner.SetStopAccelerating(false);
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
            Debug.Log("GO TO GETSLOW");
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

    public CCameraState_GetSlow(ICameraStateOwner in_owner)
    {
        _owner = in_owner;
        _owner.TurnOnOffAcceleration(false);
    }

    public EStateType Update(float in_time)
    {
        if(_owner.GetVectorBetweenCameraAndPoint().magnitude > _owner.GetPrevFrameDistance())
        {
            _owner.SetCameraPositionToDesired();
        }

        if (Mathf.Approximately(_owner.GetPointVelocity().magnitude, 0f))
        {
            _owner.SetCameraPositionToDesired();
            _owner.SetStopAccelerating(true);
            return EStateType.Idle;
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


//public class CCameraState_Stick : ICameraState
//{
//    ICameraStateOwner _owner;

//    public CCameraState_Stick(ICameraStateOwner in_owner)
//    {
//        _owner = in_owner;
//    }

//    public EStateType Update(float in_time)
//    {
//        _owner.SetCameraPositionToDesired();

//        if (_owner.GetPointVelocity().magnitude < 0.01f)
//            return EStateType.Idle;

//        Debug.Log(string.Format("STICK: Current Camera Velocity = {0}, Current Point Velocity = {1}, Distance Between Camera and Point = {2}", 
//            _owner.GetCameraVelocity().magnitude, _owner.GetPointVelocity().magnitude, _owner.GetVectorBetweenCameraAndPoint().magnitude));
//        return EStateType.Stick;
//    }

//    public EStateType GetStateType()
//    {
//        return EStateType.Stick;
//    }
//}


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
    private float _acceleration;
    private float _deceleration;
    private bool _turn_on_accelerate;
    private bool _stop_accelerating;
    private float _prev_frame_distance;

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

        _acceleration = 0.5f;
        _deceleration = -5f;
        _turn_on_accelerate = false;
        _stop_accelerating = false;

    }


    //void Update() { } 


    public void CameraHandler(float in_time)
    {
        if (_player == null)
            return;

        _desired_camera_point.SetCurrentPosition(DesiredCameraPos(), in_time);
        float acc = _turn_on_accelerate ? _acceleration : _deceleration;
        if (_stop_accelerating)
            acc = 0f;
        _prev_frame_distance = GetVectorBetweenCameraAndPoint().magnitude;
        UpdateCameraVelocityAndPosition(in_time, acc);

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
            //case EStateType.Stick: return new CCameraState_Stick(this);
        }

        return null;
    }

    public void UpdateCameraVelocityAndPosition(float in_time, float in_acceleration)
    {
        _velocity = Vector3.Project(_velocity, GetVectorBetweenCameraAndPoint().normalized);
        _cam_transform.position += _velocity * in_time + GetVectorBetweenCameraAndPoint().normalized * in_acceleration * in_time * in_time / 2;
        Vector3 temp = GetVectorBetweenCameraAndPoint().normalized * in_acceleration * in_time;
        _velocity = _velocity + temp;
        Vector3 relative_position = _player.position - _cam_transform.position;
        _cam_transform.rotation = Quaternion.LookRotation(relative_position);
    }

    public Vector3 DesiredCameraPos()
    {
        return _player.position + _player.rotation * _camera_shift;
    }

    public Vector3 GetVectorBetweenCameraAndPoint()
    {
        return DesiredCameraPos() - _cam_transform.position;
    }

    public Vector3 GetCameraVelocity()
    {
        return _velocity;
    }

    public Vector3 GetPointVelocity()
    {
        return _desired_camera_point.GetCurrentVelocity();
    }

    public float GetCameraDeceleration()
    {
        return _deceleration;
    }

    public float GetPrevFrameDistance()
    {
        return _prev_frame_distance;
    }

    public void SetCameraAcceleration(float in_acceleration)
    {
        _acceleration = in_acceleration;
    }

    public void TurnOnOffAcceleration(bool is_turn_on)
    {
        _turn_on_accelerate = is_turn_on;
    }

    public void SetCameraPositionToDesired()
    {
        _cam_transform.position = DesiredCameraPos();
        _velocity = Vector3.zero;
    }

    public void SetStopAccelerating(bool is_to_stop)
    {   
        _stop_accelerating = is_to_stop;
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
