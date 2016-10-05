using UnityEngine;
using System.Collections;

public class SheetAnimation : MonoBehaviour {

    SpriteRenderer SRenderer;
    Sprite[] sprites;
    float currentSprite = 0;
    float fps = 5;
    bool looping;
    int stopAtFrame = -1;
    string currentAnimation = "none";
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

    public bool OnLastFrame()
    {
        return (int)currentSprite == sprites.Length - 1;
    }

    public string GetAnimation()
    {
        return currentAnimation;
    }

    /// <summary>
    /// -1 means no (or last possible) stop frame
    /// </summary>
    /// <param name="frame"></param>
    public void SetStopFrame(int frame)
    {
        stopAtFrame = frame;
    }
    public void SetFrame(int frame)
    {
        currentSprite = frame;
    }
    public int GetFrame()
    {
        return (int)currentSprite;
    }

    //for animation that are color independant
    public void PlayAnimationUnC(string path, bool loop = true, float fps = 5)
    {
        looping = loop;
        this.fps = fps;
        sprites = Resources.LoadAll<Sprite>(path);
    }
    //lastFrame -1 is default last frame, lastFrame requires 'loop' to be false
    public void PlayAnimation(string path, PlayerColor color, bool loop = true, float fps = 5, int startingFrame = 0, int lastFrame = -1)
    {
        string coloredPath = "Characters/"+ color.ToString() + "/" + path;
        this.looping = loop;
        this.fps = fps;
        this.stopAtFrame = lastFrame;
        currentSprite = startingFrame;
        sprites = Resources.LoadAll<Sprite>(coloredPath);
        currentAnimation = path;
    }
	
	// Update is called once per frame
	void Update () {
        
        if(looping || (currentSprite < sprites.Length -Time.deltaTime * fps && (stopAtFrame<0 || currentSprite < stopAtFrame )))
            currentSprite += Time.deltaTime * fps;
        currentSprite %= sprites.Length;

        SRenderer.sprite = sprites[(int)currentSprite];
    }
}
