using UnityEngine;
using System.Collections;

/// <summary>
/// not in use, testing only
/// </summary>
public class PlayerHitClone : PlayerHit {

    public override void BounceUp(GameObject other, float multipl)
    {
        PlayerMovement playerMov = gameObject.GetComponent<PlayerMovement>();
        Rigidbody2D rigid = other.GetComponent<Rigidbody2D>();
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * (playerMov.JumpForce / 10f * BounceStrength * multipl));
    }
}
