using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float _walk_speed = 8f;
    [SerializeField]
    private float _rotation_speed = 1f;
    [SerializeField]
    private float _jump_speed = 15f;
    [SerializeField]
    private float _drag_coef = 1f;
    [SerializeField]
    private float _max_player_velocity = 20f;
    bool _is_pressed_jump = false;
    bool _is_grounded = false;
    Rigidbody _rb;
    BoxCollider _collider;
    Transform _tr;
    public float _InputVertical;
    public float _InputHorizontal;
    public float _InputJump;


    // Use this for initialization
    void Start()
    {   
        SRVector pos = new SRVector(transform);
        CWorld.Instance.SetPlayerStartPosition(pos);
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        CWorld.Instance.HUD.Refresh();
        _tr = transform;
        CWorld.Instance.SetPlayer(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_tr.position.y < -3)
            CWorld.Instance.Restart();

        _InputVertical = Input.GetAxis("Vertical");
        _InputHorizontal = Input.GetAxis("Horizontal");
        _InputJump = Input.GetAxis("Jump");
    }

    private void FixedUpdate()
    {
        WalkHandler();
        RotationHandler();
        JumpingHandler();
        DragHandler(_drag_coef);
        _is_grounded = CheckGrounded();
        //CWorld.Instance.GetCamera2().CameraHandler(Time.fixedDeltaTime);
        //CWorld.Instance.GetCamera3().CameraHandler(Time.fixedDeltaTime);
        CWorld.Instance.GetCamera4().CameraHandler(Time.fixedDeltaTime);
    }

    public Vector3 GetVelocity()
    {
        return _rb.velocity;
    }

    public float GetMaxVelocity()
    {
        return _max_player_velocity;
    }

    internal Vector3 GetPosition()
    {
        return _tr.position;
    }

    void WalkHandler()
    {
        if (_rb.velocity.magnitude >= _max_player_velocity)
            _rb.AddForce(Vector3.zero);
        else
            _rb.AddForce(_tr.forward * _InputVertical * _walk_speed);
    }

    void RotationHandler()
    {       
        _rb.AddTorque(_tr.up * _InputHorizontal * _rotation_speed);
    }

    void JumpingHandler()
    {
        if (_InputJump > 0f)
        {
            if (!_is_pressed_jump && _is_grounded)
            {
                _rb.AddForce(_tr.up * _InputJump * _jump_speed, ForceMode.Impulse);
                _is_pressed_jump = true;
            }
        }
        else
            _is_pressed_jump = false;
    }

    void DragHandler(float in_drag_coef)
    {
        Vector3 velocity_vector = _rb.velocity;
        Vector3 forward_vector = _tr.forward;

        Vector3 projectX = Vector3.Project(velocity_vector, _tr.right);
        float projectX_length = projectX.magnitude;
        if (Mathf.Approximately(projectX_length, 0))
            return;
        Vector3 norm_projectX = projectX / projectX_length;
        float drag_impulse = _rb.mass * projectX_length;
        _rb.AddForce(-norm_projectX * drag_impulse * in_drag_coef, ForceMode.Impulse);
    }


    bool CheckGrounded()
    {   
        Vector3 BoxCenter = _collider.center;
        Vector3 HalfSize = _collider.size / 2;
        Vector3 Scale = _tr.lossyScale;
        Vector3 ScaledCenter = Vector3.Scale(BoxCenter, Scale);
        Vector3 ScaledHalfSize = Vector3.Scale(HalfSize, Scale);

        Vector3 LeftUpCorner = new Vector3(ScaledCenter.x - ScaledHalfSize.x, ScaledCenter.y - ScaledHalfSize.y + 0.01f, ScaledCenter.z + ScaledHalfSize.z);
        Vector3 LeftDownCorner = new Vector3(ScaledCenter.x + ScaledHalfSize.x, ScaledCenter.y - ScaledHalfSize.y + 0.01f, ScaledCenter.z + ScaledHalfSize.z);
        Vector3 RightUpCorner = new Vector3(ScaledCenter.x - ScaledHalfSize.x, ScaledCenter.y - ScaledHalfSize.y + 0.01f, ScaledCenter.z - ScaledHalfSize.z);
        Vector3 RightDownCorner = new Vector3(ScaledCenter.x + ScaledHalfSize.x, ScaledCenter.y - ScaledHalfSize.y + 0.01f, ScaledCenter.z - ScaledHalfSize.z);

        Vector3 FirstCorner = LocalToWorld(_tr, LeftUpCorner);
        Vector3 SecondCorner = LocalToWorld(_tr, LeftDownCorner);
        Vector3 ThirdCorner = LocalToWorld(_tr, RightUpCorner);
        Vector3 FourthCorner = LocalToWorld(_tr, RightDownCorner);

        bool grounded1 = Physics.Raycast(FirstCorner, new Vector3(0, -1, 0), 0.11f);
        bool grounded2 = Physics.Raycast(SecondCorner, new Vector3(0, -1, 0), 0.11f);
        bool grounded3 = Physics.Raycast(ThirdCorner, new Vector3(0, -1, 0), 0.11f);
        bool grounded4 = Physics.Raycast(FourthCorner, new Vector3(0, -1, 0), 0.11f);
        

        return (grounded1 && grounded2 && grounded3 && grounded4);
    }

    Vector3 LocalToWorld(Transform in_trans, Vector3 in_pos)
    {
        return in_trans.position + in_trans.rotation * in_pos;
    }

    private void OnDrawGizmos()
    {
        if (_rb == null || _tr == null)
            return;
        Vector3 velocity_vector = _rb.velocity;
        Vector3 forward_vector = _tr.forward;

        Vector3 projectX = Vector3.Project(velocity_vector, _tr.right);
        Vector3 norm_projectX = projectX.normalized;

        Gizmos.DrawLine(_tr.position, _tr.position + velocity_vector);
        Gizmos.DrawLine(_tr.position, _tr.position + forward_vector);
        Gizmos.DrawLine(_tr.position, _tr.position + projectX);
    }

    public Transform GetTransform()
    {
        return _tr;
    }

}
