using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TeamUtility.IO;


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
        Debug.Log("Axis received: " + input.xAxis);
        Vector2 movement = new Vector2(0,_rigid.velocity.y);

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
        SpriteR.flipX = !LastMovedRight;
    }

	void Update () {
        //input = new NetworkBase.PlayerInput((int)playerID, InputManager.GetAxis("Horizontal", playerID),InputManager.GetButtonDown("Jump", playerID));

        //return;
        if(StunnedTimer >0)
        {
            StunnedTimer -= Time.deltaTime/1.5f;
            return;
        }

        bool grounded = false;

        float spriteXSizeHalf = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f;
        //racasts checking if player is standing on a block, casts from both edges of hitbox
        RaycastHit2D hit =  Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f + new Vector2(spriteXSizeHalf, 0), -Vector2.up, 0.10f);
        //RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f - new Vector2(gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f, 0), -Vector2.up, 0.10f);
        //if ((hit.transform != null && hit.transform != transform) || (hit2.transform != null && hit2.transform != transform))
        //    grounded = true;
        if (hit.transform != null && hit.transform != transform) grounded = true;
        else
        {
            RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f - new Vector2(spriteXSizeHalf, 0), -Vector2.up, 0.10f);
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

            //Quickstop before input check
        if (QuickStop && (grounded || QuickStopAIR))
        {
            _rigid.velocity = new Vector2(0, _rigid.velocity.y);
        }

        if (grounded || AirControl)
        {
            Vector2 movement = new Vector2(0, _rigid.velocity.y);
            //if (Input.GetKey(controls == Controls.WASD ? KeyCode.A : KeyCode.LeftArrow)  || InputManager.GetAxis("Horizontal", playerID) < 0)
            if ((controls == Controls.WASD && Input.GetKey(KeyCode.A)) || (controls == Controls.ARROWS && Input.GetKey(KeyCode.LeftArrow))  || InputManager.GetAxis("Horizontal", playerID) < 0)
            {
                movement.x += Vector2.left.x * MoveSpeed;
                LastMovedRight = false;
            }
            if ((controls == Controls.WASD && Input.GetKey(KeyCode.D)) || (controls == Controls.ARROWS && Input.GetKey(KeyCode.RightArrow)) || InputManager.GetAxis("Horizontal", playerID) > 0)
            {
                movement.x += Vector2.right.x * MoveSpeed;
                LastMovedRight = true;
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
            _rigid.velocity = movement;
        }

        

        //Debug.Log(InputManager.GetButtonDown("Jump", playerID));
        if ((controls == Controls.WASD && Input.GetKeyUp(KeyCode.W)) || (controls == Controls.ARROWS && Input.GetKeyUp(KeyCode.UpArrow)) || InputManager.GetButtonUp("Jump", playerID) || (!grounded && _rigid.velocity.y < 0.1f && _rigid.velocity.y > -0.1f) )
        {
            _isJumping = false;
           // _rigid.AddForce(Vector2.down * JumpENDForceDown);
            //_rigid.gravityScale = 1.0f;
        }
        if ((controls == Controls.WASD && Input.GetKeyDown(KeyCode.W)) || (controls == Controls.ARROWS && Input.GetKeyDown(KeyCode.UpArrow)) || InputManager.GetButtonDown("Jump", playerID))
        {
            if (!_isJumping && (grounded || MultipleJumps))
            {
                jumpTimer = HoldJumpMaxSec;
                _isJumping = true;
                SAnimation.PlayAnimation("Jump", Pcolor, false, 5, 2, 0);
                SAnimation.SetFrame(2);
                //_currentJumpForce = (Physics.gravity * _rigid.mass).magnitude * 5f;
                _rigid.AddForce(Vector2.up * JumpForce);
            }
        }
        if ((controls == Controls.WASD && Input.GetKey(KeyCode.W)) || (controls == Controls.ARROWS && Input.GetKey(KeyCode.UpArrow)) || InputManager.GetButton("Jump", playerID) && _isJumping)
        {
            
             
            if (jumpTimer > 0)
            {
                _rigid.gravityScale = jumpGravityScale;
                _rigid.AddForce(Vector2.up * HoldJumpForce * Time.deltaTime);
            }
            else
            {
                //if (_rigid.velocity.y > -1) SAnimation.PlayAnimation("Jump", Pcolor, false, 5, 3);
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
        CapSpeed();//do this last in update
    }
}
