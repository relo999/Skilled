using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TeamUtility.IO;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuController : MonoBehaviour {

    Button[] buttons;
    int selectedButton;
    float _currentButtonCooldown = 0;
    const float _changebuttonCooldown = 0.1f;
    bool inMenu = true;
    int localPlayers = 2;
    // Use this for initialization
    string sceneName = null;


    void Start () {
        DontDestroyOnLoad(this.gameObject);
        buttons = FindObjectsOfType<Button>();
        Array.Sort(buttons, (a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
        selectedButton = 0;
	}

    void ExecuteButton(Button b)
    {
        switch (b.name)
        {
            case "Local":
                StartLocalGame();
                break;
            case "PlayerCount":
                localPlayers++;
                localPlayers = localPlayers < 1 ? 1 : localPlayers > 4 ? 1 : localPlayers;
                b.GetComponentInChildren<Text>().text = "Players: " + localPlayers;
                break;
            default:
                Debug.Log("missing button name or behaviour not made");
                break;

        }

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

        if (Input.GetKey(KeyCode.JoystickButton2) || Input.GetKey(KeyCode.Space))
        {
            Button currentButton = buttons[selectedButton];
            ExecuteButton(currentButton);
            _currentButtonCooldown = _changebuttonCooldown;
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
            if(sceneName != null)   //first scene switch is to the menu itself
                FindObjectOfType<SpawnManager>().SetPlayers(localPlayers);

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
