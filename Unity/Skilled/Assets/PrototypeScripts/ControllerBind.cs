using UnityEngine;
using System.Collections;

public class ControllerBind : MonoBehaviour {

    public enum Buttons
    {

    }
    public bool GetButtonDown()
    {
        return false;
    }
	// Use this for initialization
	void Start () {
	
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

        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick 1 button " + i))
            {
                Debug.Log("joystick 1 button " + i);
            }
       
        }
        for (int i = 1; i < 20; i++)
        {
            //float axisInput = Input.GetAxis("Joy" + i + "X");
            //if (axisInput != 0)
            //{
            //    Debug.Log("joystick " + i + axisInput);
            //}
            //if(Input.GetAxis("Horizontal") != 0)
            //{
            //    Debug.Log()
            //}
        }

    }






}
