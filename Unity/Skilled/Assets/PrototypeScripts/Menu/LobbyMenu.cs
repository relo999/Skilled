using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using TeamUtility.IO;
using UnityEngine.UI;







public class LobbyMenu : MonoBehaviour {

    string sceneName = null;    //current active scene
    int localPlayers = 0;
    bool[] playersConnected;
    bool inMenu = true;

    MenuOptions menuOptions;

    public static LobbyMenu Instance;
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
        playersConnected = new bool[4];
        Instance = this;
        for (int i = 0; i < 4; i++) //max 4 players
        {
            if(InputManager.GetAxis("Horizontal", (PlayerID)i) != 0 || InputManager.GetAxis("Vertical", (PlayerID)i) != 0)
            {
                ChangeAxis((PlayerID)i);
            }
        }
        menuOptions = new MenuOptions();
    }





    void ChangeAxis(PlayerID playerID)
    {
        AxisConfiguration button = InputManager.GetAxisConfiguration((PlayerID)playerID, "Horizontal");
        button.axis = 7;

        AxisConfiguration button2 = InputManager.GetAxisConfiguration((PlayerID)playerID, "Vertical");
        button2.sensitivity *= -1.0f;   //some controllers have inverted axis for some reason
        button2.axis = 8;

        ControllerBind bindings = FindObjectOfType<ControllerBind>();
        bindings.ChangeButton(playerID, "Jump", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (int)(playerID + 1) + "Button" + 0));
        bindings.ChangeButton(playerID, "Action", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (int)(playerID + 1) + "Button" + 2));
    }


    void AddPlayer()
    {
        KeyCode pressed = GetButtonPressed();

        if (pressed == KeyCode.None) return;

        int playerid = -1;
        int.TryParse(pressed.ToString()[8].ToString(), out playerid);   //expected keycode is 'JoystickXButtonY' where X is the controller number
        if (playerid == -1) return; //keyboard or other not supported input
        playerid -= 1;  //joystick count starts at 1 instead of 0
        if (playersConnected[playerid] == false)    //new player is ready
        {
            playersConnected[playerid] = true;
            Debug.Log("player " + playerid + " is ready");
            GameObject mouseObject = new GameObject("Mouse_obj_P:" + playerid);
            MouseController MC = mouseObject.AddComponent<MouseController>();
            MC.SetColor(playerid, (SheetAnimation.PlayerColor)playerid);
        }
    }


    KeyCode GetButtonPressed()
    {
        for (int i = 1; i < 4; i++)  //4 max controllers plugged in, starts at 1
        {
            for (int j = 0; j < 20; j++)    //20 max joystick buttons, starts at 1
            {
                KeyCode key = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + i + "Button" + j);
                if (Input.GetKey(key))
                {
                    return key;
                }
            }
        }
        return KeyCode.None;
    }


    // Update is called once per frame
    void Update () {
        CheckSceneSwitch();
        if(inMenu)
            AddPlayer();
    }

    public void ActivateButton(Button button)
    {
        if (!inMenu) return;
        switch(button.name)
        {
            case "StartGame":
                StartGame();
                break;
            case "options?":
                //... set menuOptions.maxscore etc
                break;
        }
    }

    void CheckSceneSwitch()
    {
        string newScene = SceneManager.GetActiveScene().name;
        if (sceneName != newScene)
        {
            // New scene has been loaded
            if (sceneName != null)   //first scene switch is to the menu itself
            {
                Debug.Log(sceneName + " : " + newScene);
                FindObjectOfType<SpawnManager>().SetPlayers(playersConnected);
                ScoreManager scoreM = FindObjectOfType<ScoreManager>();
                scoreM.MaxScore = menuOptions.MaxScore;
                scoreM.StartingLives = menuOptions.StartingLives;
                scoreM.scoreMode = menuOptions.scoreMode;
                scoreM.Initialize();
                sceneName = newScene;
                Debug.Log(sceneName + " :2 " + newScene);
            }

            sceneName = newScene;
        }
    }

    void StartGame()
    {
        inMenu = false;
        SceneManager.LoadScene("prototypeScene2");
    }

    int PlayerCount()
    {
        int players = 0;
        foreach(bool player in playersConnected)
        {
            if (player) players++;
        }
        return players;
    }

    private class MenuOptions
    {
        public int MaxScore = 10;
        public int StartingLives = 3;
        public ScoreManager.ScoreMode scoreMode = ScoreManager.ScoreMode.Points;
        public MenuOptions() { }
    }

}
