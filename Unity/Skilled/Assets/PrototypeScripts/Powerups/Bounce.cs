using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour {

    public GameObject owner;
    Rigidbody2D _rigid;
    public float MaxSpeed = 1f;
    int _colCount = 0;
    public float constXSpeed = 3.0f;
    private Vector3 _lastPos = Vector2.zero;

    void Start()
    {
        _rigid = gameObject.GetComponent<Rigidbody2D>();
        LevelBounds.Instance.RegisterObject(gameObject);
    }

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
        _lastPos = transform.position;
        ConstantSpeed();
        CapSpeed(MaxSpeed, 5f);
        _colCount = 0;
    }

    void OnDestroy()
    {
        LevelBounds.Instance.UnRegisterObject(gameObject);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        PlayerHit hit = c.collider.gameObject.GetComponent<PlayerHit>();
        if (hit == null) return;
        if (hit.gameObject == owner) return;
        hit.OnDeath(gameObject);
        GameObject.Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (_colCount > 0) return;
        if(c.transform.position.y < transform.position.y)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, 0);
            _rigid.AddForce(Vector2.up * 250f);
        }

        bool toDestroy = false;

        Vector2 diff = c.transform.position - _lastPos;
        if(Mathf.Abs(diff.x) > Mathf.Abs(diff.y))   //TODO NEEDS WORK, high velocity falling causes small y diff
        {
            toDestroy = true;
            Debug.Log(true);
        }


        

        _colCount++;
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
