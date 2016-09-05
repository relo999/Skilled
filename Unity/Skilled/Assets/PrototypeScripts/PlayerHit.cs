using UnityEngine;
using System.Collections;

public class PlayerHit : MonoBehaviour {

    public bool Respawn = true;

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.gameObject.transform.position.y >= transform.position.y + gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y/2f)
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
    }
}
