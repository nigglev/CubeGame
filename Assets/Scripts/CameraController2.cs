using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2 : MonoBehaviour {

    private Camera _cam;
    private Transform _cam_transform;
    private Transform _player;


    private float _distance = 10f;
    private float _shift = 0f;
    private float _acceleration = 0f;
    private float? _initial_dist;
    private float _max_velocity_magnitude = 0.005f;
    private Vector3 _distance_to_deaccelerate;
    private Vector3 _camera_shift;
    private Vector3 _velocity;
    private bool _accelerating_mode = true;
    



    // Use this for initialization
    void Start() {

        CWorld.Instance.SetCamera2(this);
        _cam = Camera.main;
        _cam_transform = transform;
        _camera_shift = new Vector3(0, 1f, -_distance);
        _velocity = Vector3.zero;
        _distance_to_deaccelerate = new Vector3(0, 1f, 10f);
        CWorld.Instance.OnNewPlayer += OnNewPlayer;
        if (CWorld.Instance.GetPlayer() != null)
            OnNewPlayer(this, EventArgs.Empty);

    }

    // Update is called once per frame
    void Update() {}

    public void CameraHandler()
    {
        //Vector3 prev_pos = _cam_transform.position;
        //_cam_transform.position += _velocity * Time.deltaTime + GetDirectionToTarget().normalized * _acceleration * Time.deltaTime * Time.deltaTime / 2;
        //_velocity += GetDirectionToTarget().normalized * _acceleration * Time.deltaTime;

        //_shift = _cam_transform.position.magnitude - prev_pos.magnitude;
        //_distance = _distance - _shift;
        //_acceleration = CalculateAcceleration(_distance, _velocity.magnitude, 3f);

        _acceleration = CalculateAcceleration(_distance, _velocity.magnitude, 5f);
        Vector3 prev_pos = _cam_transform.position;
        _cam_transform.position += _velocity * 3f + GetDirectionToTarget().normalized * _acceleration * 3f * 3f / 2;
        _shift = (_cam_transform.position - prev_pos).magnitude;
        _velocity += GetDirectionToTarget().normalized * _acceleration * 3f;

        _distance = _distance - _shift;

        _acceleration = CalculateAcceleration(_distance, _velocity.magnitude, 2f);
        prev_pos = _cam_transform.position;
        _cam_transform.position += _velocity * 2f + GetDirectionToTarget().normalized * _acceleration * 2f * 2f / 2;
        _shift = _cam_transform.position.magnitude - prev_pos.magnitude;
    }

    private List<float> BuildParabola(Tuple<float, float> in_first_point, Tuple<float, float> in_second_point, Tuple<float, float> in_third_point)
    {
        List<float> coefs = new List<float>();
        float x1 = in_first_point.Item1;
        float y1 = in_first_point.Item2;
        float x2 = in_second_point.Item1;
        float y2 = in_second_point.Item2;
        float x3 = in_third_point.Item1;
        float y3 = in_third_point.Item2;
        float t = (x3 * (y2 - y1) + x2 * y1 - x1 * y2) / (x2 - x1);
        float a_coef = (y3 - t) / (x3 * (x3 - x1 - x2) + x1 * x2);
        float b_coef = (y2 - y1) / (x2 - x1) - a_coef * (x1 + x2);
        float c_coef = (x2 * y1 - x1 * y2) / (x2 - x1) + a_coef * x1 * x2;
        coefs.Add(a_coef);
        coefs.Add(b_coef);
        coefs.Add(c_coef);
        return coefs;
    }

    private float CalculateAcceleration(float in_dist0, float in_vel0, float in_time)
    {
        float acceleration = 2 * (in_dist0 - in_vel0 * in_time) / (in_time * in_time);
        return acceleration;
    }

    private float CalculateDeacceletion(float in_distance)
    {
        return -(_velocity.magnitude * _velocity.magnitude) / (2 * in_distance);
    }

    private Vector3 GetDirectionToTarget()
    {
        return _player.position + _player.rotation * _camera_shift - _cam_transform.position;
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
