using UnityEngine;
using System.Collections;

/// <summary>
/// displays a sprite on top of a player, used for showing the player has a powerup or special effect
/// </summary>
public class SpriteOverlay : MonoBehaviour {

    SpriteRenderer SRenderer;

    void Start()
    {
        GameObject overlayObject = new GameObject("OverlayObject");
        overlayObject.transform.parent = gameObject.transform;
        SRenderer = overlayObject.AddComponent<SpriteRenderer>();
    }

    public void SetSprite(string path)
    {
        //SRenderer.sprite = Resources.Load
    }
	

}
