using UnityEngine;
using System.Collections;

public class SpriteLayer : MonoBehaviour {


    public Layers layer;


    void Start()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr) sr.sortingOrder = (int)layer;
        else
        {
            Canvas cv = GetComponentInChildren<Canvas>();
            if (cv) cv.sortingOrder = (int)layer;
            else Debug.LogError("SpriteLayer ERROR: " + gameObject.name + " does not have a SpriteRenderer or Canvas");
        }
    }



    public enum Layers
    {
        BackgroundColor = 0,

        Background1 = 3,
        Background2 = 4,
        Background3 = 5,
        Background4 = 6,

        InkBackground1 = 9,
        InkBackground2 = 10,
        InkBackground3 = 11,

        Block1 = 14,
        Block2 = 15,

        InkPlayer1 = 18,
        InkPlayer2 = 19,
        InkPlayer3 = 20,

        Player = 23,
        
        PlayerOverlay1 = 26,
        PlayerOVerlay2 = 27,

        Pickup = 30,

        Foreground1 = 33,
        Foreground2 = 34,

        Powerup1 = 36,
        Powerup2 = 37,
        Powerup3 = 38,

        InkPlayerAlpha = 40,

        CameraEffect1 = 42,

        Hud1 = 44,
        Hud2 = 45,
        Hud3 = 46,

        CameraEffect2 = 48

        




    

        


    }

}
