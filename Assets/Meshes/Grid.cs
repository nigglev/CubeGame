using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{

    public int _x_size, _y_size;
    private Vector3[] _vertices;
    private Mesh _mesh;


    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Grid";

        _vertices = new Vector3[(_x_size + 1) * (_y_size + 1)];

        for (int i = 0, y = 0; y <= _y_size; y++)
        {
            for (int x = 0; x <= _x_size; x++, i++)
            {
                _vertices[i] = new Vector3(x, y);
            }
        }

        _mesh.vertices = _vertices;

        int[] triangles = new int[_x_size * _y_size * 6];

        for (int ti = 0, vi = 0, y = 0; y < _y_size; y++, vi++)
        {
            for (int x = 0; x < _x_size; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + _x_size + 1;
                triangles[ti + 5] = vi + _x_size + 2;
            }
        }

        
        _mesh.triangles = triangles;

    }

    //private void OnDrawGizmos()
    //{
    //    if (_vertices == null)
    //        return;

    //    Gizmos.color = Color.black;
    //    for (int i = 0; i < _vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(_vertices[i], 0.1f);
    //    }
    //}
}