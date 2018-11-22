using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStateType {Idle, Accelerate, GetSlow}


public interface ICameraStateOwner
{
    Vector3 GetChasingVelocity();
    Vector3 GetPointVelocity();
    void SetCameraAcceleration(float in_acceleration);
    float GetTimeToCatch();
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
        Debug.Log(string.Format("IDLE; Camera Chasing Velocity = {0}, Point Velocity = {1}",
            _owner.GetChasingVelocity().magnitude, _owner.GetPointVelocity().magnitude));

        if (_owner.GetChasingVelocity().magnitude < _owner.GetPointVelocity().magnitude)
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
    float _acceleration;

    public CCameraState_Accelerate(ICameraStateOwner in_owner)
    {
        _owner = in_owner;
        _acceleration = 0.5f;
    }

    public EStateType Update(float in_time)
    {

        

        Debug.Log(string.Format("ACCELERATE; Camera Chasing Velocity = {0}, Point Velocity = {1}",
            _owner.GetChasingVelocity().magnitude, _owner.GetPointVelocity().magnitude));

        return EStateType.Accelerate;
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
    }

    public EStateType Update(float in_time)
    {
        Debug.Log(string.Format("GETSLOW; Camera Chasing Velocity = {0}, Point Velocity = {1}",
            _owner.GetChasingVelocity().magnitude, _owner.GetPointVelocity().magnitude));

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

    private Vector3 _cam_velocity;
    private float _acceleration;
    private float _time_to_catch;

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
        SetCameraProperties(_desired_camera_point.GetCurrentPosition(), 0f);
        _time_to_catch = 2f;



    }


    //void Update() { } 


    public void CameraHandler(float in_time)
    {
        if (_player == null)
            return;

        _desired_camera_point.SetCurrentPosition(DesiredCameraPos(), in_time);

        EStateType new_state = _current_state.Update(in_time);
        if (new_state != _current_state.GetStateType())
        {
            ICameraState old_state = _current_state;
            _current_state = CreateStateByType(new_state);
            _current_state.Update(0f);
        }

        
        //Debug.Log(string.Format("Player Velocity = {0}, Camera Velocity = {1}, Distance = {2}", CWorld.Instance.GetPlayerVelocity().magnitude,
           // _desired_camera_point.GetCurrentVelocity().magnitude, _desired_camera_point.GetDistanceBetweenPositions()));
        

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

    public Vector3 GetChasingVelocity()
    {
        //Vector3 chasing_velocity1 = Project(_cam_velocity, _desired_camera_point.GetCurrentVelocity().normalized);
        Vector3 chasing_velocity = Vector3.Project(_cam_velocity, _desired_camera_point.GetCurrentVelocity().normalized);
        return chasing_velocity;
    }

    public Vector3 GetPointVelocity()
    {
        return _desired_camera_point.GetCurrentVelocity();
    }

    public float GetTimeToCatch()
    {
        return _time_to_catch;
    }

    //Vector3 Project(Vector3 in_vector, Vector3 in_normal)
    //{
    //    return Vector3.Dot(in_vector, in_normal);
    //}

    public void SetCameraProperties(Vector3 in_new_pos, float in_time)
    {
        _cam_transform.position += _cam_velocity * in_time + GetDirectionToTarget().normalized * _acceleration * in_time * in_time / 2;
        _cam_velocity += GetDirectionToTarget().normalized * _acceleration * in_time;
        Vector3 relative_position = _player.position - _cam_transform.position;
        _cam_transform.rotation = Quaternion.LookRotation(relative_position);
    }

    public void SetCameraAcceleration(float in_acceleration)
    {
        _acceleration = in_acceleration;
    }

    public Vector3 DesiredCameraPos()
    {
        return _player.position + _player.rotation * _camera_shift;
    }

    public Vector3 GetDirectionToTarget()
    {
        return DesiredCameraPos() - _cam_transform.position;
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
