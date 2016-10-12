using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {

    int playerid;
    Button[] buttons;
    Vector2 pointOffset;

    public void SetColor(int playerid, SheetAnimation.PlayerColor color)
    {
        this.playerid = playerid;
        SpriteRenderer spriteR = gameObject.AddComponent<SpriteRenderer>();
        spriteR.sprite = Resources.Load<Sprite>("Menu/Cursor_" + color.ToString().ToUpper()[0]);
        buttons = FindObjectsOfType<Button>();

        pointOffset = new Vector2(spriteR.bounds.size.x - spriteR.bounds.center.x,     spriteR.bounds.size.y - spriteR.bounds.center.y);
    }

	
	// Update is called once per frame
	void Update () {
        float xChange = InputManager.GetAxis("Horizontal", (PlayerID)playerid);
        float yChange = InputManager.GetAxis("Vertical", (PlayerID)playerid);
        transform.position += new Vector3((xChange < 0 ? -1 : xChange > 0 ? 1 : 0) * Time.deltaTime * 5.0f, (yChange < 0 ? 1 : xChange > 0 ? -1 : 0) * Time.deltaTime * 5.0f, 0);
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
            Texture buttonImage = button.gameObject.GetComponent<Image>().mainTexture;
            if (pointOffset.x < button.transform.position.x + buttonImage.width / 2.0f &&   //point of mouse pointer is inside button bounds
                pointOffset.x > button.transform.position.x - buttonImage.width / 2.0f &&
                pointOffset.y < button.transform.position.x + buttonImage.width / 2.0f &&
                pointOffset.y > button.transform.position.x - buttonImage.width / 2.0)
            {
                LobbyMenu.Instance.ActivateButton(button);
            }
        }
    }



}
