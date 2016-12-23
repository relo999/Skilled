using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Lobby : MonoBehaviour {

    ControllerBind controllerBind = null;

    MenuOptions menuOptions = new MenuOptions();

    GameObject buttonPointer = null;
    SheetAnimation[] buttonPointerAnimations = null;
    Vector2 buttonPointerOffset = new Vector2(-0.2f, 0.1f);            //constant ofset 
    float[] buttonPointerXOffsets = new float[2] {0f, 1.2f };    //scaled with current selected button's size
    const float ModeSelectXOffset = -1.6f;
    const float ModeSelectPointersXOffset = 0.5f;
    const float buttonPointerScale = 0.5f;
    GameObject[] buttons;
    int currentButton = 0;

    float controllerStepCooldown = 0.13f;
    float currentControllerStepCooldown = 0;


    GameObject[] ExtraButtons;
    int currentExtraButton = 0;

    public GameObject readyToPlayObject;

    LobbyPlayers _lobbyPlayers = new LobbyPlayers();


    
    [Space(10)]
    [Header("Global settings")]
    public int MinScore = 10;
    public int MaxScore = 200;
    public int ScoreStep = 10;
    [Space(10)]
    public int MinLives = 5;
    public int MaxLives = 50;
    public int LiveStep = 5;
    [Space(10)]
    public int MinTime = 60;
    public int MaxTime = 300;
    public int TimeStep = 30;

    [Space(10)]
    public GameModeDefaults[] gameModeDefaults = new GameModeDefaults[5] {new GameModeDefaults(), new GameModeDefaults(), new GameModeDefaults(), new GameModeDefaults(), new GameModeDefaults() };
   






    GameObject LoadButtonPointer()
    {
        GameObject pointer = new GameObject("buttonPointer");
        int amount = 2;
        buttonPointerAnimations = new SheetAnimation[amount];
        for (int i = 0; i < amount; i++)
        {
            GameObject pointerImage = new GameObject("buttonPointerImage" + i);
            pointerImage.transform.parent = pointer.transform;
            pointerImage.transform.localPosition += new Vector3(buttonPointerXOffsets[i], 0, 0);
            pointerImage.transform.localScale = new Vector3(buttonPointerScale, buttonPointerScale, 1);
            SpriteRenderer sRenderer = pointerImage.AddComponent<SpriteRenderer>();
            sRenderer.sortingOrder = 10;
            sRenderer.flipX = i % 2 == 0;
            SheetAnimation animation = pointerImage.AddComponent<SheetAnimation>();
            animation.doIdle = false;
            buttonPointerAnimations[i] = animation;
        }
        return pointer;
    }

    void SetButtonPointerAnimation(char colorChar)
    {
        Sprite[] spriteSheet = Resources.LoadAll<Sprite>("Menu/Pointer_" + colorChar);
        System.Array.ForEach(buttonPointerAnimations, x => x.PlayAnimationUnC(spriteSheet, true, 5));
    }



    GameObject[] FindButtons<T>() where T : Component
    {
        T[] textButtons = System.Array.FindAll(FindObjectsOfType<T>(), x => x.gameObject.name.EndsWith("0"));
        System.Array.Sort(textButtons, new ButtonSort<T>(typeof(T).Equals(typeof(SpriteRenderer))));
        GameObject[] objectbuttons = new GameObject[textButtons.Length];
        for (int i = 0; i < textButtons.Length; i++)
        {
            objectbuttons[i] = textButtons[i].gameObject;
        }
        return objectbuttons;
    }


    GameObject[] GetButtonPointers()
    {
        GameObject[] pointers = new GameObject[2];
        for (int i = 0; i < 2; i++)
        {
            pointers[i] = buttonPointer.transform.GetChild(i).gameObject;
        }
        return pointers;
    }

    void SetButtonPointerPosition(int buttonID)
    {
        if (buttonID < buttons.Length)
        {
            float xPos = buttons[buttonID].transform.position.x;
            GameObject buttonPointerRight = GetButtonPointers()[1];
            if (buttons[buttonID].name == "Mode_0")
            {
                xPos += ModeSelectXOffset;
                buttonPointerRight.transform.localPosition += new Vector3(ModeSelectPointersXOffset, 0, 0);
            }
            else if (buttonPointerRight.transform.localPosition.x == ModeSelectPointersXOffset + buttonPointerXOffsets[1]) buttonPointerRight.transform.localPosition -= new Vector3(ModeSelectPointersXOffset, 0, 0);
            buttonPointer.transform.position = new Vector2(xPos, buttons[buttonID].transform.GetChild(0).position.y) + buttonPointerOffset;
        }
        else
        {
            GameObject buttonPointerRight = GetButtonPointers()[1];
            if (buttonPointerRight.transform.localPosition.x == ModeSelectPointersXOffset + buttonPointerXOffsets[1]) buttonPointerRight.transform.localPosition -= new Vector3(ModeSelectPointersXOffset, 0, 0);
            buttonPointer.transform.position = new Vector3(999, 999);
        }
    }

    void SetActiveExtraButtonAnimation(int extraButtonID, bool active)
    {
        GameObject button = ExtraButtons[extraButtonID];
        if(active)
        {
            button.AddComponent<SheetAnimation>().PlayAnimationUnC(Resources.LoadAll<Sprite>("Menu/WindowsAndSuch/" + button.GetComponent<SpriteRenderer>().sprite.name.Substring(0, button.GetComponent<SpriteRenderer>().sprite.name.Length-2)));
        }else
        {
            button.GetComponent<SpriteRenderer>().sprite = button.GetComponent<SheetAnimation>().sprites[0];
            GameObject.Destroy(button.GetComponent<SheetAnimation>());
        }
    }

    bool TryStartGame()
    {
        if(_lobbyPlayers.EveryoneReady)
        {
            if (CountDown.Instance == null)
                gameObject.AddComponent<CountDown>();
            if (!CountDown.Instance.IsRunning)
                CountDown.Instance.StartCountDown(3, true, StartGame);

            
            
            return true;
        }
        return false;
    }

    void StartGame()
    {
        //TODO save menuOptions to ScoreManager, allow ScoreManager to use menuOptions directly
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level2");
    }

    void CheckReadyPlayers()
    {
        if(_lobbyPlayers.EveryoneReady)
        {
            readyToPlayObject.SetActive(true);
        }
        else
        {
            readyToPlayObject.SetActive(false);
        }
    }

    void MoveButtonPointer(bool up)
    {
        if (controllerBind && controllerBind.LocalPlayer1Controls == PlayerMovement.Controls.CONTROLLER)
        {
            if (currentControllerStepCooldown > 0) return;
            else currentControllerStepCooldown = controllerStepCooldown;
        }
        if(currentButton >= buttons.Length)
        {
            SetActiveExtraButtonAnimation(currentExtraButton, false);
            //stop selected extrabutton animation
        }

        currentButton += up ? -1 : 1;
        currentButton %= buttons.Length +1;
        if (currentButton < 0) currentButton += buttons.Length +1;
        if(currentButton >= buttons.Length)
        {
            SetActiveExtraButtonAnimation(currentExtraButton, true);
            //start selected extrabutton animation
        }
        else
        if(!buttons[currentButton].activeSelf)
        {
            currentButton += up ? -1 : 1;
            currentButton %= buttons.Length +1;
            if (currentButton < 0) currentButton += buttons.Length +1;
        }
        SetButtonPointerPosition(currentButton);
    }

    //min is inclusive, max is exclusive
    int ChangeOption(int currentValue, int step, int min, int max)
    {
        int newValue = currentValue + step;
        newValue %= max;
        if (newValue < min) newValue += max;
        return newValue;
    }


    void OnConfirmButton()
    {
        GameObject button;
        if (currentButton >= buttons.Length || currentButton < 0) button = ExtraButtons[currentExtraButton];
        else button = buttons[currentButton];

        switch (button.name)
        {
            default: break;
            case "ItemChoice_0":
                throw new NotImplementedException();
                break;
            case "ExitSelection_0":
                SceneLoader.LoadScene(SceneLoader.Scenes.MainMenu);
                break;
        }
    }

    void OnButtonPress(bool toRight)
    {
        GameObject button;
        if (currentButton >= buttons.Length || currentButton < 0) button = ExtraButtons[currentExtraButton];
        else button = buttons[currentButton];
        
        switch (button.name)
        {
            default: break;
            case "Mode_0":
                menuOptions.gameMode = (ScoreManager.GameMode)ChangeOption((int)menuOptions.gameMode, toRight ? 1 : -1, 0, Enum.GetNames(typeof(ScoreManager.GameMode)).Length);
                UpdateScoreMode();
                SetModeDefaults(menuOptions.gameMode);
                UpdateModeInformation();
                break;
            case "Points_0":
                menuOptions.MaxScore = ChangeOption(menuOptions.MaxScore, toRight ? ScoreStep : -ScoreStep, MinScore - ScoreStep, MaxScore + ScoreStep);
                if (menuOptions.MaxScore < MinScore && menuOptions.MaxGameTime < MinTime) menuOptions.MaxScore = ChangeOption(menuOptions.MaxScore, toRight ? ScoreStep : -ScoreStep, MinScore - ScoreStep, MaxScore + ScoreStep);
                break;
            case "Lives_0":
                menuOptions.StartingLives = ChangeOption(menuOptions.StartingLives, toRight ? LiveStep : -LiveStep, MinLives - LiveStep, MaxLives + LiveStep);
                if (menuOptions.StartingLives < MinLives && menuOptions.MaxGameTime < MinTime) menuOptions.StartingLives = ChangeOption(menuOptions.StartingLives, toRight ? LiveStep : -LiveStep, MinLives - LiveStep, MaxLives + LiveStep);
                break;
            case "Time_0":
                menuOptions.MaxGameTime = ChangeOption(menuOptions.MaxGameTime, toRight ? TimeStep : -TimeStep, MinTime - TimeStep, MaxTime);
                if (menuOptions.MaxGameTime < MinTime && ((menuOptions.scoreMode == ScoreManager.ScoreMode.Points && menuOptions.MaxScore < MinScore) || (menuOptions.scoreMode == ScoreManager.ScoreMode.Health && menuOptions.StartingLives < MinLives )))
                    menuOptions.MaxGameTime = ChangeOption(menuOptions.MaxGameTime, toRight ? TimeStep : -TimeStep, MinTime - TimeStep, MaxTime);
                break;
            case "ItemChoice_0":
            case "ExitSelection_0":
                SetActiveExtraButtonAnimation(currentExtraButton, false);
                currentExtraButton += toRight ? 1 : -1;
                currentExtraButton %= ExtraButtons.Length;
                if (currentExtraButton < 0) currentExtraButton += ExtraButtons.Length;
                SetActiveExtraButtonAnimation(currentExtraButton, true);
                break;

        }

        UpdateAllText();
    }

    void UpdateModeInformation()
    {
        string modeName;
        if (menuOptions.gameMode == ScoreManager.GameMode.BasicHealth) modeName = "Killer";
        else if (menuOptions.gameMode == ScoreManager.GameMode.BasicPoints) modeName = "Classic";
        else modeName = menuOptions.gameMode.ToString();
        GameObject.Find("ModeInfo").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/WindowsAndSuch/" + modeName);
    }

    void UpdateScoreMode()
    {
        switch (menuOptions.gameMode)
        {
            case ScoreManager.GameMode.BasicPoints:
                menuOptions.scoreMode = ScoreManager.ScoreMode.Points;
                break;
            case ScoreManager.GameMode.BasicHealth:
                menuOptions.scoreMode = ScoreManager.ScoreMode.Health;
                break;
            //case ScoreManager.GameMode.Timed:
            //    menuOptions.scoreMode = ScoreManager.ScoreMode.Points;
            //    break;
            case ScoreManager.GameMode.Chicken:
                menuOptions.scoreMode = ScoreManager.ScoreMode.Points;
                break;
            case ScoreManager.GameMode.Tag:
                menuOptions.scoreMode = ScoreManager.ScoreMode.Health;
                break;
            case ScoreManager.GameMode.Owned:
                menuOptions.scoreMode = ScoreManager.ScoreMode.Points;
                break;
            default:
                break;
        }
    }


    void SetModeDefaults(ScoreManager.GameMode gameMode)
    {
        GameModeDefaults defaults = Array.Find(gameModeDefaults, x => x.GameMode == gameMode);
        menuOptions.MaxScore = defaults.MaxScore;
        menuOptions.StartingLives = defaults.StartingLives;
        menuOptions.MaxGameTime = defaults.MaxTime;

    } 
 

    void UpdateText(GameObject button)
    {
        string newText = button.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        switch (button.name)
        {
            default: break; 
            case "Mode_0":
                if (menuOptions.gameMode == ScoreManager.GameMode.BasicHealth) newText = "Killer";
                else if (menuOptions.gameMode == ScoreManager.GameMode.BasicPoints) newText = "Classic";
                else
                newText = menuOptions.gameMode.ToString();
                break;
            case "Points_0":
                UpdateScoreMode();
                button.SetActive(menuOptions.scoreMode == ScoreManager.ScoreMode.Points);
                newText = menuOptions.MaxScore < MinScore? "_" : menuOptions.MaxScore.ToString(); 
                break;
            case "Lives_0":
                UpdateScoreMode();
                button.SetActive(menuOptions.scoreMode == ScoreManager.ScoreMode.Health);
                newText = menuOptions.StartingLives < MinLives ? "_" : menuOptions.StartingLives.ToString();
                break;
            case "Time_0":
                newText = menuOptions.MaxGameTime < MinTime ? "_" : menuOptions.MaxGameTime.ToString();  
                break;
        }
        button.transform.GetChild(0).gameObject.GetComponent<Text>().text = newText;
    }


    void UpdateAllText()
    {
        foreach(GameObject button in buttons)
        {
            UpdateText(button);
        }
    }

    void HandleInput()
    {
        PlayerMovement.Controls currentControls = controllerBind ? controllerBind.LocalPlayer1Controls : PlayerMovement.Controls.WASD;

        switch (currentControls)
        {
            default:
            case PlayerMovement.Controls.WASD:
                if (Input.GetKeyDown(KeyCode.W)) MoveButtonPointer(true);
                if (Input.GetKeyDown(KeyCode.S)) MoveButtonPointer(false);
                if (Input.GetKeyDown(KeyCode.A)) OnButtonPress(false);
                if (Input.GetKeyDown(KeyCode.D)) OnButtonPress(true);
                if (Input.GetKeyDown(KeyCode.Space)) { OnConfirmButton(); TryStartGame(); }
                break;
            case PlayerMovement.Controls.ARROWS:
                if (Input.GetKeyDown(KeyCode.UpArrow)) MoveButtonPointer(true);
                if (Input.GetKeyDown(KeyCode.DownArrow)) MoveButtonPointer(false);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) OnButtonPress(false);
                if (Input.GetKeyDown(KeyCode.RightArrow)) OnButtonPress(true);
                if (Input.GetKeyDown(KeyCode.L)) { OnConfirmButton();TryStartGame(); }
                break;
            case PlayerMovement.Controls.CONTROLLER:
                float Xaxis = TeamUtility.IO.InputManager.GetAxis("Vertical", controllerBind.LocalPlayer1ID);
                if (Xaxis > 0) MoveButtonPointer(false);
                if (Xaxis < 0) MoveButtonPointer(true);
                float Yaxis = TeamUtility.IO.InputManager.GetAxis("Horizontal", controllerBind.LocalPlayer1ID);
                if (Yaxis > 0) OnButtonPress(true);
                if (Yaxis < 0) OnButtonPress(false);
                if (TeamUtility.IO.InputManager.GetButtonDown("Jump", controllerBind.LocalPlayer1ID)) { OnConfirmButton(); TryStartGame(); }
                break;
        }
    }

    void HandleControllerSteps()
    {
        if (currentControllerStepCooldown > 0) currentControllerStepCooldown -= Time.deltaTime;
    }



    void Start () {
        menuOptions.gameMode = ScoreManager.GameMode.BasicHealth;
        buttons = FindButtons<Text>();
        ExtraButtons = FindButtons<SpriteRenderer>();
        SetModeDefaults(menuOptions.gameMode);
        UpdateAllText();
        buttonPointer = LoadButtonPointer();
        SetButtonPointerAnimation(((SheetAnimation.PlayerColor)(int)(controllerBind ? controllerBind.LocalPlayer1ID : 0)).ToString().ToUpper()[0]);
        SetButtonPointerPosition(currentButton);
        _lobbyPlayers.Initialize();
        UpdateModeInformation();
        LevelLoader.LoadLevel("Level2");    //TODO testing only
	}


	void Update () {
        if (Pauzed.IsPauzed) return;
        HandleInput();
        HandleControllerSteps();
        _lobbyPlayers.Update();
        CheckReadyPlayers();
    }




    private class ButtonSort<T> : IComparer<T> where T : Component
    {
        public bool CompareX = false;

        public ButtonSort(bool CompareX = false)
        {
            this.CompareX = CompareX;
        }
        
        public int Compare(T x, T y)
        {
            if (CompareX)
            {
                if (y.gameObject.transform.position.x < x.gameObject.transform.position.x) return 1;
                if (y.gameObject.transform.position.x > x.gameObject.transform.position.x) return -1;
                return 0;
            }
            else
            {
                if (y.gameObject.transform.position.y > x.gameObject.transform.position.y) return 1;
                if (y.gameObject.transform.position.y < x.gameObject.transform.position.y) return -1;
                return 0;
            }
        }
    }

    [Serializable]
    public struct GameModeDefaults
    {
        public ScoreManager.GameMode GameMode;
        public int MaxScore;
        public int StartingLives;
        public int MaxTime;
        public GameModeDefaults(ScoreManager.GameMode GameMode, int MaxScore, int StartingLives, int MaxTime)
        {
            this.GameMode = GameMode;
            this.MaxScore = MaxScore;
            this.StartingLives = StartingLives;
            this.MaxTime = MaxTime;
        }
    }

    private struct MenuOptions
    {
        public int MaxScore;
        public int StartingLives;
        public ScoreManager.ScoreMode scoreMode;
        public int Level;
        public ScoreManager.GameMode gameMode;
        public int MaxGameTime;
        public MenuOptions(int MaxScore = 10, int StartingLives = 3, ScoreManager.ScoreMode scoreMode = ScoreManager.ScoreMode.Points, int Level = 1, ScoreManager.GameMode gameMode = ScoreManager.GameMode.BasicPoints, int maxGameTime = -1)
        {
            this.MaxScore = MaxScore;
            this.StartingLives = StartingLives;
            this.scoreMode = scoreMode;
            this.Level = Level;
            this.gameMode = gameMode;
            this.MaxGameTime = maxGameTime;
        }
    }

}
