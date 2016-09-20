using UnityEngine;
using System.Collections;
using TeamUtility.IO;

public class ControllerBind : MonoBehaviour {

    public enum Buttons
    {

    }
    bool setup = false;
    public bool GetButtonDown()
    {
        return false;
    }
	// Use this for initialization
	void Awake () {
        CreateJoystickConfiguration();
    }
    private void CreateJoystickConfiguration()
    {

        InputManager.CreateInputConfiguration("MyJoystickConfig");
        InputManager.CreateAnalogAxis("MyJoystickConfig", "Horizontal", 0, 0, 1.0f, 0.1f);
        InputManager.CreateAnalogAxis("MyJoystickConfig", "Vertical", 0, 1, 1.0f, 0.1f);
        InputManager.CreateButton("MyJoystickConfig", "Jump", KeyCode.JoystickButton3);

        InputManager.SetInputConfiguration("MyJoystickConfig", PlayerID.One);


        InputManager.CreateInputConfiguration("MyJoystickConfig2");
        InputManager.CreateAnalogAxis("MyJoystickConfig2", "Horizontal", 0, 0, 1.0f, 0.1f);
        InputManager.CreateAnalogAxis("MyJoystickConfig2", "Vertical", 0, 1, 1.0f, 0.1f);
        InputManager.CreateButton("MyJoystickConfig2", "Jump", KeyCode.JoystickButton3);

        InputManager.SetInputConfiguration("MyJoystickConfig2", PlayerID.Two);

        //InputManager.set
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

       // for (int i = 0; i < 20; i++)
        //{
          //  if (Input.GetKeyDown("joystick 1 button " + i))
         //   {
         //       Debug.Log("joystick 1 button " + i);
           // }
       
        //}
        //Debug.Log(InputManager.GetButtonDown("Jump", PlayerID.One));
       



    }






}
