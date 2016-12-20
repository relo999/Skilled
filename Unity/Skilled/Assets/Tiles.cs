using UnityEngine;
using System.Collections;

public class Tiles : MonoBehaviour {

    SpriteRenderer SR;
    public int width = 50;
    public int height = 30;
	// Use this for initialization


    SpriteRenderer sprite;

    void Awake()
    {
        // Get the current sprite with an unscaled size
        sprite = GetComponent<SpriteRenderer>();
        Vector2 spriteSize = new Vector2(sprite.bounds.size.x / transform.localScale.x, sprite.bounds.size.y / transform.localScale.y);

        // Generate a child prefab of the sprite renderer
        GameObject childPrefab = new GameObject();
        SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();
        childPrefab.transform.position = transform.position;
        childSprite.sprite = sprite.sprite;

        // Loop through and spit out repeated tiles
        GameObject child;
        for (int i = 1; i <= height; i++)
        {
            
            for (int j = 1; j <= width; j++)
            {
                child = Instantiate(childPrefab) as GameObject;
                child.GetComponent<SpriteRenderer>().material = sprite.material;
                child.transform.position = transform.position - (new Vector3(0, spriteSize.y, 0) * i) - (new Vector3(spriteSize.x, 0, 0) * j);
                child.transform.parent = transform;
            }
        }

        // Set the parent last on the prefab to prevent transform displacement
        childPrefab.transform.parent = transform;

        // Disable the currently existing sprite component since its now a repeated image
        sprite.enabled = false;
        
    }

    // Update is called once per frame
    void Update () {
	
	}
}
