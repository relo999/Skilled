using UnityEngine;
using System.Collections;
using System;



public class SpriteColor : MonoBehaviour {

    [HideInInspector]
    public Rect spritePosition;
    
    public Palette palette;
    public enum Palette
    {
        red = 0,
        blue = 4,
        green = 8,
        yellow = 12,
        purple = 16
    }

    Color32[] colors = new Color32[20]
    {
        new Color32(68,34,34,255),
         new Color32(51,17,17,255),
          new Color32(17,0,0,255),
          new Color32(34,17,17,255),

           new Color32(51,51,85,255),
            new Color32(34,34,68,255),
             new Color32(0,0,34,255),
             new Color32(17,17,51,255),

              new Color32(34,68,34,255),
               new Color32(17,51,17,255),
                new Color32(0,17,0,255),
                new Color32(0,34,0,255),

                 new Color32(68,51,34,255),
                  new Color32(51,34,17,255),
                   new Color32(17,17,0,255),
                   new Color32(34,17,0,255),

                    new Color32(68,51,85,255),
                     new Color32(51,34,68,255),
                      new Color32(17,0,17,255),
                        new Color32(34,17,51,255)
    };
    // Use this for initialization
    void Start () {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if(Camera.main != null)
            if (Camera.main.backgroundColor != colors[(int)palette + 3]) Camera.main.backgroundColor = colors[(int)palette + 3];

        Texture2D tex = spriteRenderer.sprite.texture;
        Texture2D tex2 = new Texture2D((int)spriteRenderer.sprite.rect.width, (int)spriteRenderer.sprite.rect.height);
        Color[] temp = tex.GetPixels((int)spriteRenderer.sprite.rect.x, (int)spriteRenderer.sprite.rect.y, (int)spriteRenderer.sprite.rect.width, (int)spriteRenderer.sprite.rect.height);
        Color32[] pixels = new Color32[temp.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            pixels[i] = (Color32)temp[i];
        }


        
   
        for (int i = 0; i < pixels.Length; i++)
        {

            if (pixels[i].a == 0) continue;

            if (pixels[i].r == 68) pixels[i] = colors[(int)palette];
            else if (pixels[i].r == 51) pixels[i] = colors[(int)palette + 1];
            else if (pixels[i].r == 17) pixels[i] = colors[(int)palette + 2];


        }
        tex2.filterMode = FilterMode.Point;
        tex2.SetPixels32(0,0, (int)spriteRenderer.sprite.rect.width, (int)spriteRenderer.sprite.rect.height, pixels);
        tex2.Apply();
        Sprite sprite = Sprite.Create(tex2, new Rect(0, 0, tex2.width, tex2.height), new Vector2(0.0f, 0.0f), 50.0f);
       
        GetComponent<SpriteRenderer>().sprite = sprite;


    }
	

}
