using UnityEngine;
using System.Collections;

public class SheetAnimation : MonoBehaviour {

    SpriteRenderer SRenderer;
    public Sprite[] sprites { get; private set; }
    float currentSprite = 0;
    float fps = 5;
    bool looping;
    int stopAtFrame = -1;
    string currentAnimation = "none";
    public bool doIdle = true;
    bool hasStopped = false;

    public delegate void StoppedCallback(GameObject g);
    public StoppedCallback StoppedHandler;

    public enum PlayerColor
    {
        red,
        blue,
        green,
        yellow,
        purple
    }
    // Use this for initialization
    void Start () {
        SRenderer = GetComponent<SpriteRenderer>();
        StoppedHandler = OnStoppedAnimation;
        if (gameObject.name.Contains("splat"))
        {
            StoppedHandler += FindObjectOfType<LevelBounds>().StopClonesUpdate;
        }

        if (sprites == null && doIdle) PlayAnimation("Idle", GetComponent<PlayerHit>().color);
        

    }

    public void OnStoppedAnimation(GameObject g)
    {

    }

    public void StopAnimation()
    {
        fps = 0;
        SRenderer.sprite = null;
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
        SetSprite();
    }
    public int GetFrame()
    {
        return (int)currentSprite;
    }


    public void PlayAnimationUnC(Sprite[] sprites, bool loop = true, float fps = 5)
    {
        looping = loop;
        this.fps = fps;
        this.sprites = sprites;
        if (!loop) stopAtFrame = sprites.Length - 1;
    }

    //for animation that are color independant
    public void PlayAnimationUnC(string path, bool loop = true, float fps = 5)
    {
        looping = loop;
        this.fps = fps;
        sprites = Resources.LoadAll<Sprite>(path);
        if (!loop) stopAtFrame = sprites.Length-1;
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
        if (!loop && stopAtFrame == -1) stopAtFrame = sprites.Length-1;
         currentAnimation = path;
    }

    //lastFrame -1 is default last frame, lastFrame requires 'loop' to be false
    public void PlayAnimationCustom(string path, PlayerColor color, int stayFrame)
    {
       // string coloredPath = "Characters/" + color.ToString() + "/" + path;
        this.looping = false;
        this.fps = 0;
        this.stopAtFrame = stayFrame;
        currentSprite = stayFrame;
        sprites = Resources.LoadAll<Sprite>(path + "_" + color.ToString().ToUpper()[0]);
        currentAnimation = path;
        SetFrame(stayFrame);
    }

    void SetSprite()
    {
        if (sprites.Length >= (int)currentSprite - 1 && (int)currentSprite >= 0 && SRenderer.sprite != sprites[(int)currentSprite])
            SRenderer.sprite = sprites[(int)currentSprite];
        if (doIdle)
        {
            SpriteOverlay overlay = GetComponent<SpriteOverlay>();
            if(overlay)
                overlay.OnChangedSprite();
        }
    }
    // Update is called once per frame
    void Update () {
        if (sprites == null || fps == 0) return;
        if(!looping && (int)currentSprite == stopAtFrame && !hasStopped)
        {
            hasStopped = true;
            StoppedHandler(gameObject);
        }
        if (looping || (currentSprite < sprites.Length - Time.deltaTime * fps && (stopAtFrame < 0 || currentSprite < stopAtFrame)))
        {
            currentSprite += Time.deltaTime * fps;
        }
        else if(!hasStopped)
        {
            hasStopped = true;
            StoppedHandler(gameObject);
        }
        currentSprite %= sprites.Length;
        SetSprite();
    }
}
