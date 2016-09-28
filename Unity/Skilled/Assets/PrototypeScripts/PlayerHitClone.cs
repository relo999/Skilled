using UnityEngine;
using System.Collections;

public class PlayerHitClone : PlayerHit {

    protected override void BounceUp(GameObject other)
    {
        PlayerMovement playerMov = gameObject.GetComponent<PlayerMovement>();
        Rigidbody2D rigid = other.GetComponent<Rigidbody2D>();
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * (playerMov.JumpForce / 10f * BounceStrength));
    }
}
