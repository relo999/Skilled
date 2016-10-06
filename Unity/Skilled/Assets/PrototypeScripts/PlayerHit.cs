using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



/// <summary>
/// rigidBodys and other components that are only present on the main character have to be checked on the clones to find the correct one
/// </summary>
public class PlayerHit : MonoBehaviour {

    public bool Respawn = true;
    public float BounceStrength = 3.0f; //NOTE: based on jump strength
    public int ScorePerKill = 1;
    bool didCollisionCheck = false;

    bool Immunity = false;
    public bool StayImmune = false; //if false, immunity goes away after 1 hit
    float ImmunityTimer = 0;
    public delegate void ImmunityCallback();
    public ImmunityCallback IC;
    public bool isClone = false;
    public SheetAnimation.PlayerColor color = SheetAnimation.PlayerColor.red;

    void Update()
    {
        didCollisionCheck = false;
        if (ImmunityTimer > 0) ImmunityTimer -= Time.deltaTime;
        else if (Immunity)
        {
            Immunity = false;
            StayImmune = false;
            OnImmunityEnd();
        }
    }

    public void SetImmunity(bool Immunity, bool StayImmune = false, float seconds = 99.0f)
    {
        this.Immunity = Immunity;
        this.StayImmune = StayImmune;
        this.ImmunityTimer = seconds;
    }
    void Start()
    {
        IC = OnImmunityEnd;
    }

    void OnImmunityEnd()
    {

    }

    public void OnDeath(GameObject other)
    {
        
        //splat
        GameObject splatObject = new GameObject("splatInstance");
        splatObject.transform.position = this.gameObject.transform.position;
        SpriteRenderer SR = splatObject.AddComponent<SpriteRenderer>();
        SR.sortingOrder = -14;  //before background but behind the rest
        SheetAnimation ani = splatObject.AddComponent<SheetAnimation>();
        LevelBounds.instance.RegisterObject(splatObject);
        ani.PlayAnimation("Splat", color, false, 1);
        /*
        //shield powerup / spawn immunity
        if(Immunity)
        {
            if (!StayImmune) Immunity = false;
            IC();
            return;
        }*/
        //score
        //Debug.Log(other.name + gameObject.name);
        int playerID = other.name.Contains("1") ? 0 : (other.name.Contains("2") ? 1 : (other.name.Contains("3") ? 2 : 3));
        int thisplayerID = gameObject.name.Contains("1") ? 0 : (gameObject.name.Contains("2") ? 1 : (gameObject.name.Contains("3") ? 2 : 3));
        if (playerID == thisplayerID) return;
        //if (playerID > ScoreManager.instance.score.Length) playerID = 1;
        int useingID = ScoreManager.instance.scoreMode == ScoreManager.ScoreMode.Health ? thisplayerID : playerID;

        ScoreManager.instance.ChangeScore(useingID, ScorePerKill);
        if (ScoreManager.instance.scoreMode == ScoreManager.ScoreMode.Health &&  ScoreManager.instance.score[useingID] <= 0)
            Respawn = false;

        

        float deathTime = 0.5f;
        PlayerMovement pm = isClone ? gameObject.transform.parent.gameObject.GetComponent<PlayerMovement>() : GetComponent<PlayerMovement>();
        pm.StunnedTimer = deathTime;
        Rigidbody2D rig = isClone ? gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>() : GetComponent<Rigidbody2D>();
        rig.gravityScale = 0;
        rig.velocity = Vector2.zero;
        PowerupUser pu = isClone? gameObject.transform.parent.gameObject.GetComponent<PowerupUser>() : GetComponent<PowerupUser>();
        pu.EndAllPowerups();
        Array.ForEach(GetComponentsInChildren<Collider2D>(), x => x.isTrigger = true);
        SheetAnimation sa = isClone ? gameObject.transform.parent.gameObject.GetComponent<SheetAnimation>() : GetComponent<SheetAnimation>();
        sa.PlayAnimation("Death", color, false, 5.0f/ deathTime);
        StartCoroutine(DelayedRespawn(deathTime));
        /*
        if (Respawn)
        {
            PlayerMovement pm = gameObject.GetComponent<PlayerMovement>();
            if (pm == null) pm = gameObject.transform.parent.GetComponent<PlayerMovement>();
            PlayerMovement.Controls controls = pm.controls;
            pm.gameObject.transform.position = SpawnManager.instance.GetRandomSpawnPoint();
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }*/
        
    }

    IEnumerator DelayedRespawn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (Respawn)
        {
            PlayerMovement pm = gameObject.GetComponent<PlayerMovement>();
            if (pm == null) pm = gameObject.transform.parent.GetComponent<PlayerMovement>();

            pm.gameObject.transform.position = SpawnManager.instance.GetRandomSpawnPoint();

            Rigidbody2D rig = isClone ? gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>() : GetComponent<Rigidbody2D>();
            //animations, immunity and stopping movement during that
            rig.gravityScale = 1;
            Array.ForEach(GetComponentsInChildren<Collider2D>(), x => x.isTrigger = false);
            SheetAnimation ani = isClone ? gameObject.transform.parent.gameObject.GetComponent<SheetAnimation>() : GetComponent<SheetAnimation>();
            float spawnTime = 0.5f;
            ani.PlayAnimation("Spawn", color, false, 8.0f / spawnTime);
            SetImmunity(true, true, spawnTime * 2.0f);  //extra immunity after spawning (*2.0f)
            pm.StunnedTimer = spawnTime;
            rig.isKinematic = true;
            StartCoroutine(SpawnCallback(spawnTime));
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    IEnumerator SpawnCallback(float delay)
    {
        yield return new WaitForSeconds(delay);
        Rigidbody2D rig = isClone ? gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>() : GetComponent<Rigidbody2D>();
        rig.isKinematic = false;
    }

    //bounces other up
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
           
            //bounce
            BounceUp(other);

            //immunity, do the bounce, but don't do the death
            if (Immunity)
            {
                if (!StayImmune)
                {
                    Immunity = false;
                    OnImmunityEnd();
                }
                return;
            }

            //death
            this.OnDeath(other);
        }

        
    }
}
