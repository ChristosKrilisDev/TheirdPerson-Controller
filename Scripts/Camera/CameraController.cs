using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform _target;
    [SerializeField] private Transform _pivot;
    //Transform _parentTarget;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private bool _useOffset;
    [SerializeField] bool _invertYMouse = false;
    [Range(-1,1)]int _invertMouseValue = 1;

    [SerializeField][Range(0f,1f)] private float _lerpDelay;

    [SerializeField] private float _cRotateSpeed;

    [Header("View Angles Vars")]
    [SerializeField] float _maxViewAngle = 45f;
    [SerializeField] float _minViewAgnle = -45f;



    void Start()
    {
        //_offset = _target.position - transform.position;
        //_parentTarget = _target.parent;
        if(_invertYMouse)
            _invertMouseValue = -1;

        if(!_useOffset)
            _offset = _target.position - transform.position;

        _pivot.transform.position = _target.transform.position;
        _pivot.transform.parent = _target.transform;
    }


    void LateUpdate()
    {
        if(_target == null)
            return;

        //Lock mouse , Hide mouse
        MouseCursorActivity();

        //Get the x position of the mouse & rotate the target 
        float horizontal = Input.GetAxis("Mouse X") * _cRotateSpeed;
        _target.Rotate(0 , horizontal , 0);

        //get the y position of the mouse & rotate the @pivot
        float vertical = Input.GetAxis("Mouse Y") * _cRotateSpeed;
        //transform.Rotate(_invertMouseValue * vertical , 0 , 0);
        _pivot.Rotate(vertical , 0 , 0);

        //Invert Y
        //if(_invertYMouse)
        //    _pivot.Rotate(vertical , 0 , 0);
        //else
        //    _pivot.Rotate(-vertical , 0 , 0);

        //Limit the camera up/down rotation
        if(_pivot.rotation.eulerAngles.x > _maxViewAngle && _pivot.rotation.eulerAngles.x < 180f)
            _pivot.rotation = Quaternion.Euler(_maxViewAngle , 0 , 0);
        if(_pivot.rotation.eulerAngles.x > 180 && _pivot.rotation.eulerAngles.x < 360f + _minViewAgnle)
            _pivot.rotation = Quaternion.Euler(360f + _minViewAgnle , 0 , 0);


        //move the camera based on the current rotation and the original offset
        float desiredYangle = _target.eulerAngles.y;
        //float desiredXangle = transform.eulerAngles.x;
        float desiredXangle = _pivot.eulerAngles.x;
        Quaternion rotation = Quaternion.Euler(desiredXangle , desiredYangle , 0);



        //Move/rotate camera here
        transform.position = _target.position - (rotation * _offset);

        if(transform.position.y < _target.position.y)
            transform.position = new Vector3(transform.position.x , _target.transform.position.y - 0.5f , transform.position.z);


        transform.LookAt(_target);
    }

    private void FollowTarget()
    {
        //transform.position = _target.position + _offset;
        //transform.position = Vector3.Lerp(transform.position , _target.transform.position + _offset , _lerpDelay * Time.deltaTime);
        
    }


    private bool isMouseUnlokced = false;
    #region Test
    private void MouseCursorActivity()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            isMouseUnlokced = !isMouseUnlokced;

            Cursor.visible = isMouseUnlokced;

            if(!isMouseUnlokced)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }
    }
    #endregion


}
