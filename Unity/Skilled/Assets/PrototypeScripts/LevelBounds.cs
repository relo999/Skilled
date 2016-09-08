using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelBounds : MonoBehaviour {

    Bounds bounds;
    GameObject[] players;
    List<GameObject> objects;
    bool _requestFindPlayers = false;

    public static LevelBounds Instance { private set; get; }
    public void RegisterObject(GameObject g)
    {
        objects.Add(g);
    }
    public void UnRegisterObject(GameObject g)
    {
        objects.Remove(g);
    }

    void Update()
    {

        foreach(GameObject obj in objects)
        {
            //below bounds
            if (obj.transform.position.y <= bounds.center.y - bounds.size.y / 2f)
            {
                obj.transform.position += new Vector3(0, bounds.size.y, 0);
            }
            //above bounds
            if (obj.transform.position.y >= bounds.center.y + bounds.size.y / 2f)
            {
                obj.transform.position += new Vector3(0, -bounds.size.y, 0);
            }

            //left of bounds
            if (obj.transform.position.x <= bounds.center.x - bounds.size.x / 2f)
            {
                obj.transform.position += new Vector3(bounds.size.x, 0, 0);
            }
            //right of bounds
            if (obj.transform.position.x >= bounds.center.x + bounds.size.x / 2f)
            {
                obj.transform.position += new Vector3(-bounds.size.x, 0, 0);
            }
        }
        /*
        foreach(GameObject player in players)
        {
            if (_requestFindPlayers) FindPlayers();
            if(player == null)
            {
                _requestFindPlayers = true;
                continue;
            }

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
        }*/
    }

    public void FindPlayers()
    {
        PlayerHit[] hits = GameObject.FindObjectsOfType<PlayerHit>();
        players = new GameObject[hits.GetLength(0)];
        for (int i = 0; i < hits.GetLength(0); i++)
        {
            players[i] = hits[i].gameObject;
        }
        _requestFindPlayers = false;
    }

    void Awake()
    {
        //FindPlayers();
        bounds = GetComponent<BoxCollider2D>().bounds;
        objects = new List<GameObject>();
        Instance = this;
    }
	
}
