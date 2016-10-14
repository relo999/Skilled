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
        if(Input.anyKeyDown) SceneManager.LoadScene("ArcadiumPlayScreen2");
    }

}
