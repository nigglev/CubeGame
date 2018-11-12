using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct SRVector
{
    Vector3 _position;
    Quaternion _rotation;

    public Vector3 position { get { return _position; } }
    public Quaternion rotation { get { return _rotation; } }


    public SRVector(Vector3 in_position, Quaternion in_rotation)
    {
        _position = in_position;
        _rotation = in_rotation;
    }

    public SRVector(Transform in_transform, bool isLocal = false)
    {
        if(isLocal)
        {
            _position = in_transform.localPosition;
            _rotation = in_transform.localRotation;
        }
        else
        {
            _position = in_transform.position;
            _rotation = in_transform.rotation;
        }
        
    }


}

