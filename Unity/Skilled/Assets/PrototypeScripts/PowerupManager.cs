using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour {

    public static PowerupManager instance { private set; get; }

    public GameObject[] PowerupsPrefabs;
    void Awake()
    {
        instance = this;
    }
}
