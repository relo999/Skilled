using UnityEngine;
using System.Collections;

public class BombExplode : MonoBehaviour {

    public float BombCountdown = 3f;
    private float _currentCountdown;
    public float explosionRadius = 3f;
    public GameObject owner;
    bool started = false;
    public GameObject ExplosionSprite;
	// Use this for initialization
	void Start () {
        _currentCountdown = BombCountdown;
	}

    public void StartCountdown(GameObject owner)
    {
        started = true;
        this.owner = owner;
        //GetComponentInChildren<Animation>().Play();
    }

    void Explode()
    {
        if (!started) return;
        GameObject.Destroy(GetComponent<Rigidbody2D>());
        ExplosionSprite.SetActive(true);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        for (int i = 0; i < colliders.GetLength(0); i++)
        {
            PlayerHit hit = colliders[i].gameObject.GetComponent<PlayerHit>();
            if (hit == null) continue;
            if (hit.gameObject == owner) continue;
            if (colliders[i] is BoxCollider2D == false) continue;
            hit.OnDeath(owner);
        }
        ExplosionSprite.transform.parent = null;
        ExplosionSprite.transform.rotation = Quaternion.identity;
        DestroyAfterSeconds DAS =  ExplosionSprite.AddComponent<DestroyAfterSeconds>();
        DAS.Seconds = 0.5f;
        GameObject.Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerHit hit = other.collider.gameObject.GetComponent<PlayerHit>();
        if (hit == null) return;
        if (hit.gameObject == owner) return;
        Transform parentT = hit.gameObject.transform.parent;
        if (parentT != null && parentT.gameObject == owner) return;
        Explode();
    }
	
	// Update is called once per frame
	void Update () {
        if (!started) return;
        _currentCountdown -= Time.deltaTime;
        if(_currentCountdown <= 0)
        {
            Explode();
        }
	}
}
