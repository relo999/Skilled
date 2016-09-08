using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class KillOnHit : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D c)
    {
        PlayerHit hit = c.collider.gameObject.GetComponent<PlayerHit>();
        if (!hit) return;

        hit.OnDeath(this.gameObject);
       
    }

}
