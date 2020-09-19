using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// About Movement Controller
/// Controll player movement from this script
/// Controll Physics
/// 
/// RULES:
/// While P(w) or P(s) => turnLimits := -45 , 45 , LootAt : Forward || Backward
/// So the player is moving in a |45| (max) nDegree but he is looking Forward || Backward
/// While Moving : Camera.Rotate.Player
/// 
/// </summary>
/// 


[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    //Movement
    [Header("Movment Members")]
    [SerializeField] private float _movementSpeed = 5;
    private Vector3 _moveDirection;
    [SerializeField]private Animator _playerAnimator;

    [Space]
    [Header("Slide Members")]
    [SerializeField] private float slideSpeed = 20f; // slide speed
    private bool  isSliding  = false;
    private Vector3  slideForward ; // direction of slide
    private float  slideTimer = 0.0f;
    [SerializeField]private float slideTimerMax = 2.5f; // time while sliding

    //Gravity
    [Space]
    [Header("Physics")]
    [SerializeField] private float _gravityScale = 1.25f;
    [SerializeField] private float _jumpForce = 5;
    bool _hasDoubleJump = false;


    private CharacterController _controller;

    [Space]
    [Header("FX")]
    public GameObject _fx;
    public Transform _spawnFxPoint;


    private void Start()
    {
        //_playerAnimator = FindObjectOfType<Animator>();
        _controller = FindObjectOfType<CharacterController>();
    }

    float _runSpeed = 1;
    bool isRunning;

    private void Update()
    {

        UpdateMovementInput();

        SlideMovement();
        PlayerJump();

        //Move player/ update anims

        MovePlayer();
       // UpdateWalkAnim();
    }
    void UpdateMovementInput()
    {
        //Set new direction to move
        //_moveDirection = new Vector3(Input.GetAxis("Horizontal") * _movementSpeed , /*_controller.velocity.y*/ _moveDirection.y , Input.GetAxis("Vertical") * _movementSpeed);
        if(Input.GetKey(KeyCode.LeftShift) && _controller.isGrounded)
        {
            _runSpeed = _movementSpeed / 2.5f;
            isRunning = true;
        }
        else
        {
            _runSpeed = 1f;
            isRunning = false;
        }

        float yStore = _moveDirection.y;

        _moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        _moveDirection = _moveDirection.normalized * _runSpeed * _movementSpeed;

        _moveDirection.y = yStore;
    }


    void SlideMovement()
    {
        var h = _controller.height;
        //-slide -
        if(Input.GetKeyDown(KeyCode.F) && !isSliding) // press F to slide
        {
            slideTimer = 0.0f; // start timer
            isSliding = true;
            slideForward = transform.forward;
        }
        if(isSliding)
        {
            h = 0.5f * _controller.height; // height is crouch height
            _finalSpeed = slideSpeed; // speed is slide speed
            _moveDirection = slideForward * _movementSpeed*2;

          //  if(transform.rotation.eulerAngles.y >= -60)
          //      transform.rotation = Mathf.Lerp ( transform.rotation.eulerAngles, Quaternion.Euler(1f,60f,1f),.25f);

            slideTimer += Time.deltaTime;

            if(slideTimer > slideTimerMax)
            {
                isSliding = false;
            }
        }

        _controller.height = Mathf.Lerp(_controller.height , h , 5 * Time.deltaTime);
    }

    private float _finalSpeed;

    void MovePlayer()
    {
        _moveDirection.y += Physics.gravity.y * _gravityScale * Time.deltaTime;
        //Finally move
        _controller.Move(_moveDirection * Time.deltaTime);
    }

    void PlayerJump()
    {
        if(_controller.isGrounded)
        {
            //Prevent the massive dicrease of direction.y, 
            //so set it 0 each time to avoid ulta gravity on fall from a hight
            _moveDirection.y = 0;
            _hasDoubleJump = true;

            if(Input.GetButtonDown("Jump"))
            {
                //UpdateJumpAnims();

                _moveDirection.y = _jumpForce;
                //Jump Anims
            }

        }
        //Perform double jump , if the player isnt touching the ground 
        //and is available to Double jump
        if(!_controller.isGrounded)
            if(_hasDoubleJump)
                if(Input.GetButtonDown("Jump"))
                {
                    _hasDoubleJump = false;

                    //Create jump FX
                    GameObject newFx = Instantiate(_fx);
                    newFx.transform.position = _spawnFxPoint.position;
                    //newFx.transform.localPosition = Vector3.zero;

                    _moveDirection.y = _jumpForce + _jumpForce / 2f;
                }
    }




    private void FixedUpdate()
    {
        //add default gravity
           

    }


    private void UpdateWalkAnim()
    {
        //Move Anims
        if(_moveDirection.magnitude > 1.5f && _controller.isGrounded)
            if(isRunning)
            {
                _playerAnimator.SetBool("IsRunning" , isRunning);
            }
            else
            {
                _playerAnimator.SetBool("IsRunning" , false);
                _playerAnimator.SetBool("IsWalking" , true);
            }

        else
            _playerAnimator.SetBool("IsWalking" , false);



        _playerAnimator.SetBool("IsGrounded" , _controller.isGrounded);
    }

    private void UpdateJumpAnims()
    {
        if(isRunning)
        {
            _playerAnimator.SetTrigger("RunningJump");
        }
        else if(_moveDirection.magnitude <= 1.5f)
        {
            _playerAnimator.SetTrigger("StaticJump");
        }
        else if(_moveDirection.magnitude >= 3.5f)
        {
            _playerAnimator.SetTrigger("WalkJump");
        }

    }

}

//var walkSpeed: float = 7; // regular speed
// var crchSpeed: float = 3; // crouching speed
// var runSpeed: float = 20; // run speed
 
// private var chMotor: CharacterMotor;
// private var ch: CharacterController;
// private var tr: Transform;
// private var height: float; // initial height
 
// public var slideSpeed: float = 20; // slide speed
// private var isSliding : boolean = false;
// private var slideForward : Vector3; // direction of slide
// private var slideTimer : float = 0.0;
// public var slideTimerMax : float = 2.5; // time while sliding
 
// function Start()
//{
//    chMotor = GetComponent(CharacterMotor);
//    tr = transform;
//    ch = GetComponent(CharacterController);
//    height = ch.height;
//}

//function Update()
//{
//    var h = height;
//    var speed = walkSpeed;

//    // - run and crouch -    
//    if(ch.isGrounded && Input.GetKey("left shift") || Input.GetKey("right shift")) // press Shift to run
//    {
//        speed = runSpeed;
//    }
//    if(Input.GetKey("c")) // press C to crouch
//    {
//        h = 0.5 * height;
//        speed = crchSpeed; // slow down when crouching
//    }

//    // - slide -    
//    if(Input.GetKeyDown("f") && !isSliding) // press F to slide
//    {
//        slideTimer = 0.0; // start timer
//        isSliding = true;
//        slideForward = tr.forward;
//    }
//    if(isSliding)
//    {
//        h = 0.5 * height; // height is crouch height
//        speed = slideSpeed; // speed is slide speed
//        chMotor.movement.velocity = slideForward * speed;

//        slideTimer += Time.deltaTime;
//        if(slideTimer > slideTimerMax)
//        {
//            isSliding = false;
//        }
//    }

//    // - apply movement modifiers -    
//    chMotor.movement.maxForwardSpeed = speed; // set max speed
//    var lastHeight = ch.height; // crouch/stand up smoothly 
//    ch.height = Mathf.Lerp(ch.height , h , 5 * Time.deltaTime);
//    tr.position.y += (ch.height - lastHeight) / 2; // fix vertical position
//}