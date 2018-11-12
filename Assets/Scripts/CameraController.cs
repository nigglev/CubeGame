using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Globalization;

public class CameraController : MonoBehaviour {

    private float _distance = 2f;
    private float _currentX = 0f;
    private float _currentY = 0f;
    private float _minangleY = 0f;
    private float _maxangleY = 50f;
    private float _damping_force_mag = 0f;
    private Vector3 _start_position;
    private Vector3 _camera_shift;
    private Vector3 _velocity;
    private List<DataToExcel> _excel_data;
    [SerializeField]
    private float _spring_constant = 5f;
    [SerializeField]
    private float _damping_constant = -1f;
    [SerializeField]
    private float _distance_to_start_damping = 5f;
    [SerializeField]
    private float _distance_with_max_damping = 1f;

    private enum ECameraState { Undefined, LookAtPlayer, LookAtEnemy };
    private ECameraState _state;

    private Transform _target;
    private Transform _cam_transform;

    private Camera _cam;
    CultureInfo customCulture;

    struct DataToExcel
    {
        float _distance_to_target_data;
        float _cam_velocity_data;
        float _spring_velocity_data;
        float _damping_velocity_data;

        public DataToExcel(float in_distance, float in_cam_vel, float in_spring_vel, float in_damping_vel)
        {
            _distance_to_target_data = in_distance;
            _cam_velocity_data = in_cam_vel;
            _spring_velocity_data = in_spring_vel;
            _damping_velocity_data = in_damping_vel;
        }

        public float GetDistance { get { return _distance_to_target_data; } }
        public float GetCamVel { get { return _cam_velocity_data; } }
        public float GetSpringVel { get { return _spring_velocity_data; } }
        public float GetDampingVel { get { return _damping_velocity_data; } }
    }

    private void Start()
    {
        CWorld.Instance.SetCamera(this);
        _cam = Camera.main;
        _cam_transform = transform;
        _state = ECameraState.Undefined;
        _start_position = _cam_transform.position;
        _camera_shift = new Vector3(0, 1f, -_distance);
        _velocity = Vector3.zero;
        _excel_data = new List<DataToExcel>();
        customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        CWorld.Instance.OnNewPlayer += OnNewPlayer;
        if (CWorld.Instance.GetPlayer() != null)
            OnNewPlayer(this, EventArgs.Empty);
    }

