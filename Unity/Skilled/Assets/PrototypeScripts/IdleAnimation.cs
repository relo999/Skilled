using UnityEngine;
using System.Collections;

public class IdleAnimation : MonoBehaviour {

    SpriteRenderer SRenderer;
    Sprite[] sprites;
    float currentSprite = 0;
    float fps = 5;
    bool looping;
    public enum PlayerColor
    {
        red,
        blue,
        yellow,
        green,
        purple
    }
    // Use this for initialization
    void Start () {
        SRenderer = GetComponent<SpriteRenderer>();


        if (sprites == null) PlayAnimation("Idle", GetComponent<PlayerHit>().color);
        

    }

    //for animation that are color independant
    public void PlayAnimationUnC(string path, bool loop = true, float fps = 5)
    {
        looping = loop;
        this.fps = fps;
        sprites = Resources.LoadAll<Sprite>(path);
    }
    public void PlayAnimation(string path, PlayerColor color, bool loop = true, float fps = 5)
    {
        string coloredPath = "Characters/"+ color.ToString() + "/" + path;
        looping = loop;
        this.fps = fps;
        sprites = Resources.LoadAll<Sprite>(coloredPath);
    }
	
	// Update is called once per frame
	void Update () {
        
        if(looping || currentSprite < sprites.Length -Time.deltaTime * fps)
            currentSprite += Time.deltaTime * fps;
        currentSprite %= sprites.Length;

        SRenderer.sprite = sprites[(int)currentSprite];
    }
}
