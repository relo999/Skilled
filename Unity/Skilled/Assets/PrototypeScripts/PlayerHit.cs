using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHit : MonoBehaviour {

    public bool Respawn = true;
    public float BounceStrength = 3.0f; //NOTE: based on jump strength
    public int ScorePerKill = 1;
    bool didCollisionCheck = false;

    public bool Immunity = false;
    public bool StayImmune = false; //if false, immunity goes away after 1 hit
    public delegate void ImmunityCallback();
    public ImmunityCallback IC;
    public bool isClone = false;

    void Update()
    {
        didCollisionCheck = false;
    }

    public void SetImmunity(bool Immunity, bool StayImmune = false)
    {
        this.Immunity = Immunity;
        this.StayImmune = StayImmune;
    }
    void Start()
    {
        IC = OnImmunityEnd;
    }

    void OnImmunityEnd() { }

    public void OnDeath(GameObject other)
    {
        //score
        if(Immunity)
        {
            if (!StayImmune) Immunity = false;
            IC();
            return;

        }
        //Debug.Log(other.name + gameObject.name);
        int playerID = other.name.Contains("1") ? 0 : (other.name.Contains("2") ? 1 : (other.name.Contains("3") ? 2 : 3));
        int thisplayerID = gameObject.name.Contains("1") ? 0 : (gameObject.name.Contains("2") ? 1 : (gameObject.name.Contains("3") ? 2 : 3));
        if (playerID == thisplayerID) return;
        //if (playerID > ScoreManager.instance.score.Length) playerID = 1;
        int useingID = ScoreManager.instance.scoreMode == ScoreManager.ScoreMode.Health ? thisplayerID : playerID;

        ScoreManager.instance.ChangeScore(useingID, ScorePerKill);
        if (ScoreManager.instance.scoreMode == ScoreManager.ScoreMode.Health &&  ScoreManager.instance.score[useingID] <= 0)
            Respawn = false;

        if (Respawn)
            {
                PlayerMovement pm = gameObject.GetComponent<PlayerMovement>();
                if (pm == null) pm = gameObject.transform.parent.GetComponent<PlayerMovement>();
                PlayerMovement.Controls controls = pm.controls;
                transform.position = Vector3.zero;
            }
            else
            {
                GameObject.Destroy(this.gameObject);
            }
        
    }

    protected virtual void BounceUp(GameObject other)
    {
        PlayerMovement playerMov = gameObject.GetComponent<PlayerMovement>();
        if (playerMov == null)
            playerMov = gameObject.transform.parent.GetComponent<PlayerMovement>();
        Rigidbody2D rigid = other.GetComponent<Rigidbody2D>();
        if (rigid == null)
            rigid = other.transform.parent.GetComponent<Rigidbody2D>();
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * (playerMov.JumpForce / 10f * BounceStrength));
    }
    void OnCollisionEnter2D(Collision2D c)
    {
        if (didCollisionCheck) return;  //because multiple colliders are present on players
        if (c.collider is BoxCollider2D == false) return;
        if (c.collider.gameObject.layer != LayerMask.NameToLayer("PlayerHitbox")) return;

      
        GameObject other = c.collider.gameObject;

        
        Bounds thisBounds = gameObject.GetComponent<SpriteRenderer>().sprite.bounds;
        Bounds otherBounds = other.GetComponent<SpriteRenderer>().sprite.bounds;
        //Debug.Log("1: " + (other.transform.position.y /*- otherBounds.size.y / 4f*/ >= transform.position.y + thisBounds.size.y / 2f));
        //Debug.Log("2: " + (other.transform.position.x + otherBounds.size.x / 2f >= transform.position.x - thisBounds.size.x / 2f));
        //Debug.Log("3: " + (other.transform.position.x - otherBounds.size.x / 2f <= transform.position.x + thisBounds.size.x / 2f));
        if ((other.GetComponent<PlayerHit>()) &&
            other.transform.position.y  /*- otherBounds.size.y / 4f */>= transform.position.y + thisBounds.size.y / 2f &&   //TODO NEEDS WORK, more precise
            other.transform.position.x + otherBounds.size.x / 2f >= transform.position.x - thisBounds.size.x / 2f &&
            other.transform.position.x - otherBounds.size.x / 2f <= transform.position.x + thisBounds.size.x / 2f
            )
        {
            if (other.GetComponent<PlayerHit>().isClone) return;
            didCollisionCheck = true;
            Debug.Log(other.transform.position.y + " : " + transform.position.y);
            //bounce
            BounceUp(other);
            
            //death
            this.OnDeath(other);
        }

        
    }
}
