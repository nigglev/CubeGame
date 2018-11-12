//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class CameraController3 : MonoBehaviour, ICameraStateOwner
//{   

//    private Camera _cam;
//    private Transform _cam_transform;
//    private Transform _player;

//    private float _distance = 2f;

//    private Vector3 _camera_shift;
//    private Vector3 _velocity;
//    private float _acceleration;

//    private ICameraState _current_state;
//    // Use this for initialization
//    void Start ()
//    {
//        CWorld.Instance.SetCamera3(this);
//        _cam = Camera.main;
//        _cam_transform = transform;
//        _camera_shift = new Vector3(0, 1f, -_distance);
//        _current_state = new CCameraState_Idle(this, null);
//        _velocity = Vector3.zero;
//        _acceleration = 0f;
        

//        CWorld.Instance.OnNewPlayer += OnNewPlayer;
//        if (CWorld.Instance.GetPlayer() != null)
//            OnNewPlayer(this, EventArgs.Empty);
//    }

//    // Update is called once per frame
//    void Update() { }

//    public void CameraHandler(float in_time)
//    {
//        SetCameraProperties(in_time);

//        EStateType new_state = _current_state.Update(in_time);
//        if (new_state != _current_state.GetStateType())
//        {
//            ICameraState old_state = _current_state;
//            _current_state = CreateStateByType(new_state, old_state);
//            _current_state.Update(0f);
//        }
//    }

//    public void SetCameraProperties(float in_time)
//    {
//        _cam_transform.position += _velocity * in_time + GetDirectionToTarget().normalized * _acceleration * in_time * in_time / 2;
//        _velocity += GetDirectionToTarget().normalized * _acceleration * in_time;
//        Vector3 relative_position = _player.position - _cam_transform.position;
//        _cam_transform.rotation = Quaternion.LookRotation(relative_position);
//    }

//    private ICameraState CreateStateByType(EStateType in_state, ICameraState in_old_state)
//    {
//        switch (in_state)
//        {
//            case EStateType.Idle: return new CCameraState_Idle(this, in_old_state);
//            case EStateType.Chase: return new CCameraState_Chase(this, in_old_state);
//            case EStateType.Approaching: return new CCameraState_Approaching(this, in_old_state);
//        }

//        return null;
//    }

//    Vector3 ICameraStateOwner.GetPlayerVelocity()
//    {
//        return CWorld.Instance.GetPlayerVelocity();
//    }

//    Vector3 ICameraStateOwner.GetCameraVelocity()
//    {
//        return _velocity;
//    }

//    float ICameraStateOwner.GetCameraAcceleration()
//    {
//        return _acceleration;
//    }

//    void ICameraStateOwner.SetCameraAcceleration(float in_acceleration)
//    {
//        _acceleration = in_acceleration;
//    }

//    public Vector3 GetDirectionToTarget()
//    {
//        return _player.position + _player.rotation * _camera_shift - _cam_transform.position;
//    }

//    void ICameraStateOwner.SetCameraVelocityToZero()
//    {
//        _velocity = Vector3.zero;
//    }

//    void ICameraStateOwner.SetCameraToDefaultPosition()
//    {
//        _cam_transform.position = _player.position + _player.rotation * _camera_shift;
//    }

//    private void OnNewPlayer(object sender, EventArgs e)
//    {
//        _player = CWorld.Instance.GetPlayer();
//    }

//    private void OnDrawGizmos()
//    {
//        if (_player == null)
//            return;

//        Gizmos.color = Color.black;
//        Gizmos.DrawSphere(_player.position + _player.rotation * _camera_shift, 0.1f);

//    }
//}

