using UnityEngine;
using System.Collections;

public class PlayerHit : MonoBehaviour {

    public bool Respawn = true;
    public float BounceStrength = 3.0f; //NOTE: based on jump strength
    public int ScorePerKill = 1;
    bool didCollisionCheck = false;



    void Update()
    {
        didCollisionCheck = false;
    }

    public void OnDeath(GameObject other)
    {
        //score
        ScoreManager.instance.ChangeScore(other.name.Contains("1") ? 0 : (other.name.Contains("2") ? 1 : (other.name.Contains("3") ? 2 : 3)), ScorePerKill);
        if (Respawn)
            {
                PlayerMovement.Controls controls = gameObject.GetComponent<PlayerMovement>().controls;
                transform.position = Vector3.zero;
            }
            else
            {
                GameObject.Destroy(this.gameObject);
            }
        
    }
    void OnCollisionEnter2D(Collision2D c)
    {
        if (didCollisionCheck) return;  //because multiple colliders are present on players
        didCollisionCheck = true;

        PlayerMovement playerMov = gameObject.GetComponent<PlayerMovement>();
        GameObject other = c.collider.gameObject;
        Bounds thisBounds = gameObject.GetComponent<SpriteRenderer>().sprite.bounds;
        Bounds otherBounds = other.GetComponent<SpriteRenderer>().sprite.bounds;
        if (other.GetComponent<PlayerHit>() &&
            other.transform.position.y - otherBounds.size.y / 4f > transform.position.y + thisBounds.size.y / 2f &&   //TODO NEEDS WORK, more precise
            other.transform.position.x + otherBounds.size.x / 4f > transform.position.x - thisBounds.size.x / 2f &&
            other.transform.position.x - otherBounds.size.x / 4f < transform.position.x + thisBounds.size.x / 2f
            )
        {
            

            //bounce
            Rigidbody2D rigid = other.GetComponent<Rigidbody2D>();
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * (playerMov.JumpForce / 10f * BounceStrength));

            //death
            this.OnDeath(other);
        }

        
    }
}
