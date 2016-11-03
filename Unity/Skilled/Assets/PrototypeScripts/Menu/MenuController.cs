using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TeamUtility.IO;
using UnityEngine.SceneManagement;
//using UnityEditor;    //crashes on build, dont use this

public class MenuController : MonoBehaviour {

    Button[] buttons;
    int selectedButton;
    float _currentButtonCooldown = 0;
    const float _changebuttonCooldown = 0.1f;
    bool inMenu = true;
    int localPlayers = 2;
    public GameObject controllerMenu;
    // Use this for initialization
    string sceneName = null;

    void GetButtons()
    {
        buttons = FindObjectsOfType<Button>();   
    }

    void SortButtons()
    {
        Array.Sort(buttons, (a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
    }

    void Start () {
        DontDestroyOnLoad(this.gameObject);
        GetButtons();
        SortButtons();
        selectedButton = 0;
	}

    void ExecuteButton(Button b)
    {
        KeyCode buttonPressed = GetButtonPressed();

        switch (b.name)
        {
            case "Local":
                StartLocalGame();
                break;

                //change amount of local players
            case "PlayerCount":
                localPlayers++;
                localPlayers = localPlayers < 1 ? 1 : localPlayers > 4 ? 1 : localPlayers;
                b.GetComponentInChildren<Text>().text = "Players: " + localPlayers;
                break;

                //open rebinding menu
            case "RebindButton":
                if (buttonPressed == KeyCode.None) break;   //cant open rebinding menu with a keyboard(currently)
                bool isActive = controllerMenu.activeSelf;
                controllerMenu.SetActive(!isActive);
                if (isActive)
                {
                    GetButtons();
                    SortButtons();
                }
                for (int i = 0; i < buttons.Length; i++)
                {
                    if(buttons[i].transform.parent != controllerMenu.transform && buttons[i].transform != controllerMenu.transform && buttons[i] != b )
                        buttons[i].transform.position += Vector3.down * 500 * (isActive? -1 : 1);
                }
                if(!isActive)GetButtons();
                SortButtons();
                int playerID = buttonPressed == KeyCode.None ? -1 : int.Parse(buttonPressed.ToString()[8].ToString());
                UpdateButtonNames(playerID);
                b.GetComponentInChildren<Text>().text = isActive? "controls" : "controls player " + (buttonPressed == KeyCode.None? "Keyboard?" :  playerID.ToString());
                selectedButton = Array.FindIndex(buttons, x => x.name == "RebindButton");
                break;

                //rebinding
            case "Jump":
            case "Action":
            case "Menu":
                if (buttonPressed != KeyCode.None)
                {
                    b.GetComponentInChildren<Text>().text = b.name + ": " + buttonPressed;
                    ChangeButtonBind(b.name, buttonPressed);
                }
                break;
         

            default:
                Debug.Log("missing button name or behaviour not made");
                break;

        }

    }

    //update the text of rebinding menu buttons to corresponding in game button for each player
    void UpdateButtonNames(int playerID)
    {
        InputConfiguration config = InputManager.GetInputConfiguration("P" + playerID + "Controls");    
        foreach (Button b in buttons)
        {
            if(b.name == "Jump"|| b.name == "Action"||b.name == "Menu")
            {
                AxisConfiguration button = config.axes.Find(x => x.name == b.name);
                b.GetComponentInChildren<Text>().text = b.name + ": " + button.positive;
            }
        }
    }

    void ChangeButtonBind(string buttonName, KeyCode newButton)
    {
        InputConfiguration config = InputManager.GetInputConfiguration("P" + newButton.ToString()[8].ToString() + "Controls");
        
        AxisConfiguration button = config.axes.Find(x => x.name == buttonName);
        button.positive = newButton;
    }


    //find joystick button pressed, returns none if its a keyboard key
    KeyCode GetButtonPressed()
    {
        for (int i = 1; i < 4; i++)  //4 max controllers plugged in, starts at 1
        {
            for (int j = 0; j < 20; j++)    //20 max joystick buttons, starts at 1
            {
                KeyCode key = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + i+ "Button" + j);
                if (Input.GetKey(key))
                {
                    return key;
                }
            }
        }
        return KeyCode.None;
    }


	
    bool ChangeButton(int axis)
    {
        if (_currentButtonCooldown > 0) return false;
        _currentButtonCooldown = _changebuttonCooldown;
        selectedButton += axis;
        return true;
    }

    public void StartLocalGame()
    {
        inMenu = false;
        SceneManager.LoadScene("prototypeScene2");
        

    }

    void ConfirmButtonHandler()
    {
        Button currentButton = buttons[selectedButton];
        if (Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.Space) || currentButton.transform.parent == controllerMenu.transform)
        {
            
            ExecuteButton(currentButton);
            //_currentButtonCooldown = _changebuttonCooldown;
        }
    }



    bool AxisHandler(int axis, object[] userdata)
    {
        if(axis == 1)
        {
            float totalAxis = 0;
            for (int i = 0; i < 4; i++)
            {
                float currentAxis = InputManager.GetAxis("Vertical", (PlayerID)i);
                if (currentAxis != 0)
                {
                    totalAxis += currentAxis;
                    break;
                }
            }
            if (totalAxis != 0) ChangeButton((int)totalAxis);
            return true;
        }
        return false;
    }
	// Update is called once per frame
	void Update () {

        if (sceneName != SceneManager.GetActiveScene().name)
        {
            // New scene has been loaded
            if (sceneName != null)   //first scene switch is to the menu itself
                //FindObjectOfType<SpawnManager>().SetPlayers(localPlayers);
                Debug.LogError("spawning don't work with this old script");

            sceneName = SceneManager.GetActiveScene().name;
        }

        //only poll for menu input when actually in the main menu
        if (!inMenu) return;


        if (_currentButtonCooldown > 0)
            _currentButtonCooldown -= Time.deltaTime;
        else
        {
            //if(SpawnManager.ConnectedControllers() > 0)
                //InputManager.StartJoystickAxisScan(AxisHandler, null, 1, null, null);
        }
        

        

        //testing
        float totalAxis = 0;
        totalAxis = Input.GetKey(KeyCode.DownArrow) ? 1 : (Input.GetKey(KeyCode.UpArrow) ? -1 : 0);
        if (totalAxis != 0) ChangeButton((int)totalAxis);
        //endTesting

        if (selectedButton < 0) selectedButton = buttons.Length - 1;
        selectedButton %= buttons.Length;

        
        //InputManager.StartJoystickButtonScan(ConfirmButtonHandler, null, 1, null, null);
        //InputManager.StartJoystickButtonScan(ConfirmButtonHandler, null, 5, null, null);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.transform.localScale = i == selectedButton? new Vector3(1.3f, 1.3f, 1) : new Vector3(1,1,1);
        }
        if(_currentButtonCooldown <= 0)
            ConfirmButtonHandler();




    }
}
