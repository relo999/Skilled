using UnityEngine;
using System.Collections;

public class PlayerHit : MonoBehaviour {

    public bool Respawn = true;
    public float BounceStrength = 3.0f; //NOTE: based on jump strength

    public void OnDeath(GameObject cause)
    {
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
        if (c.collider.GetComponent<PlayerHit>() &&
            c.collider.gameObject.transform.position.y > transform.position.y + gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y / 2f)
        {
            Rigidbody2D rigid = c.collider.gameObject.GetComponent<Rigidbody2D>();
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            
            rigid.AddForce(Vector2.up * (gameObject.GetComponent<PlayerMovement>().JumpForce / 10f * BounceStrength));
            this.OnDeath(c.collider.gameObject);
        }
    }
}
