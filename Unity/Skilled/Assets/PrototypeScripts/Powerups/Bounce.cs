using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour {
    public GameObject owner;
    Rigidbody2D _rigid;
    public float MaxSpeed = 1f;
    int _colCount = 0;
    public int MaxBalls = 5;
    public static int Max_Balls;
    public float constXSpeed = 3.0f;
    public float BounceForce = 250.0f;
   // private Vector3 _lastPos = Vector2.zero;
    [HideInInspector]
    public bool isClone = false;

    void Awake()
    {
        Max_Balls = MaxBalls;
    }

    void Start()
    {
        
        _rigid = gameObject.GetComponent<Rigidbody2D>();
        if(!isClone)
            LevelBounds.Instance.RegisterObject(gameObject);
    }

    /// <summary>
    /// not used currently
    /// </summary>
    /// <param name="maxSpeed"></param>
    /// <param name="maxYSpeed"></param>
    void CapSpeed(float maxSpeed, float maxYSpeed = -1)
    {
        if(maxYSpeed != -1 && Mathf.Abs(_rigid.velocity.y) > maxYSpeed)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, maxYSpeed * (_rigid.velocity.y < 0? -1f : 1f));
        }
        if (_rigid.velocity.magnitude > MaxSpeed)
        {
            _rigid.velocity.Normalize();
            _rigid.velocity *= maxSpeed;

        }
    }

    void ConstantSpeed()
    {
        _rigid.velocity = new Vector2(constXSpeed * (_rigid.velocity.x < 0? -1f : 1f), _rigid.velocity.y);
    }

    void Update()
    {
        //_lastPos = transform.position;
        ConstantSpeed();
        //CapSpeed(MaxSpeed, 5f);
        _colCount = 0;
    }

    void OnDestroy()
    {
        if(!isClone)
            LevelBounds.Instance.UnRegisterObject(gameObject);
    }



    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject == owner) return;
        //to make sure it doesn't hit multiple colliders on the players only 1 collisioncheck is allowed per frame
        if (_colCount > 0) return;
        _colCount++;
        //bounce from atop of blocks
        if (c.transform.position.y < transform.position.y)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, 0);
            _rigid.AddForce(Vector2.up * BounceForce);
        }

        //bounce from underneath blocks
        if(c.transform.position.y > transform.position.y)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, 0);
            _rigid.AddForce(Vector2.up * -BounceForce);
        }

        bool toDestroy = false;


        //player hit
        PlayerHit hit = c.gameObject.GetComponent<PlayerHit>();
        if(hit != null && hit.gameObject != owner)
        {
            hit.OnDeath(gameObject);
            toDestroy = true;
        }

      

        if(toDestroy)
            GameObject.Destroy(gameObject);
    }

}
