using UnityEngine;
using System.Collections;
using System;

public class PassThrough : ActionBlock {
    BoxCollider2D solidC;
    BoxCollider2D triggerC;
    public override void Activate(GameObject activator)
    {
        
    }

    protected override void BlockUpdate()
    {

    }

    protected override void Initialize()
    {
        BoxCollider2D[] cols = GetComponents<BoxCollider2D>();
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].isTrigger) triggerC = cols[i];
            else if (!cols[i].isTrigger) solidC = cols[i];
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {

    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if(!c.gameObject.name.Contains("Bounce"))
            Physics2D.IgnoreCollision(c, solidC, true);
    }
    void OnTriggerExit2D(Collider2D c)
    {
        if (!c.gameObject.name.Contains("Bounce"))
            Physics2D.IgnoreCollision(c, solidC, false);
    }

}
