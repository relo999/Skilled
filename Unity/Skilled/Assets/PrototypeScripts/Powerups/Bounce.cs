using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour {

    public GameObject owner;
    Rigidbody2D _rigid;
    public float MaxSpeed = 1f;

    void Start()
    {
        _rigid = gameObject.GetComponent<Rigidbody2D>();
        LevelBounds.Instance.RegisterObject(gameObject);
    }

    void CapSpeed(float maxSpeed)
    {
        _rigid.velocity.Normalize();
        _rigid.velocity *= maxSpeed;
    }

    void Update()
    {
        CapSpeed(MaxSpeed);
    }

    void OnDestroy()
    {
        LevelBounds.Instance.UnRegisterObject(gameObject);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        PlayerHit hit = c.collider.gameObject.GetComponent<PlayerHit>();
        if (hit == null) return;
        if (hit.gameObject == owner) return;
        hit.OnDeath(gameObject);
        GameObject.Destroy(gameObject);
    }
}
