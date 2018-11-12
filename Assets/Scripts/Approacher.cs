using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

class CApproacher
{
    float _time_to_approach;
    float _current_time;
    float _velocity;
    Vector3 _velocity_vector;
    float _max_velocity = 10f;
    float _acceleration;
    Vector3 _initial_distance;
    float _time_to_accelerate;
    float shift = 0f;
    float? _initial_velocity_to_deaccelerate;
    float? _initial_distance_to_deaccelerate;

    private List<DataToExcel> _excel_data;
    CultureInfo customCulture;

    public CApproacher(Vector3 in_inital_distance, float in_time_to_approach)
    {
        _time_to_approach = in_time_to_approach;
        _initial_distance = in_inital_distance;
        _velocity = 0f;
        _velocity_vector = Vector3.zero;

        _excel_data = new List<DataToExcel>();
        customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
    }

    struct DataToExcel
    {
        float _velocity_data;
        float _time_data;
        

        public DataToExcel(float in_velocity, float in_time)
        {
            _velocity_data = in_velocity;
            _time_data = in_time;
        }

        public float GetVelocity { get { return _velocity_data; } }
        public float GetTime { get { return _time_data; } }
    }


    //public Vector3 Update(Vector3 in_to_target, float in_time)
    //{
    //    float prev_vel = _velocity;
    //    shift = _velocity * in_time + _acceleration * in_time * in_time / 2;
    //    _velocity += _acceleration * in_time;
    //    _current_time += in_time;
    //    float time_left = _time_to_approach / 2 - _current_time;

    //    //if (in_to_target.magnitude <= 0.01f)
    //    //    return Vector3.negativeInfinity;

    //    if (time_left >= 0.00001f && !Mathf.Approximately(time_left, 0f))
    //    {
    //        _acceleration = CalculateAcceleration((in_to_target - _initial_distance / 2).magnitude, _velocity, time_left);
    //        Debug.Log(string.Format("ACCELERATION Time Left = {0}; Velocity = {1}, Acceleration = {2}, Shift = {3}, Distance to Target = {4}",
    //        time_left, _velocity, _acceleration, shift, (in_to_target - _initial_distance / 2).magnitude));
    //    }

    //    else
    //    {
    //        if (!_initial_velocity_to_deaccelerate.HasValue)
    //            _initial_velocity_to_deaccelerate = _velocity;
    //        time_left = _time_to_approach / 2 + time_left;
    //        _acceleration = CalculateDeaccelerationDistance(_initial_velocity_to_deaccelerate.Value, _time_to_approach / 2);
    //        Debug.Log(string.Format("DEACCELERATION Time Left = {0}; Velocity = {1} [{5}], Acceleration = {2}, Shift = {3}, Distance to Target = {4}",
    //        time_left, _velocity, _acceleration, shift, in_to_target.magnitude, prev_vel));
    //    }



    //    return shift * in_to_target.normalized;
    //}

    public Vector3 Update(float in_time, Vector3 cur_pos, Vector3 end_pos)
    {
        _current_time += in_time;
        float time_left = _time_to_approach / 2 - _current_time;

        //return WeightedAverage(cur_pos, end_pos, 3f);
        float new_pos_x = SmoothMovement(in_time, cur_pos.x, end_pos.x, ref _velocity_vector.x, time_left, _max_velocity);
        float new_pos_y = SmoothMovement(in_time, cur_pos.y, end_pos.y, ref _velocity_vector.y, time_left, _max_velocity);
        float new_pos_z = SmoothMovement(in_time, cur_pos.z, end_pos.z, ref _velocity_vector.z, time_left, _max_velocity);

        _velocity_vector = new Vector3(_velocity_vector.x, _velocity_vector.y, _velocity_vector.z);
        Vector3 new_pos = new Vector3(new_pos_x, new_pos_y, new_pos_z);

        Debug.Log(string.Format("Time Left = {0}; Velocity = {1}, Distance to Target = {2}",
        time_left, _velocity_vector.magnitude, (end_pos - cur_pos).magnitude));

        if(time_left >= 0)
        {
            DataToExcel new_data = new DataToExcel(_velocity_vector.magnitude, _current_time);
            _excel_data.Add(new_data);
            WriteToExcel();
        }

        return new_pos;
    }

