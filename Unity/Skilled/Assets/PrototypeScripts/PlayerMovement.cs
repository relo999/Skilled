using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TeamUtility.IO;
using System;

public class PlayerMovement : MonoBehaviour {

    public NetworkBase.PlayerInput input;
    public PlayerID playerID;
    public Controls controls = Controls.WASD;
    public float JumpForce = 250.0f;
    public float HoldJumpForce = 300.0f;
    public float HoldJumpMaxSec = 0.3f;
    public float jumpGravityScale = 0.5f;

    //public float HoldJumpDecay = 0.75f; //lower means higher/longer jumps
    //public float JumpENDForceDown = 50.0f;  //when releasing the jump key

    public float MoveSpeed = 1.0f;
    public bool AirControl = true;
    public bool MultipleJumps = false;
    public bool QuickStop = true;
    public bool QuickStopAIR = false;
    public bool Grounded { private set; get; }
    public float MaxYSpeed = 15.0f;
    public bool LastMovedRight { private set; get; }
    SpriteOverlay overlay;

    bool _isJumping = false;
    float _currentJumpForce;
    private float jumpTimer = 0.0f;

    public float StunnedTimer = 0;

    SpriteRenderer SpriteR;

    SheetAnimation SAnimation;
    SheetAnimation.PlayerColor Pcolor;
    int currentAxis = 0;
    [HideInInspector]
    public bool canMove = true;
 
    public bool NetworkControl = false;

    public Vector2[] oldPositions = new Vector2[10];
    public int oldPositionPointer = 0;
    float oldPositionTimer = 0;


    bool holdingJump = false;

    public bool OnlineGame = false;

    public enum Controls
    {
        CONTROLLER,
        WASD,
        ARROWS
    }
    Rigidbody2D _rigid;
	// Use this for initialization
	void Start () {

       

        _rigid = GetComponent<Rigidbody2D>();
        SpriteR = GetComponent<SpriteRenderer>();

        SAnimation = GetComponent<SheetAnimation>();
        Pcolor = GetComponent<PlayerHit>().color;
        overlay = GetComponent<SpriteOverlay>();
        if (currentAxis == 0 && InputManager.GetAxis("Horizontal", playerID) != 0)
        {
            ChangeAxis();
            
        }
    }

    
    void SaveOldPosition()
    {
        oldPositionTimer += Time.deltaTime;
        if (oldPositionTimer >= 0.05f)
        {
            oldPositionTimer = 0;
            oldPositions[oldPositionPointer] = transform.position;
            oldPositionPointer++;
            oldPositionPointer %= 10;

        }
    }
	
