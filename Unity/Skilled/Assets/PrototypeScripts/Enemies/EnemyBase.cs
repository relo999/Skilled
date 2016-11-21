using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyBase : MonoBehaviour {

    SpriteRenderer _spriteR;

    Rigidbody2D _rigid;

    [HideInInspector]
    public bool faceRight = true;

    [HideInInspector]
    private float MoveSpeed = 1.0f;

    BoxCollider2D _col;

    [HideInInspector]
    public bool isClone = false;

    float _lastXPos;

    public bool InstaKill = false;

    float _stunnedTime = 3; //seconds
    float _stunnedTimer = 0;


    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject other = col.collider.gameObject;
        Bounds thisBounds = _spriteR.sprite.bounds;

        SpriteRenderer SROther = other.GetComponent<SpriteRenderer>();
        if (SROther == null) return;    //Collided with something that doesn't interact with this
        Bounds otherBounds = SROther.sprite.bounds;
        PlayerHit hit = other.GetComponent<PlayerHit>();
        if(hit || other.GetComponent<EnemyBase>())
        {
            faceRight = !faceRight;
        } 
        if ((hit != null) &&
            other.transform.position.y  /*- otherBounds.size.y / 4f */>= transform.position.y + thisBounds.size.y / 2f &&   //TODO NEEDS WORK, more precise
            other.transform.position.x + otherBounds.size.x / 2f >= transform.position.x - thisBounds.size.x / 2f &&
            other.transform.position.x - otherBounds.size.x / 2f <= transform.position.x + thisBounds.size.x / 2f
            )
        {
            if (hit.isClone) return;    //no collisionCheck for looping level clones
            if (_stunnedTimer > 0 || InstaKill)
            {
                GameObject.Destroy(this.gameObject);
                //DEAD
            }
            else
            {
                _stunnedTimer = _stunnedTime;
                //START STUNNED ANIMATION

            }
        }

    }

    /*  //pseudo code
    if(hitOnHead)
    {
        if(_stunnedTimer > 0 || InstaKill)
        {
            //DEAD
        }
        else
        {
            _stunnedTimer = _stunnedTime;
            //START STUNNED ANIMATION
            
        }
    }
    */



	// Use this for initialization
	void Start () {
        _col = GetComponent<BoxCollider2D>();
        _rigid = GetComponent<Rigidbody2D>();
        _lastXPos = transform.position.x;
            _spriteR = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if(_stunnedTimer > 0)
        {
            _stunnedTimer -= Time.deltaTime;
            if(_stunnedTimer <= 0)
            {
                //STOP STUNNED ANIMATION
            }
            return;
        }
        if (isClone) return;
        if (transform.position.x == _lastXPos) faceRight = !faceRight;
        _lastXPos = transform.position.x;

        _rigid.velocity = new Vector2((faceRight ? 1.0f : -1.0f) * MoveSpeed, _rigid.velocity.y);
    }

}