    public Vector3 Update(float in_time, Vector3 cur_velocity, Vector3 cur_pos, Vector3 end_pos)
    {
        _current_time += in_time;
        float time_left = _time_to_approach / 2 - _current_time;

        //return WeightedAverage(cur_pos, end_pos, 3f);
        float new_pos_x = SmoothMovement(in_time, cur_pos.x, end_pos.x, ref _velocity_vector.x, time_left, _max_velocity);
        float new_pos_y = SmoothMovement(in_time, cur_pos.y, end_pos.y, ref _velocity_vector.y, time_left, _max_velocity);
        float new_pos_z = SmoothMovement(in_time, cur_pos.z, end_pos.z, ref _velocity_vector.z, time_left, _max_velocity);

        _velocity_vector = new Vector3(_velocity_vector.x, _velocity_vector.y, _velocity_vector.z);
        Vector3 new_pos = new Vector3(new_pos_x, new_pos_y, new_pos_z);
        

        return new_pos;
    }


    public float SmoothMovement(float in_time, float cur_coordinate, float end_coordinate, ref float cur_velocity, float smooth_time, float max_speed)
    {
        smooth_time = Mathf.Max(0.000001f, smooth_time);

        float distance = cur_coordinate - end_coordinate;
        float max_distance = smooth_time * max_speed;
        distance = Mathf.Clamp(distance, -max_distance, max_distance);
        float prev_end = end_coordinate;
        end_coordinate = cur_coordinate - distance;

        float coef1 = 3f / smooth_time;
        float coef2 = coef1 * in_time;
        //float coef3 = 1 / (1f + coef2 + 0.48f * coef2 * coef2 + 0.235f * coef2 * coef2 * coef2);
        float coef3 = -coef2 + 1;
        

        float coef4 = (cur_velocity + coef1 * distance) * in_time;
        float temp = cur_velocity;
        cur_velocity = (temp - coef1 * coef4) * coef3;
        float new_pos = end_coordinate + (distance + coef4) * coef3;
        if(prev_end - cur_coordinate > 0f == new_pos > prev_end)
        {
            new_pos = prev_end;
            cur_velocity = (new_pos - prev_end) / in_time;
        }

        return new_pos;
    }

    private void WriteToExcel()
    {
        string path = Application.dataPath + "/StreamingAssets/VelocityData.csv";

        using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Velocity");
            for (int i = 0; i < _excel_data.Count; i++)
            {
                str.AppendLine(string.Format("{0}", _excel_data[i].GetVelocity.ToString(customCulture)));
            }

            Byte[] info = new UTF8Encoding(true).GetBytes(str.ToString());
            fs.Write(info, 0, info.Length);
        }

        path = Application.dataPath + "/StreamingAssets/TimeData.csv";

        using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Time");
            for (int i = 0; i < _excel_data.Count; i++)
            {
                str.AppendLine(string.Format("{0}",_excel_data[i].GetTime.ToString(customCulture)));
            }

            Byte[] info = new UTF8Encoding(true).GetBytes(str.ToString());
            fs.Write(info, 0, info.Length);
        }

    }

    public Vector3 WeightedAverage(Vector3 current_pos, Vector3 end, float slowdown_factor)
    {
        float new_pos_x = ((current_pos.x * (slowdown_factor - 1)) + end.x) / slowdown_factor;
        float new_pos_y = ((current_pos.y * (slowdown_factor - 1)) + end.y) / slowdown_factor;
        float new_pos_z = ((current_pos.z * (slowdown_factor - 1)) + end.z) / slowdown_factor;

        return new Vector3(new_pos_x, new_pos_y, new_pos_z);
    }

    public float CalculateTime(float in_vel, float in_acc)
    {
        return -in_vel / in_acc;
    }

    public float CalculateAcceleration(float in_dist0, float in_vel0, float in_time)
    {
        float acceleration = 2 * (in_dist0 - in_vel0 * in_time) / (in_time * in_time);
        return acceleration;
    }

    public float CalculateDeaccelerationDistance(float in_vel, float in_dist)
    {
        return -(in_vel * in_vel) / (2 * in_dist);
    }

    public float CalculateDeaccelerationTime(float in_vel, float in_time)
    {
        return -in_vel / in_time;
    }

    public float CalculateDeacceleration(float in_dist, float in_time)
    {
        float acceleration = (-2 * in_dist) / (in_time * in_time);
        return acceleration;
    }

    public float CalculateDeaccelerationBatya(float in_dist, float in_time, float in_vel)
    {
        float acceleration = 2 * (in_vel * in_time - in_dist) / (in_time * in_time);
        return acceleration;
    }

}

