﻿using UnityEngine;
using System.Collections;

/// <summary>
/// displays a sprite on top of a player, used for showing the player has a powerup or special effect
/// </summary>
public class SpriteOverlay : MonoBehaviour {

    SpriteRenderer SRenderer;
    SheetAnimation SAnimation;
    SpriteRenderer MainRenderer;
    GameObject overlayObject;
    Sprite OldMainSprite;
    string spriteName;
    public bool flipX
    {
        set { SRenderer.flipX = value; }
    }
    
    void Awake()
    {
        overlayObject = new GameObject("OverlayObject");
       
        overlayObject.transform.parent = gameObject.transform;
        overlayObject.transform.localPosition = Vector3.zero;
        SRenderer = overlayObject.AddComponent<SpriteRenderer>();
        SAnimation = overlayObject.AddComponent<SheetAnimation>();
        SAnimation.doIdle = false;
        MainRenderer = GetComponent<SpriteRenderer>();
        
    }

    public void OnChangedSprite()
    {
        if (OldMainSprite != MainRenderer.sprite)
        {
            OldMainSprite = MainRenderer.sprite;
            //Debug.Log(MainRenderer.sprite.bounds.size.y - 0.16f);

            SetOffset(new Vector2(0, MainRenderer.sprite.bounds.size.y - 0.32f));
        }
        OldMainSprite = MainRenderer.sprite;
    }

    void Update()
    {
        
        
    }

    public void DestroySprite()
    {
        SAnimation.StopAnimation();
    }

    public void SetFrame(int frame)
    {
        SAnimation.SetFrame(frame);
    }

    public void SetOffset(Vector2 offset)
    {
        overlayObject.transform.localPosition = offset;
    }

    public void SetSprite(string path, SheetAnimation.PlayerColor color)
    {
        spriteName = path;
        SAnimation.PlayAnimationCustom(path, GetComponent<PlayerHit>().color, 0);
    }
	

}
