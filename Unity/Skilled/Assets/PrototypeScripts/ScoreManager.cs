using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TeamUtility.IO;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance { private set; get; }
    public static GameData gameData;
    float _gameTimer = 0;
    
    public enum ScoreMode
    {
        Health,
        Points
    
    }
    public ScoreMode scoreMode = ScoreMode.Points;

    public enum GameMode
    {
        BasicPoints,
        BasicHealth,
        //Timed,
        Chicken,
        Tag,
        Owned

    }
    public GameMode currentGameMode = GameMode.BasicPoints;

    public BasicMode gameMode = null;

    public GameObject[] itemPickups;
    private int players = 2;   //defaults to 2, more players are detected in initialize
    SpriteRenderer[] playerSprites;
    SpriteRenderer[] hudSprites;
    SpriteRenderer[] overlayHudSprites;
    [HideInInspector]
    public Text[] scoreText;    //TODO based on number of players + find textfields through script
    public Canvas canvas;
    public Vector2 HudIconPositon = new Vector2(-50, -30); //hud position for first player hud, other players' are based on 'spriteDistance'
    public Vector3 SpriteScale = new Vector3(3, 3, 1);
    public Vector2 SpriteDistance = new Vector2(0, -50);    //hud y distance each element is apart from one another
    public float[] score { private set; get; }
    bool initialized = false;
    Text gameTimerText;

    float maxTime = 999999;


    public int MaxScore = 10;   //TODO not used atm
    public int StartingLives = 3;

    float _afkTimer = 0;    //keeps track of seconds that passed without any players input
    float afkTimeOut = 30;  //seconds of afk until game goes back to menu

    public void SetGameMode(BasicMode mode)
    {
        gameMode = mode;
        MaxScore = mode.maxPoints;
        scoreMode = mode.scoreMode;
        StartingLives = mode.maxPoints;
        maxTime = mode.maxTime;
        
    }

    public void Initialize()
    {

        switch (currentGameMode)
        {
            case GameMode.BasicPoints:
            default:
                SetGameMode(new BasicMode());
                break;
            case GameMode.BasicHealth:
                SetGameMode(new BasicMode(ScoreMode.Health));
                break;
            //case GameMode.Timed:
            //    SetGameMode(new BasicMode(ScoreMode.Points, -1, 1, 180));
                break;
            case GameMode.Chicken:
                SetGameMode(new ChickenMode());
                break;
            case GameMode.Tag:
                SetGameMode(new TagMode());
                break;
            case GameMode.Owned:
                SetGameMode(new OwnedMode());
                break;

                

        }

        PlayerMovement[] tempps = FindObjectsOfType<PlayerMovement>();
        PlayerMovement[] ps = new PlayerMovement[4];

        for (int i = 0; i < tempps.Length; i++) //sorting based on playerID
        {
            ps[(int)tempps[i].playerID] = tempps[i];
        }

        
        if (gameMode is TagMode && !initialized)
        {
            ((TagMode)gameMode).SetTag(tempps[UnityEngine.Random.Range(0, tempps.Length)].gameObject);
        }

        initialized = true;
        gameTimerText = GameObject.Find("GameTimer").GetComponent<Text>();
        players = 4;
        playerSprites = new SpriteRenderer[4];
        hudSprites = new SpriteRenderer[4];
        overlayHudSprites = new SpriteRenderer[4];
        scoreText = new Text[4];
        for (int i = 0; i < 4; i++)
        {
            if (ps[i] == null) continue;
            playerSprites[i] = ps[i].gameObject.GetComponent<SpriteRenderer>();

            //create new object to hold the hud image
            GameObject sprite = new GameObject("HudSprite " + i);
            SpriteRenderer newSR = sprite.AddComponent<SpriteRenderer>();
            newSR.sortingOrder = -9;   
            hudSprites[i] = newSR;

            //set its position
            sprite.transform.parent = canvas.transform;
            
            sprite.transform.localScale = SpriteScale;
            sprite.transform.localPosition = HudIconPositon + SpriteDistance * i;

            //create text next to the image to display score/lives
            GameObject textHolder = new GameObject("hudSprite " + i + " text");
           
            textHolder.transform.parent = sprite.transform;
            textHolder.transform.localScale = new Vector3(1, 1, 1);
            scoreText[i] = textHolder.AddComponent<Text>();
            scoreText[i].font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textHolder.GetComponent<RectTransform>().localPosition = new Vector2(80, 0) + SpriteDistance * i * 10;

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
        score = new float[players];

        //set score to set amount of lives instead of 0 in case of health start
        if (scoreMode == ScoreMode.Health)
        {
            for (int i = 0; i < score.GetLength(0); i++)
            {
                score[i] = StartingLives;
            }
        }
        UpdateScore();
        gameData = new GameData();
        gameData.Players = tempps.Length;


        if (!CountDown.Instance) gameObject.AddComponent<CountDown>();
        if (!CountDown.Instance.IsRunning) CountDown.Instance.StartCountDown(3, true, null);

    }



    void Update()
    {
        //if (!initialized) return;
        if (!initialized) Initialize();
        UpdatePlayerSprites();
        _gameTimer += Time.deltaTime;
        if (MaxScore == -1) CheckGameOver();    //constantly check timed
        if(gameMode != null)
            gameMode.ScoreUpdate();
        gameTimerText.text = "Time: " + (maxTime - (int)_gameTimer).ToString();
        HandleAFK();
    }


    void HandleAFK()
    {
        return;//TODO remove this return..... testing only
        if (CheckAFK())
        {
            _afkTimer += Time.deltaTime;
            //TODO display timer onscreen indicating afk
            if(_afkTimer >= afkTimeOut)
            {
                //TODO back to lobby menu
                gameData.AfkEnd = true;
                gameData.Time = _gameTimer;
                //gameData.Scores = score;
                gameData.WriteToFile();
                SceneManager.LoadScene("StartScene");
            }
        }
        else _afkTimer = 0;
    }

    void UpdatePlayerSprites()
    {
        for (int i = 0; i < playerSprites.Length; i++)
        {
            if (playerSprites[i] == null) continue;
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

    public void ChangeScore(int playerID, float scoreChange)
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
        CheckGameOver();
    }

    void CheckGameOver()
    {
        int playersAlive = 0;
        int lastPlayer = -1;
        bool healthMode = scoreMode == ScoreMode.Health;

        if(_gameTimer >= maxTime)
        {
            float highestScore = 0;
            int highestID = 0;
            for (int i = 0; i < 4; i++)
            {
                if (score[i] > highestScore)
                {
                    highestScore = score[i];
                    highestID = i;
                }
            }
            OnGameOver(highestID);
        }

        for (int i = 0; i < 4; i++)
        {
            if (playerSprites[i] == null) continue;
            if (healthMode)
            {
                if (score[i] > 0)
                {
                    playersAlive++;
                    lastPlayer = i;
                }
            }
            else
            {
                if (score[i] >= MaxScore && MaxScore != -1) OnGameOver(i);
            }
        }
        if (healthMode && playersAlive == 1) OnGameOver(lastPlayer);
    }

    bool didGameOver = false;
    void OnGameOver(int winnerID)
    {
        if (didGameOver) return;
        didGameOver = true;
        float gameEndSeconds = 10;
        PlayerMovement[] players = GameObject.FindObjectsOfType<PlayerMovement>();
        for (int i = 0; i < players.Length; i++)
        {
            if ((int)players[i].playerID == winnerID) continue;
            players[i].canMove = false;

        }
        Sprite PlayerWins = Resources.Load<Sprite>("Menu/Wins2_" + ((SheetAnimation.PlayerColor)winnerID).ToString().ToUpper()[0]);
        GameObject WinSprite = new GameObject("Win sprite");
        WinSprite.transform.position += Vector3.up * 2;
        WinSprite.AddComponent<SpriteRenderer>().sprite = PlayerWins;
        gameData.Time = _gameTimer;
        gameData.Level = SceneManager.GetActiveScene().name[5];
        //gameData.Scores = score;
        gameData.WriteToFile();
        StartCoroutine(BackToMenu(gameEndSeconds));
    }

    IEnumerator BackToMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("ArcadiumPlayScreen2");
    }

    public void UpdateScore()
    {
        for (int i = 0; i < scoreText.Length; i++)
        {
            if (scoreText[i] == null) continue;
            if (score[i] > MaxScore && MaxScore != -1) score[i] = MaxScore;
            scoreText[i].text = ((int)score[i]).ToString();
        }
    }


}