    void CapSpeed()
    {
        //TODO cap xspeed
        if(Mathf.Abs(_rigid.velocity.y) > MaxYSpeed)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, _rigid.velocity.y > 0 ? MaxYSpeed : -MaxYSpeed);
        }
    }

    public void ForceStopJump()
    {
        _isJumping = false;
        _rigid.velocity = Vector2.zero;
    }

    public void DoMovement(NetworkBase.PlayerInput input)
    {
        if (!canMove) return;
        //if (NetworkControl) return;


        bool grounded = false;

        float spriteXSizeHalf = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f;
        //racasts checking if player is standing on a block, casts from both edges of hitbox
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f + new Vector2(spriteXSizeHalf - 0.05f, 0), -Vector2.up, 0.10f);
        //RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f - new Vector2(gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f, 0), -Vector2.up, 0.10f);
        //if ((hit.transform != null && hit.transform != transform) || (hit2.transform != null && hit2.transform != transform))
        //    grounded = true;
        if (hit.transform != null && hit.transform != transform) grounded = true;
        else
        {
            RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f - new Vector2(spriteXSizeHalf - 0.05f, 0), -Vector2.up, 0.10f);
            if (hit2.transform != null && hit2.transform != transform) grounded = true;
        }

        //if (!Grounded && grounded) SAnimation.PlayAnimation("Idle", Pcolor);
        Grounded = grounded;

        /*
        //Quickstop before input check
        if ((QuickStop && grounded) || (!grounded && QuickStopAIR))
        {
            _rigid.velocity = new Vector2(0, _rigid.velocity.y);
        }*/

        if (grounded || AirControl)
        {
            Vector2 movement = new Vector2(0, _rigid.velocity.y);
            //if (Input.GetKey(controls == Controls.WASD ? KeyCode.A : KeyCode.LeftArrow)  || InputManager.GetAxis("Horizontal", playerID) < 0)
            if (input.xAxis < 0)
            {
                movement.x += Vector2.left.x * MoveSpeed;
                LastMovedRight = false;
            }
            if (input.xAxis > 0)
            {
                movement.x += Vector2.right.x * MoveSpeed;
                LastMovedRight = true;
            }
      
            _rigid.velocity = movement;
        }


        if (input.JumpUp|| (!grounded && _rigid.velocity.y < 0.1f && _rigid.velocity.y > -0.1f))
        {
            _isJumping = false;
            holdingJump = false;
            // _rigid.AddForce(Vector2.down * JumpENDForceDown);
            //_rigid.gravityScale = 1.0f;
        }


        if (input.JumpDown)
        {
            if (!_isJumping && (grounded || MultipleJumps))
            {
                jumpTimer = HoldJumpMaxSec;
                _isJumping = true;
                holdingJump = true;
                SAnimation.PlayAnimation("Jump", Pcolor, false, 5, 2, 0);
                SAnimation.SetFrame(2);
                //_currentJumpForce = (Physics.gravity * _rigid.mass).magnitude * 5f;
                _rigid.AddForce(Vector2.up * JumpForce);
                ScoreManager.gameData.Jumped++;
            }
        }

        /*
        if (input.Jump && _isJumping) holdingJump = true;
        else holdingJump = false;
        */

            
     



    }

    void ChangeAxis()
    {
        currentAxis = 7;
        AxisConfiguration button = InputManager.GetAxisConfiguration((PlayerID)playerID, "Horizontal");
        button.axis = 7;

        AxisConfiguration button2 = InputManager.GetAxisConfiguration((PlayerID)playerID, "Vertical");
        button2.sensitivity = -1.0f;   //some controllers have inverted axis for some reason
        button2.axis = 8;

        ControllerBind bindings = FindObjectOfType<ControllerBind>();
        bindings.ChangeButton(playerID, "Jump", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (int)(playerID + 1) + "Button" + 0));
        bindings.ChangeButton(playerID, "Action", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (int)(playerID + 1) + "Button" + 2));
    }


	void Update () {
        bool doInput = NetworkControl || !OnlineGame;
        //if (!NetworkControl) return;
        if(NetManager.hasStarted && !NetManager.isServer && doInput)
        {
            if (controls == Controls.CONTROLLER)
                input = new NetworkBase.PlayerInput((int)playerID, InputManager.GetAxis("Horizontal", playerID),
                                                                   InputManager.GetButton("Jump", playerID),
                                                                   InputManager.GetButtonDown("Jump", playerID),
                                                                   InputManager.GetButtonUp("Jump", playerID),
                                                                   InputManager.GetButtonDown("Action", playerID));
            if (controls == Controls.WASD)
                input = new NetworkBase.PlayerInput((int)playerID, Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D)? 1 : 0),
                                                                   Input.GetKey(KeyCode.W),
                                                                   Input.GetKeyDown(KeyCode.W),
                                                                   Input.GetKeyUp(KeyCode.W),
                                                                   Input.GetKeyDown(KeyCode.Space));
            if (controls == Controls.ARROWS)
                input = new NetworkBase.PlayerInput((int)playerID, Input.GetKey(KeyCode.LeftArrow) ? -1 : (Input.GetKey(KeyCode.RightArrow) ? 1 : 0),
                                                                   Input.GetKey(KeyCode.UpArrow),
                                                                   Input.GetKeyDown(KeyCode.UpArrow),
                                                                   Input.GetKeyUp(KeyCode.UpArrow),
                                                                   Input.GetKeyDown(KeyCode.L));
            if (input.xAxis != 0 || input.Jump || input.JumpDown || input.JumpUp || input.Action)
                NetManager.instance.SendInput(input);


        }

      
        //return; //TESTING


        if (!canMove) return;
        if (StunnedTimer > 0)
        {
            StunnedTimer -= Time.deltaTime / 1.5f;
            return;
        }
        if (doInput) SaveOldPosition();

        bool grounded = false;            

        float spriteXSizeHalf = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f;
        //racasts checking if player is standing on a block, casts from both edges of hitbox
        RaycastHit2D hit =  Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f + new Vector2(spriteXSizeHalf-0.05f, 0), -Vector2.up, 0.10f);
        //RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f - new Vector2(gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f, 0), -Vector2.up, 0.10f);
        //if ((hit.transform != null && hit.transform != transform) || (hit2.transform != null && hit2.transform != transform))
        //    grounded = true;
        if (hit.transform != null && hit.transform != transform) grounded = true;
        else
        {
            RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f - new Vector2(spriteXSizeHalf-0.05f, 0), -Vector2.up, 0.10f);
            if (hit2.transform != null && hit2.transform != transform) grounded = true;
        }


        if (SAnimation.GetAnimation() == "Jump")
        {
            int newFrame = _rigid.velocity.y > -1 ? (_rigid.velocity.y < 1 ? 3 : 2) : 4;
            if(SAnimation.GetFrame() != newFrame)
                SAnimation.SetFrame(newFrame);
        }
        //if (!Grounded && grounded) SAnimation.PlayAnimation("Idle", Pcolor);
        Grounded = grounded;

        /*
            //Quickstop before input check
        if ((QuickStop && grounded) || (!grounded && QuickStopAIR))
        {
            _rigid.velocity = new Vector2(0, _rigid.velocity.y);
        }*/

        if (grounded || AirControl)
        {

            Vector2 movement = new Vector2(_rigid.velocity.x * 0.9f, _rigid.velocity.y);   //TODO change x back to 0
            //if (Input.GetKey(controls == Controls.WASD ? KeyCode.A : KeyCode.LeftArrow)  || InputManager.GetAxis("Horizontal", playerID) < 0)
            if ((controls == Controls.WASD && Input.GetKey(KeyCode.A)) || (controls == Controls.ARROWS && Input.GetKey(KeyCode.LeftArrow))  || InputManager.GetAxis("Horizontal", playerID) < 0)
            {
                if (doInput)
                {
                    movement.x = Vector2.left.x * MoveSpeed;
                    LastMovedRight = false;
                }
            }
            if ((controls == Controls.WASD && Input.GetKey(KeyCode.D)) || (controls == Controls.ARROWS && Input.GetKey(KeyCode.RightArrow)) || InputManager.GetAxis("Horizontal", playerID) > 0)
            {
                if (doInput)
                {
                    movement.x = Vector2.right.x * MoveSpeed;
                    LastMovedRight = true;
                }
            }
            if (Grounded && !_isJumping)
            {
                if (movement.x != 0)
                {
                    if (SAnimation.GetAnimation() != "Run")
                    {
                        SAnimation.PlayAnimation("Run", Pcolor, true, 16);
                    }
                }
                else if (SAnimation.GetAnimation() != "Idle" && (SAnimation.GetAnimation() != "Spawn" || (SAnimation.GetAnimation() == "Spawn"&& SAnimation.OnLastFrame())))
                {
                    SAnimation.PlayAnimation("Idle", Pcolor, true, 5);
                }
            }
            if (movement.x == 0 && ((!QuickStopAIR && !grounded) || (!QuickStop && grounded))) movement = _rigid.velocity;
            _rigid.velocity = movement;
        }


        if (doInput)
        {
            //Debug.Log(InputManager.GetButtonDown("Jump", playerID));
            if ((controls == Controls.WASD && Input.GetKeyUp(KeyCode.W)) || (controls == Controls.ARROWS && Input.GetKeyUp(KeyCode.UpArrow)) || InputManager.GetButtonUp("Jump", playerID) || (!grounded && _rigid.velocity.y < 0.1f && _rigid.velocity.y > -0.1f))
            {
                _isJumping = false;
                holdingJump = false;
                // _rigid.AddForce(Vector2.down * JumpENDForceDown);
                //_rigid.gravityScale = 1.0f;
            }
            if ((controls == Controls.WASD && Input.GetKeyDown(KeyCode.W)) || (controls == Controls.ARROWS && Input.GetKeyDown(KeyCode.UpArrow)) || InputManager.GetButtonDown("Jump", playerID))
            {
                if (!_isJumping && (grounded || MultipleJumps))
                {
                    jumpTimer = HoldJumpMaxSec;
                    _isJumping = true;
                    holdingJump = true;
                    SAnimation.PlayAnimation("Jump", Pcolor, false, 5, 2, 0);
                    SAnimation.SetFrame(2);
                    //_currentJumpForce = (Physics.gravity * _rigid.mass).magnitude * 5f;
                    _rigid.AddForce(Vector2.up * JumpForce);
                    ScoreManager.gameData.Jumped++;
                }
            }
        }

        if (_isJumping || holdingJump)
        {
            
             
            if (jumpTimer > 0)
            {
                _rigid.gravityScale = jumpGravityScale;
                _rigid.AddForce(Vector2.up * HoldJumpForce * Time.deltaTime);
            }
            else
            {
                if (_rigid.velocity.y > -1) SAnimation.PlayAnimation("Jump", Pcolor, false, 5, 3);
            }

            //_currentJumpForce *= HoldJumpDecay;
        }
        else
        {
            _rigid.gravityScale = 1.0f;
        }
        if (jumpTimer > 0) jumpTimer -= Time.deltaTime;
        SpriteR.flipX = !LastMovedRight;
        overlay.flipX = !LastMovedRight;

        if ((controls == Controls.WASD && Input.GetKey(KeyCode.S)) || (controls == Controls.ARROWS && Input.GetKey(KeyCode.DownArrow)) || InputManager.GetAxis("Vertical", playerID) > 0)
        {
            if (SAnimation.GetAnimation() != "Jump")
            {
                SAnimation.PlayAnimation("Jump", Pcolor, false, 0, 7, 7);

            }
            SAnimation.SetFrame(5);
        }

        CapSpeed();//do this last in update
    }
}
