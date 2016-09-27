using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Collider2D))]
public class HitAbove : ActionBlock
{
    public override void Activate(GameObject activator)
    {
        GetComponent<Animation>().Play();
        PlayerHit[] hits = FindObjectsOfType<PlayerHit>();
       
        for (int i = hits.GetLength(0) - 1; i >= 0; i--)
        {
            if (Vector2.Distance(hits[i].transform.position, gameObject.transform.position) > 0.40f) continue;
            if (hits[i].transform.position.y > transform.position.y && hits[i].gameObject.GetComponent<PlayerMovement>().Grounded)
            {
                hits[i].OnDeath(activator);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        
        PlayerHit hit = c.collider.gameObject.GetComponent<PlayerHit>();
        if (!hit) return;
        
        if (hit.gameObject.transform.position.y < this.transform.position.y)
        {
            Activate(hit.gameObject);
        }
    }
}
