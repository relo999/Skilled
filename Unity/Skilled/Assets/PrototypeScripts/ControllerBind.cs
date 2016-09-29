using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using System;

public class ControllerBind : MonoBehaviour {

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
        //InputManager.CreateAnalogAxis("MyJoystickConfig", "Vertical", 1, 1, 1.0f, 0.1f);
        InputManager.CreateButton(configName, "Jump", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick"+ (playerid+1) + "Button2"));
        InputManager.CreateButton(configName, "Action", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (playerid+1) + "Button3"));
        InputManager.SetInputConfiguration(configName, (PlayerID)playerid);
    }
    private void CreateJoystickConfiguration()
    {
        int maxPlayers = 4;
        for (int i = 0; i < maxPlayers; i++)
        {
            CreateConfig(i);
        }
        /*

        string p1 = "P1Controls";
        InputManager.CreateInputConfiguration(p1);
        InputManager.CreateAnalogAxis(p1, "Horizontal", 0, 0, 1.0f, 0.1f);
        InputManager.CreateAnalogAxis(p1, "Vertical", 0, 1, 1.0f, 0.1f);
        //InputManager.CreateAnalogAxis("MyJoystickConfig", "Vertical", 1, 1, 1.0f, 0.1f);
        InputManager.CreateButton(p1, "Jump", KeyCode.Joystick1Button2);
        InputManager.CreateButton(p1, "Action", KeyCode.Joystick1Button3);
        InputManager.SetInputConfiguration(p1, PlayerID.One);


        string p2 = "P2Controls";
        InputManager.CreateInputConfiguration(p2);
        InputManager.CreateAnalogAxis(p2, "Horizontal", 1, 0, 1.0f, 0.1f);
        //InputManager.CreateAnalogAxis("MyJoystickConfig2", "Vertical", 2, 1, 1.0f, 0.1f);
        InputManager.CreateButton(p2, "Jump", KeyCode.Joystick2Button2);
        InputManager.CreateButton(p2, "Action", KeyCode.Joystick2Button3);

        InputManager.SetInputConfiguration(p2, PlayerID.Two);
        */
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
