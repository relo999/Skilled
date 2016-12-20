using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour {

    GameObject[] blocks;
    GameObject[] players;
    public static SpawnManager instance;
    bool initialized = false;
    const float minSpawnDistance = 1.5f;
    public bool AutoSpawn = true;

	// Use this for initialization
	void Awake () {
        
        GameObject splatTest = new GameObject("splatTest");
        Splat splt = splatTest.AddComponent<Splat>();
        
        
       
        
    
        

        instance = this;
        GameObject[] tempBlocks = GameObject.FindGameObjectsWithTag("Walkable");
        GameObject[] tempPass = GameObject.FindGameObjectsWithTag("PassThrough");
        blocks = new GameObject[tempBlocks.Length + tempPass.Length];
        List<GameObject> listTempBlocks = new List<GameObject>();
        List<GameObject> toBeRemoved = new List<GameObject>();
        for (int i = 0; i < tempBlocks.Length; i++)
        {
            listTempBlocks.Add(tempBlocks[i]);
            if (tempBlocks[i].name.Contains("ItemBlock") || tempBlocks[i].transform.position.y > 2.0f)
            {
                toBeRemoved.Add(tempBlocks[i]);
            } 
        }
        
        for (int i = 0; i < tempPass.Length; i++)
        {
            listTempBlocks.Add(tempPass[i]);
        }
        for (int i = listTempBlocks.Count-1; i >= 0; i--)
        {
            if (listTempBlocks.Find(x => Mathf.Abs(x.transform.position.x - listTempBlocks[i].transform.position.x) < 0.1f && x.transform.position.y > listTempBlocks[i].transform.position.y && x.transform.position.y - listTempBlocks[i].transform.position.y < 0.5f) != null)
            {
                toBeRemoved.Add(listTempBlocks[i]);
            }
        }
        for (int i = toBeRemoved.Count-1; i >= 0; i--)
        {
            //if(toBeRemoved[i] != null)
            //    toBeRemoved[i].transform.position = new Vector2(999, 999);    //to visually show which blocks cannot be spawned on
            listTempBlocks.Remove(toBeRemoved[i]);
        }
        blocks = listTempBlocks.ToArray();
        NetworkBase b = new NetworkBase(new System.Net.Sockets.UdpClient());

	}



    public static int ConnectedControllers()
    {
        int controllers = 0;
        foreach(string controllerName in Input.GetJoystickNames())
        {
            /*if (controllerName != string.Empty)*/ controllers++;
        }
        return controllers;
    }

    public Vector2 GetRandomSpawnPoint(float minDistance = minSpawnDistance)
    {
        bool canSpawn = false;
        int timeout = 30;
        while (!canSpawn && timeout > 0)
        {
            timeout--;
            GameObject block = blocks[Random.Range(0, blocks.Length)];
            bool spawnAble = true;
            
            foreach (GameObject player in players)
            {
                if (player == null) continue;
                if ((block.transform.position - player.transform.position).magnitude < minDistance)
                {
                    spawnAble = false;
                    break;
                }
            }
            canSpawn = spawnAble;

            if(canSpawn) return (Vector2)block.transform.position + Vector2.up * 0.32f;
        }
        return GetRandomSpawnPoint(0.1f);   //fail safe, if no suitable spawn point is found, search for a block closer to players
    }

    public GameObject[] SetPlayers(bool[] playersready)
    {
        players = new GameObject[playersready.Length];
        for (int i = 0; i < playersready.Length; i++)
        {
            if (!playersready[i]) continue;
            GameObject p = SpawnPlayer(i);
            players[i] = p;
            float spawnTime = 0.5f;
            p.GetComponent<SheetAnimation>().PlayAnimation("Spawn", p.GetComponent<PlayerHit>().color, false, 8.0f / spawnTime);
            p.GetComponent<PlayerMovement>().StunnedTimer = spawnTime;
        }
        initialized = true;
        return players;     
    }

    public GameObject SpawnPlayer(int playerID)
    {
        GameObject player = GameObject.Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
        player.name += playerID+1;
        player.transform.position = GetRandomSpawnPoint();
        PlayerMovement mov = player.GetComponent<PlayerMovement>();
        mov.playerID = (TeamUtility.IO.PlayerID)playerID;



        int controllerCount = ConnectedControllers();
        int newControls = playerID >= controllerCount ? playerID - controllerCount + 1 : 0;
        if (newControls > 2) Debug.LogError("too many players for amount of controllers connected");
        mov.controls = (PlayerMovement.Controls)newControls;
        player.GetComponent<PlayerHit>().color = (SheetAnimation.PlayerColor)playerID;
        player.AddComponent<SheetAnimation>();
        players[playerID] = player;
        return player;
    }
	
	// Update is called once per frame
	void Update () {
        if (!initialized && AutoSpawn) SetPlayers(new bool[4] {true,true,false,false });
        if (Input.GetKeyDown(KeyCode.H)) FindObjectOfType<Splat>().RemoveSplats();  //TODO testing only
    }
}
