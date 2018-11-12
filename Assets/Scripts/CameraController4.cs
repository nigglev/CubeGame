using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController4 : MonoBehaviour, ICameraStateOwner
{


    private Camera _cam;
    private Transform _cam_transform;
    private Transform _player;
    private CApproacher _approacher;
    private float _distance = 2f;
    private Vector3 _camera_shift;
    private ICameraState _current_state;
    private Vector3 _position;

    void Start ()
    {

        CWorld.Instance.SetCamera4(this);
        _cam = Camera.main;
        _cam_transform = transform;
        _camera_shift = new Vector3(0, 1f, -_distance);
        CWorld.Instance.OnNewPlayer += OnNewPlayer;
        if (CWorld.Instance.GetPlayer() != null)
            OnNewPlayer(this, EventArgs.Empty);

        _approacher = new CApproacher(GetDirectionToTarget(), 5f);
        _current_state = new CCameraState_Idle(this);
        _position = DesiredCameraPos();

    }


    //void Update() { }


    private ICameraState CreateStateByType(EStateType in_state, ICameraState in_prev_state)
    {
        switch (in_state)
        {
            case EStateType.Idle: return new CCameraState_Idle(this);
            case EStateType.Chase: return new CCameraState_Chase(this);
            case EStateType.Approaching: return new CCameraState_Approaching(this, in_prev_state);
            case EStateType.Sticked: return new CCameraState_Sticked(this, in_prev_state);
        }

        return null;
    }


    public void CameraHandler(float in_time)
    {
        if (_player == null)
            return;

        //Vector3 shift = _approacher.Update(GetDirectionToTarget(), in_time);
        //_cam_transform.position += shift;

        //Vector3 new_pos = _approacher.Update(in_time, _cam_transform.position, DesiredCameraPos());
        SetCameraProperties(_position);


        EStateType new_state = _current_state.Update(in_time);
        if (new_state != _current_state.GetStateType())
        {
            ICameraState prev_state = _current_state;
            _current_state = CreateStateByType(new_state, prev_state);
            _current_state.Update(0f);
        }

    }

    public void SetCameraProperties(Vector3 in_new_pos)
    {
        _cam_transform.position = in_new_pos;
        Vector3 relative_position = _player.position - _cam_transform.position;
        _cam_transform.rotation = Quaternion.LookRotation(relative_position);
    }

    public Vector3 GetPlayerVelocity()
    {
        return CWorld.Instance.GetPlayerVelocity();
    }

    public float GetPlayerMaxVelocity()
    {
        return CWorld.Instance.GetPlayerMaxVelocity();
    }

    public Vector3 GetDirectionToTarget()
    {
        return _player.position + _player.rotation * _camera_shift - _cam_transform.position;
    }

    public Vector3 DesiredCameraPos()
    {
        return _player.position + _player.rotation * _camera_shift;
    }

    public void SetNewCameraPos(Vector3 in_new_pos)
    {
        _position = in_new_pos;
    }

    public Vector3 GetCameraCurrentPosition()
    {
        return _cam_transform.position;
    }

    public Vector3 GetPlayerCurrentPosition()
    {
        return CWorld.Instance.GetPlayerPosition();
    }

    private void OnNewPlayer(object sender, EventArgs e)
    {
        _player = CWorld.Instance.GetPlayer();
    }
}
