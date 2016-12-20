using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using UnityEngine.SceneManagement;
using System;

public class PressAnyMenu : MonoBehaviour {

    void Start()
    {
        //if a controllers deadzone is not proper, use dpad instead
        for (int i = 0; i < 4; i++) //max 4 players
        {
            if (InputManager.GetAxis("Horizontal", (PlayerID)i) != 0 || InputManager.GetAxis("Vertical", (PlayerID)i) != 0)
            {
                ChangeAxis((PlayerID)i);
            }
        }
    }

    void ChangeAxis(PlayerID playerID)
    {
        AxisConfiguration button = InputManager.GetAxisConfiguration((PlayerID)playerID, "Horizontal");
        button.axis = 7;

        AxisConfiguration button2 = InputManager.GetAxisConfiguration((PlayerID)playerID, "Vertical");
        button2.sensitivity = -1.0f;   //some controllers have inverted axis for some reason
        button2.axis = 8;

        ControllerBind bindings = FindObjectOfType<ControllerBind>();
        bindings.ChangeButton(playerID, "Jump", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (int)(playerID + 1) + "Button" + 0));
        bindings.ChangeButton(playerID, "Action", (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (int)(playerID + 1) + "Button" + 2));
    }



    void Update()
    {
        if (Input.anyKeyDown)
        {
            SetLocalPlayer1();

            SceneLoader.LoadScene(SceneLoader.Scenes.MainMenu);
        }
    }


    void SetLocalPlayer1()
    {
        ControllerBind bind = FindObjectOfType<ControllerBind>();
        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.Space))
        {
            bind.LocalPlayer1ID = (PlayerID)SpawnManager.ConnectedControllers();
            bind.LocalPlayer1Controls = PlayerMovement.Controls.WASD;
            return;
        }

        if (Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.RightArrow) ||
            Input.GetKey(KeyCode.UpArrow) ||
            Input.GetKey(KeyCode.L))
        {
            bind.LocalPlayer1ID = (PlayerID)(1 + SpawnManager.ConnectedControllers());
            bind.LocalPlayer1Controls = PlayerMovement.Controls.ARROWS;
            return;
        }



        KeyCode joystickButton = KeyCode.None;
        for (int i = 1; i <= 4; i++)  //4 max controllers plugged in, starts at 1
        {
            for (int j = 0; j < 20; j++)    //20 max joystick buttons, starts at 1
            {
                KeyCode key = (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + i + "Button" + j);
                if (Input.GetKey(key))
                {
                    joystickButton = key;
                    break;
                }
            }
        }
        if(joystickButton != KeyCode.None)
        {
            bind.LocalPlayer1Controls = PlayerMovement.Controls.CONTROLLER;
            bind.LocalPlayer1ID = (PlayerID)(int.Parse(joystickButton.ToString()[8].ToString()) - 1);
        }



    }



}
