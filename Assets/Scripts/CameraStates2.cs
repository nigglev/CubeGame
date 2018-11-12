using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EStateType { Idle, Chase, Approaching, Sticked }

public interface ICameraStateOwner
{
    Vector3 GetPlayerVelocity();
    float GetPlayerMaxVelocity();
    void SetNewCameraPos(Vector3 in_new_pos);
    Vector3 GetDirectionToTarget();
    Vector3 GetCameraCurrentPosition();
    Vector3 GetPlayerCurrentPosition();
}

public interface ICameraState
{
    EStateType Update(float in_time);
    EStateType GetStateType();
    Vector3 GetLastDistance();
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

        if(_owner.GetPlayerVelocity().magnitude >= 2f)
        {
            Debug.Log("GO TO CHASE");
            return EStateType.Chase;
        }

        Debug.Log("CURRENT STATE = IDLE");
        return EStateType.Idle;
    }

    public EStateType GetStateType()
    {
        return EStateType.Idle;
    }

    public Vector3 GetLastDistance()
    {
        return Vector3.zero;
    }
}

public class CCameraState_Chase : ICameraState
{
    ICameraStateOwner _owner;
    float _acceleration;
    float _time_to_accelerate_to_max_player_velocity;
    float _velocity;
    Vector3 _last_distance;

    public CCameraState_Chase(ICameraStateOwner in_owner)
    {
        _owner = in_owner;
        _time_to_accelerate_to_max_player_velocity = 6f;
        _velocity = 0f;
        _acceleration = _owner.GetPlayerMaxVelocity() / _time_to_accelerate_to_max_player_velocity;
    }

    public EStateType Update(float in_time)
    {
        float shift = _velocity * in_time + _acceleration * in_time * in_time / 2;
        _velocity += _acceleration * in_time;
        if (_velocity >= _owner.GetPlayerVelocity().magnitude)
        {
            Debug.Log("GO TO APPROACHING");
            _last_distance = _owner.GetDirectionToTarget();
            return EStateType.Approaching;
        }   
        else
        {   
            Vector3 new_pos = shift * _owner.GetDirectionToTarget().normalized + _owner.GetCameraCurrentPosition();
            _owner.SetNewCameraPos(new_pos);
        }
        Debug.Log(string.Format("CURRENT STATE = CHASE; Acceleration = {0}, Velocity = {1}, Player Velocity = {2}", _acceleration, _velocity, _owner.GetPlayerVelocity().magnitude));
        return EStateType.Chase;
    }

    public Vector3 GetLastDistance()
    {
        return _last_distance;
    }

    public EStateType GetStateType()
    {
        return EStateType.Chase;
    }
}

public class CCameraState_Approaching : ICameraState
{
    ICameraStateOwner _owner;
    CApproacher _approacher;
    Vector3 _distance_between_cam_player;

    public CCameraState_Approaching(ICameraStateOwner in_owner, ICameraState prev_state)
    {
        _owner = in_owner;
        float _time_to_approach = 2f;
        _distance_between_cam_player = prev_state.GetLastDistance();
        _approacher = new CApproacher(_distance_between_cam_player, _time_to_approach);
        
    }

    public EStateType Update(float in_time)
    {
        _owner.SetNewCameraPos(_owner.GetPlayerCurrentPosition() - _distance_between_cam_player);
        return EStateType.Approaching;
    }

    public EStateType GetStateType()
    {
        return EStateType.Approaching;
    }

    public Vector3 GetLastDistance()
    {
        return Vector3.zero;
    }
}

public class CCameraState_Sticked : ICameraState
{
    ICameraStateOwner _owner;

    public CCameraState_Sticked(ICameraStateOwner in_owner, ICameraState prev_state)
    {
        _owner = in_owner;
    }

    public EStateType Update(float in_time)
    {
        return EStateType.Sticked;
    }

    public EStateType GetStateType()
    {
        return EStateType.Sticked;
    }

    public Vector3 GetLastDistance()
    {
        return Vector3.zero;
    }
}

