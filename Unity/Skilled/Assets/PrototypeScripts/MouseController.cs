﻿using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {

    int playerid;
    Button[] buttons;

    public void SetColor(int playerid, SheetAnimation.PlayerColor color)
    {
        this.playerid = playerid;
        SpriteRenderer spriteR = gameObject.AddComponent<SpriteRenderer>();
        spriteR.sprite = Resources.Load<Sprite>("Menu/Cursor_" + color.ToString().ToUpper()[0]);
        buttons = FindObjectsOfType<Button>();
        spriteR.sortingOrder = 5;
    }

	
	// Update is called once per frame
	void Update () {
        float xChange = InputManager.GetAxis("Horizontal", (PlayerID)playerid);
        float yChange = InputManager.GetAxis("Vertical", (PlayerID)playerid);
        transform.position += new Vector3((xChange < 0 ? -1 : xChange > 0 ? 1 : 0) * Time.deltaTime * 5.0f, (yChange < 0 ? 1 : yChange > 0 ? -1 : 0) * Time.deltaTime * 5.0f, 0);
        if(InputManager.GetButtonDown("Jump", (PlayerID)playerid) || InputManager.GetButtonDown("Menu", (PlayerID)playerid))
        {
            ButtonPress();
        }
        
	}


    public void ButtonPress()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            
            Button button = buttons[i];
            //if (button.name != "Mode") continue;
            
            //Texture buttonImage = button.gameObject.GetComponent<Image>().mainTexture;
            //Debug.Log(button.gameObject.GetComponent<RectTransform>().rect.width * button.gameObject.GetComponent<RectTransform>().localScale.x);
            //Debug.Log(transform.position.x + " : " + button.transform.position.x + button.gameObject.GetComponent<RectTransform>().rect.width/2.0f);

            RectTransform RT = button.gameObject.GetComponent<RectTransform>();
            float correctedWidth = RT.rect.width * RT.localScale.x * 0.5f;
            float correctedheight = RT.rect.height * RT.localScale.y * 0.5f;
            if (transform.position.x < button.transform.position.x + correctedWidth &&   //point of mouse pointer is inside button bounds
                transform.position.x > button.transform.position.x - correctedWidth &&
                transform.position.y < button.transform.position.y + correctedheight &&
                transform.position.y > button.transform.position.y - correctedheight)
            {
                LobbyMenu.Instance.ActivateButton(button, transform.position.x);
            }
        }
    }



}
