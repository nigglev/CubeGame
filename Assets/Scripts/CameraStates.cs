//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//public enum EStateType { Idle, Chase, Approaching }

//public interface ICameraStateOwner
//{   
//    Vector3 GetCameraVelocity();
//    Vector3 GetPlayerVelocity();
//    Vector3 GetDirectionToTarget();
//    float GetCameraAcceleration();
//    void SetCameraAcceleration(float in_acceleration);
//    void SetCameraVelocityToZero();
//    void SetCameraToDefaultPosition();
    
//}


//public interface ICameraState
//{
//    EStateType Update(float in_time);
//    EStateType GetStateType();
//    Vector3 GetLastVelocity();
//    Vector3 GetLastPosition();
//}

//public class CCameraState_Idle : ICameraState
//{
//    ICameraStateOwner _owner;
//    ICameraState _old_state;
//    Vector3 _last_pos;
//    Vector3 _last_velocity;

//    public CCameraState_Idle(ICameraStateOwner in_owner, ICameraState in_old_state)
//    {
//        _owner = in_owner;
//        _old_state = in_old_state;
        
//    }

//    public EStateType Update(float in_time)
//    {

//        if (_owner.GetCameraVelocity().magnitude + 2f < _owner.GetPlayerVelocity().magnitude)
//            return EStateType.Chase;
//        if (_owner.GetCameraVelocity().magnitude >= _owner.GetPlayerVelocity().magnitude)
//            return EStateType.Approaching;

//        //Debug.Log(string.Format("IDLE; Player Velocity = {0}, Camera Velocity = {1}, Acceleration = {2}, Distance To Target = {3}",
//        //    _owner.GetPlayerVelocity().magnitude, _owner.GetCameraVelocity().magnitude, _owner.GetCameraAcceleration(), _owner.GetDirectionToTarget().magnitude));
//        return EStateType.Idle;
//    }

//    public Vector3 GetLastVelocity()
//    {
//        return _last_velocity;
//    }

//    public Vector3 GetLastPosition()
//    {
//        return _last_pos;
//    }

//    public EStateType GetStateType()
//    {
//        return EStateType.Idle;
//    }
//}


//public class CCameraState_Chase : ICameraState
//{
//    ICameraStateOwner _owner;
//    ICameraState _old_state;
//    Vector3 _last_pos;
//    Vector3 _last_velocity;
//    float acceleration = 0f;

//    public CCameraState_Chase(ICameraStateOwner in_owner, ICameraState in_old_state)
//    {
//        _owner = in_owner;
//        _old_state = in_old_state;
//    }

//    public EStateType Update(float in_time)
//    {
     
        


//        //Debug.Log(string.Format("CHASE; Player Velocity = {0}, Camera Velocity = {1}, Acceleration = {2}, Distance To Target = {3}", 
//        //    _owner.GetPlayerVelocity().magnitude, _owner.GetCameraVelocity().magnitude, _owner.GetCameraAcceleration(), _owner.GetDirectionToTarget().magnitude));


//        return EStateType.Chase;
//    }

//    public Vector3 GetLastVelocity()
//    {
//        return _last_velocity;
//    }

//    public Vector3 GetLastPosition()
//    {
//        return _last_pos;
//    }

//    public EStateType GetStateType()
//    {
//        return EStateType.Chase;
//    }
//}



//public class CCameraState_Approaching : ICameraState
//{
//    ICameraStateOwner _owner;
//    ICameraState _old_state;
//    CApproacher _approacher;

//    float _time_to_approach;

//    Vector3 _last_velocity;
//    Vector3 _last_pos;

//    public CCameraState_Approaching(ICameraStateOwner in_owner, ICameraState in_old_state)
//    {
//        _owner = in_owner;
//        _old_state = in_old_state;
//        _time_to_approach = 5f;
//        _approacher = new CApproacher(in_old_state.GetLastPosition(), _time_to_approach);
//    }

//    public EStateType Update(float in_time)
//    {
//        //_approacher.Update(in_time, )
//        return EStateType.Approaching;
//    }


//    public Vector3 GetLastVelocity()
//    {
//        return _last_velocity;
//    }

//    public Vector3 GetLastPosition()
//    {
//        return _last_pos;
//    }

//    public float CalculateAcceleration(float in_dist0, float in_vel0, float in_time)
//    {
//        float acceleration = 2 * (in_dist0 - in_vel0 * in_time) / (in_time * in_time);
//        return acceleration;
//    }

//    public float CalculateDeacceleration(float in_initial_velocity, float in_time)
//    {
//        return -in_initial_velocity / in_time;
//    }

//    public EStateType GetStateType()
//    {
//        return EStateType.Approaching;
//    }

    
//}
