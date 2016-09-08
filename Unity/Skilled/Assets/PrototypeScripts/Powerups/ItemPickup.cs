using UnityEngine;
using System.Collections;

public abstract class ItemPickup : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.gameObject.GetComponent<PlayerHit>())
        {
            Pickup(c.collider.gameObject);
            GameObject.Destroy(gameObject);
        }
    }

    protected abstract void Pickup(GameObject player);

}
