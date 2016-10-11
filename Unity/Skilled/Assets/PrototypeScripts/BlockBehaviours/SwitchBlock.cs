using UnityEngine;
using System.Collections;

public class SwitchBlock : MonoBehaviour {

    Sprite activeSprite;
    Sprite inActiveSprite;
    SpriteRenderer SRenderer;
    bool isActive = true;
    public SheetAnimation.PlayerColor color;
	
    void Start()
    {
        SRenderer = GetComponent<SpriteRenderer>();
        Sprite[] actives = Resources.LoadAll<Sprite>("Blocks/SwitchBlockOut");
        Sprite[] inActives = Resources.LoadAll<Sprite>("Blocks/SwitchBlockOutline");
        int spriteID = (int)color;
        switch(spriteID)
        {
            case 0:
                spriteID = 4;
                break;
            case 1:
                spriteID = 3;
                break;
            case 2: case 3:
                spriteID -= 1;
                break;
            case 4:
                spriteID = 0;
                break;
        }
        activeSprite = actives[spriteID];
        inActiveSprite = inActives[spriteID];
        SRenderer.sprite = activeSprite;
    }

    public void Switch()
    {
        isActive = !isActive;
        gameObject.layer = isActive ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("IgnoreCollisions");
        SRenderer.sprite = isActive ? activeSprite : inActiveSprite;
    }
}
