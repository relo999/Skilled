using UnityEngine;
using System.Collections;
using System;

public class CountDown : MonoBehaviour{

    const int MAX_COUNTDOWN = 3;
    const string SPRITE_PATH = "Menu/Countdown/Countdown_";
    const SpriteLayer.Layers SPRITE_LAYER = SpriteLayer.Layers.Hud3;
    readonly Vector2 OFFSET = new Vector2(0, 2.25f);

    Sprite[] _digitSprites;
    
    public static CountDown Instance = null;

    int runningCountdowns = 0;
    public bool IsRunning { get { return runningCountdowns > 0; } }

    

    public void StartCountDown(int seconds, bool freezeGame, Action callback)
    {
        StartCoroutine(DoCountDown(seconds, freezeGame, callback));
    }

    IEnumerator DoCountDown(int seconds, bool freezeGame, Action callback)
    {
        if (freezeGame) Pauzed.IsPauzed = true;
        runningCountdowns++;
        GameObject spriteHolder = new GameObject("Countdown_sprites");
        spriteHolder.transform.localPosition = OFFSET;
        SpriteRenderer digitRenderer = spriteHolder.AddComponent<SpriteRenderer>();
        spriteHolder.AddComponent<SpriteLayer>().layer = SPRITE_LAYER;
        for (int i = seconds -1; i >= 0; i--)
        {
            digitRenderer.sprite = _digitSprites[i];
            yield return new WaitForSeconds(1);
        }
        runningCountdowns--;
        if (runningCountdowns <= 0) Pauzed.IsPauzed = false;
        GameObject.Destroy(spriteHolder);
        if (callback != null)
            callback();
        yield return null;
    }

    void Awake()
    {
        Instance = this;
        LoadSprites();
    }

    void LoadSprites()
    {
        _digitSprites = new Sprite[MAX_COUNTDOWN];
        for (int i = 1; i <= MAX_COUNTDOWN; i++)
        {
            _digitSprites[i - 1] = Resources.Load<Sprite>(SPRITE_PATH + i);
        }
  
    }

    void OnDestroy()
    {
        Instance = null;
    }

	

}
