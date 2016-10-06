using UnityEngine;
using System.Collections;

/// <summary>
/// displays a sprite on top of a player, used for showing the player has a powerup or special effect
/// </summary>
public class SpriteOverlay : MonoBehaviour {

    SpriteRenderer SRenderer;
    SheetAnimation SAnimation;
    string spriteName;
    void Start()
    {
        GameObject overlayObject = new GameObject("OverlayObject");
        overlayObject.transform.parent = gameObject.transform;
        SRenderer = overlayObject.AddComponent<SpriteRenderer>();
        SAnimation = overlayObject.AddComponent<SheetAnimation>();

    }

    public void SetFrame(int frame)
    {
        SAnimation.SetFrame(frame);
    }

    public void SetOffset(Vector2 offset)
    {
        transform.localPosition = offset;
    }

    public void SetSprite(string path)
    {
        spriteName = path;
        SAnimation.PlayAnimationCustom(path, GetComponent<PlayerHit>().color, 0);
    }
	

}