    private void Update()
    {
        if (_state == ECameraState.Undefined)
            return;

        if (Input.GetMouseButton(0))
        {
            _currentX += Input.GetAxis("Mouse X");
            _currentY += Input.GetAxis("Mouse Y");
            _currentY = Mathf.Clamp(_currentY, _minangleY, _maxangleY);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_state == ECameraState.LookAtPlayer)
            {
                _state = ECameraState.LookAtEnemy;
                _target = CWorld.Instance.GetNearestEnemy(CWorld.Instance.GetPlayerPosition());
            }
            else if (_state == ECameraState.LookAtEnemy)
            {
                _state = ECameraState.LookAtPlayer;
                _target = CWorld.Instance.GetPlayer();
            }
        }
    }

    //private void LateUpdate()
    //{
    //    if (_state == ECameraState.Undefined)
    //        return;

    //    if (_state == ECameraState.LookAtPlayer)
    //    {
    //        DampedSpring();
    //        Vector3 relative_position = _target.position - transform.position;
    //        _cam_transform.rotation = Quaternion.LookRotation(relative_position);
    //    }
    //    else if (_state == ECameraState.LookAtEnemy)
    //    {
    //        Vector3 dir = _target.position - CWorld.Instance.GetPlayerPosition();
    //        dir = dir.normalized;
    //        _cam_transform.position = (CWorld.Instance.GetPlayerPosition() - _distance * dir) + new Vector3(0, 0.5f, 0);
    //        Vector3 relative_position = _target.position - transform.position;
    //        _cam_transform.rotation = Quaternion.LookRotation(relative_position);
    //    }
    //}

    public void CameraUpdate()
    {
        if (_state == ECameraState.Undefined)
            return;

        if (_state == ECameraState.LookAtPlayer)
        {
            DampedSpring();
            Vector3 relative_position = _target.position - transform.position;
            _cam_transform.rotation = Quaternion.LookRotation(relative_position);
        }
        else if (_state == ECameraState.LookAtEnemy)
        {
            Vector3 dir = _target.position - CWorld.Instance.GetPlayerPosition();
            dir = dir.normalized;
            _cam_transform.position = (CWorld.Instance.GetPlayerPosition() - _distance * dir) + new Vector3(0, 0.5f, 0);
            Vector3 relative_position = _target.position - transform.position;
            _cam_transform.rotation = Quaternion.LookRotation(relative_position);
        }
    }

    private void DampedSpring()
    {   

        Vector3 current_to_target = GetDirectionToTarget();
        Vector3 spring_force = current_to_target * _spring_constant;
        Vector3 spring_velocity = spring_force * Time.deltaTime;

        float damping_constant_velocity_to_target = 2 * Mathf.Sqrt(_spring_constant) * GetDistanceCoef();
        float damping_constant_transverse_velocity = Mathf.Sqrt(_spring_constant) * 0.5f;

        Vector3 normal = current_to_target.normalized;
        Vector3 velocity_to_target = Vector3.Project(_velocity, normal);
        Vector3 transverse_velocity = _velocity - velocity_to_target;
        
        Vector3 damped_velocity_to_target = GetDampedVelocity(velocity_to_target, damping_constant_velocity_to_target);
        Vector3 damped_transverse_velocity = GetDampedVelocity(transverse_velocity, damping_constant_transverse_velocity);

        _velocity += spring_velocity + damped_velocity_to_target + damped_transverse_velocity;

        //DataToExcel new_data = new DataToExcel(current_to_target.magnitude, _velocity.magnitude, spring_velocity.magnitude, damped_velocity_to_target.magnitude);
        //_excel_data.Add(new_data);
        //WriteToExcel();

        _cam_transform.position = _cam_transform.position + _velocity * Time.deltaTime;


    }


    private Vector3 GetDampedVelocity(Vector3 in_velocity, float in_coef)
    {
        Vector3 damping_force = -in_velocity * in_coef;
        _damping_force_mag += damping_force.magnitude;
        Vector3 damping_velocity = damping_force * Time.deltaTime;

        if (damping_velocity.sqrMagnitude > in_velocity.sqrMagnitude)
            damping_velocity = in_velocity * (-1);

        return damping_velocity;
    }


    float GetDistanceCoef()
    {   
        Vector3 distance = GetDirectionToTarget();
        float f_distance = distance.magnitude;

        if (f_distance >= _distance_to_start_damping)
            return 0f;

        Debug.Log(String.Format("Coefficient = {0}, Distance To Target = {1}, Distance to Start Damping = {2}, Player Velocity = {3}, Camera Velocity = {4}", 
            (_distance_to_start_damping - f_distance) / _distance_to_start_damping, f_distance, _distance_to_start_damping, 
            CWorld.Instance.GetPlayerVelocity().magnitude, _velocity.magnitude));


        return (_distance_to_start_damping - f_distance) / _distance_to_start_damping;

    }

    private Vector3 GetDirectionToTarget()
    {
        return _target.position + _target.rotation * _camera_shift - _cam_transform.position;
    }

    private void OnNewPlayer(object sender, EventArgs e)
    {
        _state = ECameraState.LookAtPlayer;
        _target = CWorld.Instance.GetPlayer();
    }


    private void OnDrawGizmos()
    {
        if (_target == null)
            return;

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(_target.position + _target.rotation * _camera_shift, 0.1f);
        
    }


    private void WriteToExcel()
    {
        if (_excel_data.Count % 200 != 0)
            return;

        string path = Application.dataPath + "/StreamingAssets/GraphData" + _excel_data.Count.ToString() + ".csv";

        using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Distance,CameraVelocity,SpringVel,DampingVel");
            for(int i = 0; i < _excel_data.Count; i++)
            {
                str.AppendLine(string.Format("{0},{1},{2},{3}", _excel_data[i].GetDistance.ToString(customCulture), _excel_data[i].GetCamVel.ToString(customCulture),
                    _excel_data[i].GetSpringVel.ToString(customCulture), _excel_data[i].GetDampingVel.ToString(customCulture)));
            }
            
            Byte[] info = new UTF8Encoding(true).GetBytes(str.ToString());
            fs.Write(info, 0, info.Length);
        }

    }
}
