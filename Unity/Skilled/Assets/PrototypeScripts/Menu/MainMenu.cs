using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    GameObject[] buttons;
    int currentButton = 0;
    GameObject buttonPointer = null;
    public Vector2 buttonPointerOffset = new Vector2(-0.5f, 0);
    ControllerBind controllerBind = null;
    float controllerStepCooldown = 0.13f;
    float currentControllerStepCooldown = 0;


    GameObject LoadButtonPointer()
    {
        GameObject pointer = new GameObject("buttonPointer");
        pointer.AddComponent<SpriteRenderer>();
        SheetAnimation animation = pointer.AddComponent<SheetAnimation>();
        animation.doIdle = false;
        return pointer;
    }

    void SetButtonPointerAnimation(char colorChar)
    {
        buttonPointer.GetComponent<SheetAnimation>().PlayAnimationUnC(Resources.LoadAll<Sprite>("Menu/Pointer_" + colorChar), true, 5);
    }

    GameObject[] FindButtons()
    {
        SpriteRenderer[] spritebuttons = System.Array.FindAll(FindObjectsOfType<SpriteRenderer>(), x => x.gameObject.name.EndsWith("0"));
        System.Array.Sort(spritebuttons, new ButtonSort());
        GameObject[] objectbuttons = new GameObject[spritebuttons.Length];
        for (int i = 0; i < spritebuttons.Length; i++)
        {
            objectbuttons[i] = spritebuttons[i].gameObject;
        }
        return objectbuttons;
    }

    void OnButtonPress()
    {
        switch (buttons[currentButton].name)
        {
            default:
                Debug.LogError("button " + buttons[currentButton].name + " does not match any action");
                break;
            case "Local_0":
                SceneLoader.LoadScene(SceneLoader.Scenes.Local);
                break;
            case "Arcade_0":
                throw new NotImplementedException();
                break;
            case "Party_0":
                throw new NotImplementedException();
                break;
            case "Online_0":
                throw new NotImplementedException();
                break;
            case "Options_0":
                throw new NotImplementedException();
                break;
            case "Exit_0":
                SceneLoader.LoadScene(SceneLoader.Scenes.StartMenu);
                break;
        }

    }

    void MoveButtonPointer(bool up)
    {
        if(controllerBind && controllerBind.LocalPlayer1Controls == PlayerMovement.Controls.CONTROLLER)
        {
            if (currentControllerStepCooldown > 0) return;
            else currentControllerStepCooldown = controllerStepCooldown;
        }
        currentButton += up ? -1 : 1;
        currentButton %= buttons.Length;
        if (currentButton < 0) currentButton += buttons.Length;
        SetButtonPointerPosition(currentButton);
    }

    void SetButtonPointerPosition(int buttonID)
    {
        SpriteRenderer spriteRenderer = buttons[buttonID].GetComponent<SpriteRenderer>();
        float xPos = spriteRenderer.sprite.bounds.center.x - spriteRenderer.sprite.bounds.size.x / 2;
        buttonPointer.transform.position = new Vector2(xPos, buttons[buttonID].transform.position.y) + buttonPointerOffset;
    }

    void HandleInput()
    {
        PlayerMovement.Controls currentControls = controllerBind? controllerBind.LocalPlayer1Controls : PlayerMovement.Controls.WASD;
  
        switch (currentControls)
        {
            default:
            case PlayerMovement.Controls.WASD:
                if (Input.GetKeyDown(KeyCode.W)) MoveButtonPointer(true);
                if (Input.GetKeyDown(KeyCode.S)) MoveButtonPointer(false);
                if (Input.GetKeyDown(KeyCode.Space)) OnButtonPress();
                break;
            case PlayerMovement.Controls.ARROWS:
                if (Input.GetKeyDown(KeyCode.UpArrow)) MoveButtonPointer(true);
                if (Input.GetKeyDown(KeyCode.DownArrow)) MoveButtonPointer(false);
                if (Input.GetKeyDown(KeyCode.L)) OnButtonPress();
                break;
            case PlayerMovement.Controls.CONTROLLER:
                float axis = TeamUtility.IO.InputManager.GetAxis("Vertical", controllerBind.LocalPlayer1ID);
                if (axis > 0) MoveButtonPointer(false);
                if (axis < 0) MoveButtonPointer(true);
                if (TeamUtility.IO.InputManager.GetButtonDown("Jump", controllerBind.LocalPlayer1ID)) OnButtonPress();
                break;
        }
    }

    void HandleControllerSteps()
    {
        if (currentControllerStepCooldown > 0) currentControllerStepCooldown -= Time.deltaTime;
    }


	void Start () {
        controllerBind = FindObjectOfType<ControllerBind>();

        buttons = FindButtons();
        buttonPointer = LoadButtonPointer();
        SetButtonPointerAnimation(((SheetAnimation.PlayerColor)(int)(controllerBind? controllerBind.LocalPlayer1ID : 0)).ToString().ToUpper()[0]);
        SetButtonPointerPosition(currentButton);
        
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
        HandleControllerSteps();
	}

    private class ButtonSort : IComparer<SpriteRenderer>
    {
        public int Compare(SpriteRenderer x, SpriteRenderer y)
        {
            if (y.gameObject.transform.position.y > x.gameObject.transform.position.y) return 1;
            if (y.gameObject.transform.position.y < x.gameObject.transform.position.y) return -1;
            return 0;
        }
    }
}


