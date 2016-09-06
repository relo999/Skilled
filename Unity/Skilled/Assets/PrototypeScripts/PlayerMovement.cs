using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerMovement : MonoBehaviour {

    public Controls controls = Controls.WASD;
    public float JumpForce = 250.0f;
    public float MoveSpeed = 1.0f;
    public bool AirControl = true;
    public bool MultipleJumps = false;
    public bool QuickStop = true;
    public bool QuickStopAIR = false;
    public bool Grounded { private set; get; }
    public float MaxYSpeed = 15.0f;
    public bool LastMovedRight { private set; get; }

    public enum Controls
    {
        WASD,
        ARROWS
    }
    Rigidbody2D _rigid;
	// Use this for initialization
	void Start () {
        _rigid = GetComponent<Rigidbody2D>();
	}
	
    void CapSpeed()
    {
        //TODO cap xspeed
        if(Mathf.Abs(_rigid.velocity.y) > MaxYSpeed)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, _rigid.velocity.y > 0 ? MaxYSpeed : -MaxYSpeed);
        }
    }


	void Update () {
        bool grounded = false;
        
        //racasts checking if player is standing on a block, casts from both edges of hitbox
        RaycastHit2D hit =  Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f + new Vector2(gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f, 0), -Vector2.up, 0.20f);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f - new Vector2(gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f, 0), -Vector2.up, 0.20f);


        if ((hit.transform != null && hit.transform != transform) || (hit2.transform != null && hit2.transform != transform))
        {

            grounded = true;
        }
        Grounded = grounded;

            //Quickstop before input check
        if (QuickStop && (grounded || QuickStopAIR))
        {
            _rigid.velocity = new Vector2(0, _rigid.velocity.y);
        }

        if (grounded || AirControl)
        {
            if (Input.GetKey(controls == Controls.WASD ? KeyCode.A : KeyCode.LeftArrow))
            {
                _rigid.velocity = new Vector2(Vector2.left.x * MoveSpeed, _rigid.velocity.y);
                LastMovedRight = false;
            }
            if (Input.GetKey(controls == Controls.WASD ? KeyCode.D : KeyCode.RightArrow))
            {
                _rigid.velocity = new Vector2(Vector2.right.x * MoveSpeed, _rigid.velocity.y);
                LastMovedRight = true;
            }
        }
        if (Input.GetKeyDown(controls == Controls.WASD ? KeyCode.W : KeyCode.UpArrow) &&( grounded || MultipleJumps))
        {
            _rigid.AddForce(Vector2.up * JumpForce);
        }

        CapSpeed();//do this last in update
    }
}
