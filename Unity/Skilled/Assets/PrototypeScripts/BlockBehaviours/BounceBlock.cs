using UnityEngine;
using System.Collections;

public class BounceBlock : ActionBlock {

    public float bounceForce;

    public override void Activate(GameObject activator)
    {

    }

    protected override void BlockUpdate()
    {

    }

    protected override void Initialize()
    {

    }

    void OnCollisionEnter2D(Collision2D c)
    {
        Rigidbody2D rigid = c.collider.gameObject.GetComponent<Rigidbody2D>();
       
        if (!rigid) return;
        PlayerMovement PM = rigid.gameObject.GetComponent<PlayerMovement>();
        if(PM) PM.StunnedTimer = 0.1f;
        rigid.velocity = Vector2.zero;
        rigid.AddForce((rigid.transform.position - transform.position) * bounceForce);

    }
}
