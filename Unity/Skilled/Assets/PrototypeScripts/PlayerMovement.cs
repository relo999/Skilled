using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TeamUtility.IO;
using System;
using System.Collections.Generic;

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

    float baseMoveSpeed;
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

    public Vector2[] oldPositions = new Vector2[32];
    public int oldPositionPointer = 0;
    float oldPositionTimer = 0;

  
    public List<NetworkPosition> networkPositions = new List<NetworkPosition>();
    const float timeDelay = 0.10f;



    public NetworkBase.PlayerInput oldInput = null;


    bool holdingJump = false;

    public bool OnlineGame = false;


    public struct NetworkPosition
    {
        public Vector2 Position;
        public float GameTime;
        public Vector2 Velocity;

        public NetworkPosition(Vector2 Position, float GameTime, Vector2 Velocity)
        {
            this.Position = Position;
            this.GameTime = GameTime;
            this.Velocity = Velocity;
        }
    }


    public enum Controls
    {
        CONTROLLER,
        WASD,
        ARROWS
    }
    Rigidbody2D _rigid;

    public GameObject testClone;
    public GameObject[] testClones = new GameObject[32];
    public GameObject testClone2;


    public void SetMoveSpeed(float speed)
    {
        if (baseMoveSpeed == 0) baseMoveSpeed = MoveSpeed;
        MoveSpeed = baseMoveSpeed * speed;
    }
    public void ResetMoveSpeed()
    {
        MoveSpeed = baseMoveSpeed;
    }

	// Use this for initialization
	void Start () {
        baseMoveSpeed = MoveSpeed;
        _rigid = GetComponent<Rigidbody2D>();
        SpriteR = GetComponent<SpriteRenderer>();

        SAnimation = GetComponent<SheetAnimation>();
        Pcolor = GetComponent<PlayerHit>().color;
        overlay = GetComponent<SpriteOverlay>();
        if (currentAxis == 0 && InputManager.GetAxis("Horizontal", playerID) != 0)
        {
            ChangeAxis();
            
        }
        /*
        GameObject clones = new GameObject("test clones");
        testClone = new GameObject();
        testClone.transform.parent = clones.transform;
        testClone.AddComponent<SpriteRenderer>().sprite = SpriteR.sprite;
        testClone.GetComponent<SpriteRenderer>().sortingOrder = SpriteR.sortingOrder - 1;
        testClone.transform.localScale = new Vector3(testClone.transform.localScale.x * 0.3f, testClone.transform.localScale.y * 0.3f, testClone.transform.localScale.z);

        testClone2 = new GameObject();
        testClone2.transform.parent = clones.transform;
        testClone2.AddComponent<SpriteRenderer>().sprite = SpriteR.sprite;
        testClone2.GetComponent<SpriteRenderer>().flipY = true;
        testClone2.GetComponent<SpriteRenderer>().sortingOrder = SpriteR.sortingOrder - 2;
        testClone2.GetComponent<SpriteRenderer>().color = Color.yellow;
        testClone2.transform.localScale = new Vector3(testClone2.transform.localScale.x * 0.3f, testClone2.transform.localScale.y * 0.3f, testClone2.transform.localScale.z);

        for (int i = 0; i < testClones.Length; i++)
        {
            testClones[i] = new GameObject();
            testClones[i].transform.parent = clones.transform;
            testClones[i].AddComponent<SpriteRenderer>().sprite = SpriteR.sprite;
            testClones[i].GetComponent<SpriteRenderer>().flipY = true;
            testClones[i].GetComponent<SpriteRenderer>().sortingOrder = SpriteR.sortingOrder - 3;
            testClones[i].GetComponent<SpriteRenderer>().color = Color.magenta;
            testClones[i].transform.localScale = new Vector3(testClones[i].transform.localScale.x * 0.3f, testClones[i].transform.localScale.y * 0.3f + ((float)i / 25f), testClones[i].transform.localScale.z);
        }
        */

    }

    
    void SaveOldPosition()
    {
        oldPositionTimer += Time.deltaTime;
        if (oldPositionTimer >= 0.015f)
        {
            oldPositionTimer = 0;
            oldPositions[oldPositionPointer] = transform.position;
            oldPositionPointer++;
            oldPositionPointer %= oldPositions.Length;

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
            Vector2 movement = new Vector2((_rigid.velocity.x < 0? Vector2.left.x : (_rigid.velocity.x > 0? Vector2.right.x : 0)) * MoveSpeed, _rigid.velocity.y);
            //if (Input.GetKey(controls == Controls.WASD ? KeyCode.A : KeyCode.LeftArrow)  || InputManager.GetAxis("Horizontal", playerID) < 0)
            if (input.xAxis < 0)
            {
                movement.x = Vector2.left.x * MoveSpeed;
                LastMovedRight = false;
            }
            if (input.xAxis > 0)
            {
                movement.x = Vector2.right.x * MoveSpeed;
                LastMovedRight = true;
            }
            if (input.xAxis == 0) movement.x = 0;
      
            _rigid.velocity = movement;
        }


        if (input.JumpUp|| (!grounded && _rigid.velocity.y < 0.1f && _rigid.velocity.y > -0.1f))
        {
            _isJumping = false;
            holdingJump = false;
            // _rigid.AddForce(Vector2.down * JumpENDForceDown);
            //_rigid.gravityScale = 1.0f;
        }


        if (input.JumpDown || input.Jump)
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
                //ScoreManager.gameData.Jumped++;
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


    NetworkPosition FindClosestTime(float time, bool higher)
    {
        NetworkPosition closest = new NetworkPosition(Vector2.zero, float.MaxValue, Vector2.zero);
        foreach(NetworkPosition netPos in networkPositions)
        {
            if (higher? netPos.GameTime > time : netPos.GameTime < time &&
                Mathf.Abs(time - netPos.GameTime) < closest.GameTime) closest = netPos;
        }

        return closest;
    }

    void DoAnimations()
    {
        if (SAnimation.GetAnimation() == "Jump")
        {
            int newFrame = _rigid.velocity.y > -1 ? (_rigid.velocity.y < 1 ? 3 : 2) : 4;
            if (SAnimation.GetFrame() != newFrame)
                SAnimation.SetFrame(newFrame);
        }


        if (Grounded && !_isJumping)
        {
            if (_rigid.velocity.x != 0)
            {
                if (SAnimation.GetAnimation() != "Run")
                {
                    SAnimation.PlayAnimation("Run", Pcolor, true, 16);
                }
            }
            else if (SAnimation.GetAnimation() != "Idle" && (SAnimation.GetAnimation() != "Spawn" || (SAnimation.GetAnimation() == "Spawn" && SAnimation.OnLastFrame())) && SAnimation.GetAnimation() != "Death")
            {
                SAnimation.PlayAnimation("Idle", Pcolor, true, 5);
            }
        }


        SpriteR.flipX = _rigid.velocity.x == 0? SpriteR.flipX : _rigid.velocity.x < 0;

    }

	void Update () {
        if (Pauzed.IsPauzed) return;
        bool doInput = NetworkControl || !OnlineGame;
        if ((OnlineGame && NetworkControl )|| !OnlineGame || true)//!NetworkControl || true)
        {
            if (holdingJump)
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
        }
        DoAnimations();
        if (!NetworkControl && OnlineGame)
        {
            if (!NetManager.isServer)
            {
                if (networkPositions.Count < 2) return;
                float renderingGameTime = NetworkBase.GameTimer - timeDelay;

                if (networkPositions.Count > 20) networkPositions.RemoveRange(0, networkPositions.Count - 10);
                NetworkPosition minClosest = FindClosestTime(renderingGameTime, false);
                NetworkPosition maxClosest = FindClosestTime(renderingGameTime, true);

                if (minClosest.GameTime == 0 || maxClosest.GameTime == 0) return;
                float percentage = (renderingGameTime - minClosest.GameTime) / (maxClosest.GameTime - minClosest.GameTime);
                Vector2 newPos = minClosest.Position + ((maxClosest.Position - minClosest.Position) * percentage);
                Vector2 newVel = minClosest.Velocity + ((maxClosest.Velocity - minClosest.Velocity) * percentage);
                if (newPos.x != newPos.x || newPos.y != newPos.y) return;   //NaN
                                                                            //Debug.Log(renderingGameTime + " - " + minClosest.GameTime + " - " + maxClosest.GameTime + " : " + percentage);
                //newVel = networkPositions[networkPositions.Count - 1].Velocity;
                //newPos = networkPositions[networkPositions.Count - 1].Position;
                _rigid.velocity = newVel;
                transform.localPosition = newPos;
            }
            return;
        }
        else if (OnlineGame)
        {
            if (GameClient.instance != null)
            {
                float time = NetworkBase.GameTimer - ((float)GameClient.instance.Ping / 1000f);
                NetworkPosition minClosest = FindClosestTime(time, false);
                NetworkPosition maxClosest = FindClosestTime(time, true);
                transform.position = minClosest.Position.magnitude == 0 ? (maxClosest.Position.magnitude == 0 ? (Vector2)transform.position : maxClosest.Position) : minClosest.Position;
                
                Debug.Log(transform.position.x + " -:- " + FindClosestTime(NetworkBase.GameTimer - GameClient.instance.Ping, false).Position.x + " : " + FindClosestTime(NetworkBase.GameTimer - GameClient.instance.Ping, true).Position.x);
            }
        }

        if ((NetManager.hasStarted || !OnlineGame) && doInput)
        {
            if (controls == Controls.CONTROLLER)
                input = new NetworkBase.PlayerInput((int)playerID, InputManager.GetAxis("Horizontal", playerID),
                                                                   InputManager.GetButton("Jump", playerID),
                                                                   InputManager.GetButtonDown("Jump", playerID),
                                                                   InputManager.GetButtonUp("Jump", playerID),
                                                                   InputManager.GetButtonDown("Action", playerID));
            if (controls == Controls.WASD)
                input = new NetworkBase.PlayerInput((int)playerID, Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0),
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


            if (OnlineGame && ((input.xAxis != 0 || (oldInput != null && oldInput.xAxis != input.xAxis)) || input.Jump || input.JumpDown || input.JumpUp || input.Action) && !NetManager.isServer)

            {
                Debug.Log("sending input..");
                NetManager.instance.SendInput(input);
            }
            oldInput = input;
            if (doInput) SaveOldPosition();
            if (!canMove) return;
            if (StunnedTimer > 0)
            {
                StunnedTimer -= Time.deltaTime / 1.5f;
                return;
            }
            DoMovement(input);
            return;

        }
        //DoMovement(input);
        //return;
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

            Vector2 movement = new Vector2(0, _rigid.velocity.y);   //TODO change x back to 0
            //if (Input.GetKey(controls == Controls.WASD ? KeyCode.A : KeyCode.LeftArrow)  || InputManager.GetAxis("Horizontal", playerID) < 0)
            if (input.xAxis < 0)
            {
                if (doInput)
                {
                    movement.x = Vector2.left.x * MoveSpeed;
                    LastMovedRight = false;
                }
            }
            if (input.xAxis > 0)
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
            if (input.xAxis == 0 && NetworkControl && ((!QuickStopAIR && !grounded) || (!QuickStop && grounded))) movement = _rigid.velocity;
            _rigid.velocity = movement;
        }


        if (doInput)
        {
            //Debug.Log(InputManager.GetButtonDown("Jump", playerID));
            if ((controls == Controls.WASD && Input.GetKeyUp(KeyCode.W)) || (controls == Controls.ARROWS && Input.GetKeyUp(KeyCode.UpArrow)) || InputManager.GetButtonUp("Jump", playerID) || (!grounded && _rigid.velocity.y < 0.1f && _rigid.velocity.y > -0.1f))
            {
                _isJumping = false;
                //holdingJump = false;
                // _rigid.AddForce(Vector2.down * JumpENDForceDown);
                //_rigid.gravityScale = 1.0f;
            }
            if ((controls == Controls.WASD && Input.GetKeyDown(KeyCode.W)) || (controls == Controls.ARROWS && Input.GetKeyDown(KeyCode.UpArrow)) || InputManager.GetButtonDown("Jump", playerID))
            {
                if (!_isJumping && (grounded || MultipleJumps))
                {
                    jumpTimer = HoldJumpMaxSec;
                    _isJumping = true;
                    //holdingJump = true;
                    SAnimation.PlayAnimation("Jump", Pcolor, false, 5, 2, 0);
                    SAnimation.SetFrame(2);
                    //_currentJumpForce = (Physics.gravity * _rigid.mass).magnitude * 5f;
                    _rigid.AddForce(Vector2.up * JumpForce);
                    ScoreManager.gameData.Jumped++;
                }
            }
        }

        if (_isJumping)
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
