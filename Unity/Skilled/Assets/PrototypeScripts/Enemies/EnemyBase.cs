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

    float[] moveSpeeds = new float[3]
        {1f, 1.5f, 2.0f};

    public float spawnInvulnerability = 0;

    Sprite[] _stunnedSpriteSheet = null;
    GameObject _stunnedObject = null;
    public bool DoStun = true;

    float BounceStrength = 4.5f;

    public Speed speed;
    public enum Speed
    {
        Slow,
        Medium,
        Fast
    }

    void StartStunAnimation()
    {
        _stunnedObject.SetActive(true);
        _stunnedObject.GetComponent<SheetAnimation>().PlayAnimationUnC(_stunnedSpriteSheet, true, 8);
    }

    void StopStunAnimation()
    {
        _stunnedObject.SetActive(false);
        if ((int)speed <= 1)
        {
            speed++;
            SetSpeed();
            CorrectSprite();
        }
    }

    protected virtual void OnSpeedChanged()
    {
        //empty
    }

    private void DoSplat()
    {
        int color = 0;
        switch (speed)
        {
            case Speed.Slow:
                color = (int)SheetAnimation.PlayerColor.green;
                break;
            case Speed.Medium:
                color = (int)SheetAnimation.PlayerColor.yellow;
                break;
            case Speed.Fast:
                color = (int)SheetAnimation.PlayerColor.red;
                break;

        }
        FindObjectOfType<Splat>().DoSplat(transform.position, 0, color);

    }

    protected void OnDeath(PlayerHit player)
    {
        if (player) player.BounceUp(gameObject);
        DoSplat();
        GameObject.Destroy(this.gameObject);
    }

    protected virtual void OnPlayerHit(PlayerHit player, bool above)
    {
        if(!above)
            player.OnDeath(gameObject);
    }

    protected virtual void HitCallback(PlayerHit player)
    {
        OnDeath(player);
        player.BounceUp(player.gameObject);
        //dead
    }

    private void OnHit(PlayerHit player)
    {
        if (_stunnedTimer > 0 || InstaKill || !DoStun)
        {
            HitCallback(player);

        }
        else
        {
            player.BounceUp(player.gameObject);
            _stunnedTimer = _stunnedTime;
            //START STUNNED ANIMATION
            _rigid.velocity = new Vector2(transform.position.x < player.transform.position.x ? -1 : 1, 1) * BounceStrength;
            StartStunAnimation();

        }
    }
  

    void OnCollisionEnter2D(Collision2D col)
    {
        if (spawnInvulnerability > 0) return;
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


    void SetSpeed()
    {
        float newMoveSpeed = MoveSpeed;
        if (speed == Speed.Medium) newMoveSpeed = moveSpeeds[1];
        if (speed == Speed.Fast) newMoveSpeed = moveSpeeds[2];
        if (MoveSpeed != newMoveSpeed) OnSpeedChanged();
        MoveSpeed = newMoveSpeed;
    }

    public void Init()
    {
        _col = GetComponent<BoxCollider2D>();
        _rigid = GetComponent<Rigidbody2D>();
        _lastXPos = transform.position.x;
        _spriteR = GetComponent<SpriteRenderer>();
        _sheetAni = GetComponent<SheetAnimation>();
        _sheetAni.doIdle = false;
        _stunnedSpriteSheet = Resources.LoadAll<Sprite>("SinglePlayer/Enemies/Knocked out");
        if (!_stunnedObject)
        {
            _stunnedObject = new GameObject();
            
            _stunnedObject.transform.parent = this.transform;
            _stunnedObject.transform.localPosition = Vector2.zero;
            _stunnedObject.AddComponent<SpriteRenderer>();
            _stunnedObject.AddComponent<SheetAnimation>().doIdle = false;
            _stunnedObject.SetActive(false);
        }
        SetSpeed();
        CorrectSprite();
    }

	// Use this for initialization
	void Start () {
        Init();
	}

    public void CorrectSprite()
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
            case "SSpli":   //small splitter remains
                basePath += "Splitter/";
                walkName += "SplitterMini" + int.Parse(gameObject.name[0].ToString());
                fallName += "SplitterMini" + int.Parse(gameObject.name[0].ToString());
                break;
            default:
                basePath = null;
                break;
        }
        if (basePath == null) Debug.LogWarning("Enemy Sprite Error..");
        else
        {
            if (!walkName.StartsWith("SplitterMini"))
            {
                walkName += speed.ToString()[0];
                fallName += speed.ToString()[0];
            }
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
        if (Pauzed.IsPauzed) return;
        if(spawnInvulnerability > 0)
        {
            spawnInvulnerability -= Time.deltaTime;
            return;
        }
        if(_stunnedTimer > 0)   //TODO ALL enemies stunned first and hitcallback during stunned
        {
            _stunnedTimer -= Time.deltaTime;
            if(_stunnedTimer <= 0)
            {
                StopStunAnimation();
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
