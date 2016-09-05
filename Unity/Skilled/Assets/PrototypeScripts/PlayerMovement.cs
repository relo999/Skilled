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
	
	// Update is called once per frame
	void Update () {
      
        //if (!isLocalPlayer) return;
        bool grounded = false;
        
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f, -Vector2.up, 0.20f);
            //Debug.DrawRay(transform.position, -Vector2.up * 0.20f); //0.18 => slightly bigger than half of the sprite size
       
            // If it hits something...

            if (hit.transform != null && hit.transform != transform)
            {
                //Debug.Log(hit.transform.gameObject.name);
                grounded = true;
            }
        Grounded = grounded;
            //before input check
        if (QuickStop && (grounded || QuickStopAIR))
        {
            _rigid.velocity = new Vector2(0, _rigid.velocity.y);
        }

        if (grounded || AirControl)
        {
            if (Input.GetKey(controls == Controls.WASD ? KeyCode.A : KeyCode.LeftArrow))
            {
                _rigid.velocity = new Vector2(Vector2.left.x * MoveSpeed, _rigid.velocity.y);
            }
            if (Input.GetKey(controls == Controls.WASD ? KeyCode.D : KeyCode.RightArrow))
            {
                _rigid.velocity = new Vector2(Vector2.right.x * MoveSpeed, _rigid.velocity.y);
            }
        }
        if (Input.GetKeyDown(controls == Controls.WASD ? KeyCode.W : KeyCode.UpArrow) &&( grounded || MultipleJumps))
        {
            _rigid.AddForce(Vector2.up * JumpForce);
        }
    }
}
