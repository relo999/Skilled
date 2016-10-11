using UnityEngine;
using System.Collections;
using System;

public class SwitchActivator : ActionBlock {

    SwitchBlock[] switchBlocks;
    public SheetAnimation.PlayerColor color;
    bool hasCollided = false;   //check to make sure a switch can only occur once per frame
    // Use this for initialization
    protected override void Initialize () {
        //switchBlocks
        SwitchBlock[] allSwitchBlocks = FindObjectsOfType<SwitchBlock>();
        switchBlocks = Array.FindAll(allSwitchBlocks, x => x.color == color);


        SpriteRenderer SRenderer = GetComponent<SpriteRenderer>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Blocks/SwitchBlock");
        int spriteID = (int)color;
        switch (spriteID)
        {
            case 0:
                spriteID = 4;
                break;
            case 1:
                spriteID = 3;
                break;
            case 2:
            case 3:
                spriteID -= 1;
                break;
            case 4:
                spriteID = 0;
                break;
        }
        SRenderer.sprite = sprites[spriteID];

    }

    protected override void BlockUpdate()
    {
        hasCollided = false;
    }
	
	void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.GetComponent<PlayerMovement>() && c.collider.gameObject.transform.position.y < transform.position.y - _bounds.size.y/2f)  //player hits, ignoring other moving objects
        {
            hasCollided = true;
            foreach(SwitchBlock block in switchBlocks)
            {
                block.Switch();
            }

        }
    }

    public override void Activate(GameObject activator)
    {
        //empty
    }
}
