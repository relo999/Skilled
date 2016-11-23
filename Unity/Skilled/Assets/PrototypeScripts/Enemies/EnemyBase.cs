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

    protected SheetAnimation _sheetAni;
    private bool _isFalling = false;

    [HideInInspector]
    public bool isClone = false;

    float _lastXPos;

    public bool InstaKill = false;

    float _stunnedTime = 3; //seconds
    float _stunnedTimer = 0;

    Sprite[] WalkingSprites;
    Sprite[] FallingSprites;

    public Speed speed;
    public enum Speed
    {
        Slow,
        Medium,
        Fast
    }


    protected void OnDeath(PlayerHit player)
    {
        if (player) player.BounceUp(gameObject);
        GameObject.Destroy(this.gameObject);
    }

    protected virtual void OnPlayerHit(PlayerHit player, bool above)
    {
        if(!above)
            player.OnDeath(gameObject);
    }

    protected virtual void OnHit(PlayerHit player)
    {
        if (_stunnedTimer > 0 || InstaKill)
        {
            OnDeath(player);
            player.BounceUp(player.gameObject);
            //DEAD
        }
        else
        {
            player.BounceUp(player.gameObject);
            _stunnedTimer = _stunnedTime;
            //START STUNNED ANIMATION

        }
    }
  

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
            OnHit(hit);

            OnPlayerHit(hit, true);
        }
        else if(hit != null)
        {
            OnPlayerHit(hit, false);
        }

    }

    protected void Init()
    {
        _col = GetComponent<BoxCollider2D>();
        _rigid = GetComponent<Rigidbody2D>();
        _lastXPos = transform.position.x;
        _spriteR = GetComponent<SpriteRenderer>();
        _sheetAni = GetComponent<SheetAnimation>();
        _sheetAni.doIdle = false;
        if (speed == Speed.Medium) MoveSpeed *= 1.3f;
        if (speed == Speed.Fast) MoveSpeed *= 1.8f;
        CorrectSprite();
    }

	// Use this for initialization
	void Start () {
        Init();
	}

    void CorrectSprite()
    {
        string basePath = "SinglePlayer/Enemies/";
        string walkName = "";
        string fallName = "";
        switch (_spriteR.sprite.name.Substring(0,5))
        {
            case "Walki":   //default enemy
                basePath += "Base/";
                walkName = "WalkingBase_";
                fallName = "Falling_";
                break;
            case "Shiel":
                basePath += "Shield/";
                walkName = "ShieldWalk_";
                fallName = "FallingShielder_";
                break;
            case "Spike":
                basePath += "Spiker/";
                walkName = "Spiker_";
                fallName = "FallingSpiker_";
                break;
            case "Falli":
                basePath += "Faller/";
                walkName = "WalkingFaller_";
                fallName = "FallingFaller_";
                break;
            case "Split":
                basePath += "Splitter/";
                walkName = "SplitterWalk_";
                fallName = "FallingSplitter_";
                break;
            default:
                basePath = null;
                break;
        }
        if (basePath == null) Debug.LogWarning("Enemy Sprite Error..");
        else
        {
            walkName += speed.ToString()[0];
            fallName += speed.ToString()[0];
            WalkingSprites = Resources.LoadAll<Sprite>(basePath + walkName);
            FallingSprites = Resources.LoadAll<Sprite>(basePath + fallName);
            GetComponent<SheetAnimation>().PlayAnimationUnC(WalkingSprites, true, 5);
        }
        

    }

    protected virtual void EnemyUpdate()
    {
        //used for inhereting classes
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
            else return;
        }
        if (isClone) return;
        
        if(!_isFalling && _rigid.velocity.y < -0.5f)
        {
            _sheetAni.PlayAnimationUnC(FallingSprites);
            _isFalling = true;
        }
        else
        {
            if(_isFalling && _rigid.velocity.y >= 0)
            {
                _sheetAni.PlayAnimationUnC(WalkingSprites);
                _isFalling = false;
            }
        }
        EnemyUpdate();
        if (transform.position.x == _lastXPos || Mathf.Abs(_rigid.velocity.x) < 0.1f) faceRight = !faceRight;
        if(_spriteR)
            _spriteR.flipX = faceRight;
        _lastXPos = transform.position.x;
        if(_rigid)
            _rigid.velocity = new Vector2((faceRight ? 1.0f : -1.0f) * MoveSpeed, _rigid.velocity.y);
    }

}
