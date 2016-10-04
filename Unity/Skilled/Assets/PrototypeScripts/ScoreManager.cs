using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance { private set; get; }

    public ScoreMode scoreMode = ScoreMode.Points;
    public enum ScoreMode
    {
        Health,
        Points
    }
    public GameObject[] itemPickups;
    private int players = 2;    //TODO get number of players in game
    SpriteRenderer[] playerSprites;
    SpriteRenderer[] hudSprites;
    [HideInInspector]
    public Text[] scoreText;    //TODO based on number of players + find textfields through script
    public Canvas canvas;
    public Vector2 HudIconPositon = new Vector2(-100, -30);
    public Vector3 SpriteScale = new Vector3(3, 3, 1);
    public Vector2 SpriteDistance = new Vector2(0, -50);
    public int[] score { private set; get; }
    bool initialized = false;

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
        scoreText = new Text[ps.Length];
        for (int i = 0; i < ps.Length; i++)
        {
            
            playerSprites[i] = ps[i].gameObject.GetComponent<SpriteRenderer>();

            GameObject sprite = new GameObject("HudSprite " + i);
            SpriteRenderer newSR = sprite.AddComponent<SpriteRenderer>();
            newSR.sortingOrder = -9;
            hudSprites[i] = newSR;

            sprite.transform.parent = canvas.transform;
            sprite.transform.position = HudIconPositon + SpriteDistance * i;
            sprite.transform.localScale = SpriteScale;

            GameObject textHolder = new GameObject("hudSprite " + i + " text");
            textHolder.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            textHolder.transform.parent = sprite.transform;           
            scoreText[i] = textHolder.AddComponent<Text>();
            scoreText[i].font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textHolder.GetComponent<RectTransform>().localPosition = new Vector3(1.5f, -1.08f, 0);
        }
        
        ScoreManager.instance = this;
        score = new int[players];


        if (scoreMode == ScoreMode.Health)
        {
            for (int i = 0; i < score.GetLength(0); i++)
            {
                score[i] = 3;
            }
        }
        UpdateScore();
    }

    void Start()
    {
        

    }

    void Update()
    {
        if (!initialized) return;
        UpdatePlayerSprites();
    }

    void UpdatePlayerSprites()
    {
        for (int i = 0; i < playerSprites.Length; i++)
        {
            hudSprites[i].sprite = playerSprites[i].sprite;
        }
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
