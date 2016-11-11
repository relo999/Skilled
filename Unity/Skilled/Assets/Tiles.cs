using UnityEngine;
using System.Collections;

public class Tiles : MonoBehaviour {

    SpriteRenderer SR;
    public int width = 50;
    public int height = 30;
	// Use this for initialization
	void Start () {
        /*
        SR = GetComponent<SpriteRenderer>();
        Texture2D tex = SR.sprite.texture;
        Texture2D tex2 = new Texture2D(tex.width * width, tex.height * height);
        

        Color32[] Patternpixels = tex.GetPixels32();
        Color32[] pixels = tex2.GetPixels32();

        SR.material.mainTextureScale = new Vector2(0.1f, 0.1f);
        SR.sprite.texture.wrapMode = TextureWrapMode.Repeat;
        SR.sprite.texture.Apply();
        /*
        tex2.filterMode = FilterMode.Point;
        tex2.SetPixels32(pixels);
        tex2.Apply();
        Sprite sprite = Sprite.Create(tex2, new Rect(0, 0, tex2.width, tex2.height), new Vector2(0.0f, 0.0f), 50.0f);

        SR.sprite = sprite;*/




    }

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
