using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour {

    public static PowerupManager instance { private set; get; }

    public GameObject[] PowerupsPrefabs;
    public Sprite[] ShieldSprites;
    public Sprite[] ExtraSprites;
    [HideInInspector]
    public Sprite[] OriginalSprites;


    void Awake()
    {
        instance = this;
        OriginalSprites = new Sprite[4];
    }
}
