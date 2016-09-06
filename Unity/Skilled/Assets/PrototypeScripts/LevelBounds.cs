using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelBounds : MonoBehaviour {

    Bounds bounds;
    GameObject[] players;
   

    void Update()
    {
        foreach(GameObject player in players)
        {
            //below bounds
            if(player.transform.position.y <= bounds.center.y - bounds.size.y/2f)
            {
                player.transform.position += new Vector3(0, bounds.size.y, 0);
            }
            //above bounds
            if (player.transform.position.y >= bounds.center.y + bounds.size.y / 2f)
            {
                player.transform.position += new Vector3(0, -bounds.size.y, 0);
            }

            //left of bounds
            if (player.transform.position.x <= bounds.center.x - bounds.size.x / 2f)
            {
                player.transform.position += new Vector3(bounds.size.x, 0, 0);
            }
            //right of bounds
            if (player.transform.position.x >= bounds.center.x + bounds.size.x / 2f)
            {
                player.transform.position += new Vector3(-bounds.size.x, 0, 0);
            }
        }
    }

    public void FindPlayers()
    {
        PlayerHit[] hits = GameObject.FindObjectsOfType<PlayerHit>();
        players = new GameObject[hits.GetLength(0)];
        for (int i = 0; i < hits.GetLength(0); i++)
        {
            players[i] = hits[i].gameObject;
        }
    }

    void Start()
    {
        FindPlayers();
        bounds = GetComponent<BoxCollider2D>().bounds;
    }
	
}
