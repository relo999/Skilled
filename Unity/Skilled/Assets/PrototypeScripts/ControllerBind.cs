using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using System;

public class ControllerBind : MonoBehaviour {

    public PlayerID LocalPlayer1ID = PlayerID.One;
    public PlayerMovement.Controls LocalPlayer1Controls = PlayerMovement.Controls.CONTROLLER;
	// Use this for initialization
	void Awake () {
        CreateJoystickConfiguration();
    }

    private void CreateConfig(int playerid)
    {
        string configName = "P" + (playerid+1) + "Controls";
        InputManager.CreateInputConfiguration(configName);
        InputManager.CreateAnalogAxis(configName, "Horizontal", playerid, 0, 1.0f, 0.1f);
        InputManager.CreateAnalogAxis(configName, "Vertical", playerid, 1, 1.0f, 0.1f);

        //InputManager.CreateAnalogAxis(configName, "AxisSwitch", playerid, 7, 1.0f, 0.1f);

        InputManager.CreateButton(configName, "Jump", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick"+ (playerid+1) + "Button2"));
        InputManager.CreateButton(configName, "Action", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (playerid+1) + "Button3"));
        InputManager.CreateButton(configName, "Menu", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (playerid + 1) + "Button9"));
        InputManager.SetInputConfiguration(configName, (PlayerID)playerid);


    }
    private void CreateJoystickConfiguration()
    {
        if (InputManager.GetInputConfiguration("P1Controls") != null) return;
        int maxPlayers = 4;
        for (int i = 0; i < maxPlayers; i++)
        {
            CreateConfig(i);
            /*
            for (int j = 0; j < NetworkBase.playerIDs.Length; j++)
            {
                if(i == NetworkBase.playerIDs[j]) CreateConfig(i);
            }*/
            
        }
    }

    public void ChangeButton(PlayerID playerID, string buttonName, KeyCode newButton)
    {
        AxisConfiguration button = InputManager.GetAxisConfiguration(playerID, buttonName);
        button.positive = newButton;
    }

    // Update is called once per frame
    void Update () {
        //string[] joysticknames = Input.GetJoystickNames();
        //for (int i = 0; i < joysticknames.GetLength(0); i++)
        //{
        //    Debug.Log(joysticknames[i]);
        //}
        //bool input1 = Input.GetButtonDown("YButton1");
        //Debug.Log("YButton1: " + input1);
        //bool input2 = Input.GetButtonDown("YButton2");
        // Debug.Log("YButton2: " + input2);

        /*
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick 1 button " + i))
            {
                Debug.Log("joystick 1 button " + i);
            }
       
        }

    */

        //Debug.Log(InputManager.GetButtonDown("Jump", PlayerID.One));
       



    }






}
