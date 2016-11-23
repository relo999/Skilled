using UnityEngine;
using System.Collections;

public class EnemyFall : EnemyBase
{
    bool hasFallen = false;
    bool doneFalling = false;
    float remainingFallingDistance = 0;
    float oldYPos;
    // Use this for initialization
    void Start()
    {
        Init();
        oldYPos = transform.position.y;
    }

    protected override void OnPlayerHit(PlayerHit player, bool above)
    {
        if (!above)
        {
            player.OnDeath(gameObject);
        }
    }
    protected override void HitCallback(PlayerHit player)
    {
        player.BounceUp(player.gameObject);
        if (hasFallen)
        {
            OnDeath(player);
        }
        else
        {
            hasFallen = true;

            RaycastHit2D hit;
            
            if (CheckGrounded(out hit))
            {
                Debug.Log("started falling");
                float spriteYSize = 0.48f;  //min falling distance (1.5 blocks so it will in all cases be pushed out the bottom by physics)
                remainingFallingDistance = spriteYSize;
                //TODO sprite layer BEHIND level blocks 
                System.Array.ForEach(GetComponentsInChildren<Collider2D>(), x => x.isTrigger = true);
            }
        }
    }

    bool CheckGrounded(out RaycastHit2D rayHit)
    {
        bool grounded = false;

        float spriteXSizeHalf = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f;
        //racasts checking if player is standing on a block, casts from both edges of hitbox
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f + new Vector2(spriteXSizeHalf - 0.05f, 0), -Vector2.up, 0.20f);
        rayHit = hit;
        if (hit.transform != null && hit.transform != transform)
        {
            grounded = true;
        }
        else
        {
            RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position - Vector2.up * 0.20f - new Vector2(spriteXSizeHalf - 0.05f, 0), -Vector2.up, 0.20f);
            if (hit2.transform != null && hit2.transform != transform)
            {
                grounded = true;
                rayHit = hit2;
            }
        }
        return grounded;
    }

    protected override void EnemyUpdate()
    {
        if(remainingFallingDistance > 0)
        {
            float distance = Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(oldYPos));
            remainingFallingDistance -= distance < 1 ? distance : 0;
        }
        else
        {
            if(!doneFalling && hasFallen)
            {
                Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, GetComponentInChildren<CircleCollider2D>().radius);
                if (cols.Length <= 2)
                {
                    System.Array.ForEach(GetComponentsInChildren<Collider2D>(), x => x.isTrigger = false);
                    //TODO sprite layer INFRONT level blocks again
                    doneFalling = true;
                }
                else
                {
                    remainingFallingDistance += 0.32f;
                }
            }
            
        }

        oldYPos = transform.position.y;
    }

}
