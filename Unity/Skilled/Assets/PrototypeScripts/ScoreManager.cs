using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TeamUtility.IO;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance { private set; get; }

    public ScoreMode scoreMode = ScoreMode.Points;
    public enum ScoreMode
    {
        Health,
        Points
    }
    public GameObject[] itemPickups;
    private int players = 2;   //defaults to 2, more players are detected in initialize
    SpriteRenderer[] playerSprites;
    SpriteRenderer[] hudSprites;
    SpriteRenderer[] overlayHudSprites;
    [HideInInspector]
    public Text[] scoreText;    //TODO based on number of players + find textfields through script
    public Canvas canvas;
    public Vector2 HudIconPositon = new Vector2(-100, -30); //hud position for first player hud, other players' are based on 'spriteDistance'
    public Vector3 SpriteScale = new Vector3(3, 3, 1);
    public Vector2 SpriteDistance = new Vector2(0, -50);    //hud y distance each element is apart from one another
    public int[] score { private set; get; }
    bool initialized = false;


    public int MaxScore = 10;   //TODO not used atm
    public int StartingLives = 3;

    float _afkTimer = 0;    //keeps track of seconds that passed without any players input
    float afkTimeOut = 30;  //seconds of afk until game goes back to menu

    public void Initialize()
    {
        initialized = true;
        PlayerMovement[] tempps = FindObjectsOfType<PlayerMovement>();
        PlayerMovement[] ps = new PlayerMovement[tempps.Length];

        for (int i = 0; i < tempps.Length; i++) //sorting based on playerID
        {
            ps[(int)tempps[i].playerID] = tempps[i];
        }
        players = ps.Length;
        playerSprites = new SpriteRenderer[ps.Length];
        hudSprites = new SpriteRenderer[ps.Length];
        overlayHudSprites = new SpriteRenderer[ps.Length];
        scoreText = new Text[ps.Length];
        for (int i = 0; i < ps.Length; i++)
        {
            
            playerSprites[i] = ps[i].gameObject.GetComponent<SpriteRenderer>();

            //create new object to hold the hud image
            GameObject sprite = new GameObject("HudSprite " + i);
            SpriteRenderer newSR = sprite.AddComponent<SpriteRenderer>();
            newSR.sortingOrder = -9;   
            hudSprites[i] = newSR;

            //set its position
            sprite.transform.parent = canvas.transform;
            sprite.transform.position = HudIconPositon + SpriteDistance * i;
            sprite.transform.localScale = SpriteScale;

            //create text next to the image to display score/lives
            GameObject textHolder = new GameObject("hudSprite " + i + " text");
            textHolder.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            textHolder.transform.parent = sprite.transform;           
            scoreText[i] = textHolder.AddComponent<Text>();
            scoreText[i].font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textHolder.GetComponent<RectTransform>().localPosition = new Vector3(1.5f, -1.08f, 0);

            //object to hold overlays that are also present on the player (mostly powerup indications)
            GameObject overlay = new GameObject("HudSpriteOverlay " + i);
            overlay.transform.parent = sprite.transform;
            overlay.transform.localPosition = Vector2.zero;
            overlay.transform.localScale *= sprite.transform.localScale.x;
            SpriteRenderer overSR = overlay.AddComponent<SpriteRenderer>();
            overSR.sortingOrder = -8;
            overlayHudSprites[i] = overSR;
        }
        
        ScoreManager.instance = this;
        score = new int[players];

        //set score to set amount of lives instead of 0 in case of health start
        if (scoreMode == ScoreMode.Health)
        {
            for (int i = 0; i < score.GetLength(0); i++)
            {
                score[i] = StartingLives;
            }
        }
        UpdateScore();
    }



    void Update()
    {
        if (!initialized) return;
        UpdatePlayerSprites();


        HandleAFK();
    }


    void HandleAFK()
    {
        
        if (CheckAFK())
        {
            _afkTimer += Time.deltaTime;
            //TODO display timer onscreen indicating afk
            if(_afkTimer >= afkTimeOut)
            {
                //TODO back to lobby menu
            }
        }
        else _afkTimer = 0;
    }

    void UpdatePlayerSprites()
    {
        for (int i = 0; i < playerSprites.Length; i++)
        {
            hudSprites[i].sprite = playerSprites[i].sprite;
            SpriteOverlay ov = playerSprites[i].gameObject.GetComponent<SpriteOverlay>();
            overlayHudSprites[i].gameObject.transform.localPosition = ov.currentOFfset;

            if(overlayHudSprites[i].sprite != ov.SRenderer.sprite)
                overlayHudSprites[i].sprite = ov.SRenderer.sprite;
        }
    }


    /// <summary>
    /// checks all currently configured inputs
    /// </summary>
    /// <returns> true = afk, false = not afk </returns>
    bool CheckAFK()
    {
        //controller input check
        for (int i = 0; i < 4; i++)     //max 4 players
        {
            PlayerID id = (PlayerID)i;

            //axis
            if (InputManager.GetAxis("Horizontal", id) != 0 ||
                InputManager.GetAxis("Vertical", id) != 0)
                return false;

            //buttons
            if(InputManager.GetButton("Jump", id) || InputManager.GetButton("Action", id) || InputManager.GetButton("Menu", id))
            {
                return false;
            }
        }

        //keyboard input check
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || //keyboardplayer1
           Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) ||   //keyboardplayer2
           Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.L))  //keyboard actions
            return false;

        //defaults to true
        return true;
    }

    public void ChangeScore(int playerID, int scoreChange)
    {
        //Debug.Log(playerID + " : " + scoreChange);
        if (playerID >= players)
        {
            Debug.Log("tried to add score for player that doesnt exist");
            return;
        }
        if (scoreMode == ScoreMode.Health)
            scoreChange *= -1;

        score[playerID] += scoreChange;
        UpdateScore();
    }

    public void UpdateScore()
    {
        for (int i = 0; i < scoreText.GetLength(0); i++)
        {
            scoreText[i].text = score[i].ToString();
        }
    }


}
