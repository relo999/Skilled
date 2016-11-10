using UnityEngine;
using System.Collections;

public class Tiles : MonoBehaviour {

    SpriteRenderer SR;
	// Use this for initialization
	void Start () {
        SR = GetComponent<SpriteRenderer>();
        Texture2D tex = SR.sprite.texture;
        Texture2D tex2 = new Texture2D(Screen.width, Screen.height);
        

        Color32[] Patternpixels = tex.GetPixels32();
        Color32[] pixels = tex2.GetPixels32();


        for (int i = 0; i < Screen.width; i+= tex.width)
        {
            for (int j= 0; j < Patternpixels.Length/tex.height; j++)
            {
      
                pixels[i+j] = Patternpixels[j];
            }
            
        }

        tex2.filterMode = FilterMode.Point;
        tex2.SetPixels32(pixels);
        tex2.Apply();
        Sprite sprite = Sprite.Create(tex2, new Rect(0, 0, tex2.width, tex2.height), new Vector2(0.0f, 0.0f), 50.0f);

        SR.sprite = sprite;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
