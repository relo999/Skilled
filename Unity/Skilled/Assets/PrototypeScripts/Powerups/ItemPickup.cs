using UnityEngine;
using System.Collections;

public abstract class ItemPickup : MonoBehaviour {

    public float DestroyAfterSeconds = 5.0f;
	void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.gameObject.GetComponent<PlayerHit>())
        {
            Pickup(c.collider.gameObject);
            GameObject.Destroy(gameObject);
        }
    }

    void Start()
    {
        if(!gameObject.GetComponent<DestroyAfterSeconds>())
        {
            DestroyAfterSeconds DAS = gameObject.AddComponent<DestroyAfterSeconds>();
            DAS.Seconds = DestroyAfterSeconds;
        }
           
    }

    protected abstract void Pickup(GameObject player);

}
