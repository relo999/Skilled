using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class HitAbove : MonoBehaviour
{

    List<GameObject> collidingObjects = new List<GameObject>();


    void OnCollisionEnter2D(Collision2D c)
    {
        collidingObjects.Add(c.collider.gameObject);
        PlayerHit hit = c.collider.gameObject.GetComponent<PlayerHit>();
        if (!hit) return;
        
        if (hit.gameObject.transform.position.y < this.transform.position.y)
        {
            PlayerHit[] hits = FindObjectsOfType<PlayerHit>();
            for (int i = hits.GetLength(0)-1; i >= 0; i--)
            {
                Debug.Log(hits[i].gameObject.name);
                if(GetComponent<Collider2D>().IsTouching(hits[i].gameObject.GetComponent<Collider2D>()) && hits[i].transform.position.y > transform.position.y && hits[i].gameObject.GetComponent<PlayerMovement>().Grounded)
                {
                    hits[i].OnDeath(this.gameObject);
                }
            }
          //
        
        }


    }
    void OnCollisionExit2D(Collision2D c)
    {
        collidingObjects.Remove(c.collider.gameObject);
    }
}
