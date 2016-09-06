using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelBounds : MonoBehaviour {

    Bounds bounds;
    GameObject[] players;
   

    void Update()
    {

    }

    public void InitPlayers()
    {
        PlayerHit[] hits = GameObject.FindObjectsOfType<PlayerHit>();
        players = new GameObject[hits.GetLength(0)];
        foreach (PlayerHit hit in hits)
        {

        }
    }

    void Start()
    {
        bounds = GetComponent<BoxCollider2D>().bounds;
    }
	
}
