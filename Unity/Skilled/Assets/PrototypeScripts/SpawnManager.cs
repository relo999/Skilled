﻿using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

    GameObject[] blocks;
    GameObject[] players;
    public static SpawnManager instance;
    bool initialized = false;
    const float minSpawnDistance = 3;
	// Use this for initialization
	void Awake () {
        instance = this;
        blocks = GameObject.FindGameObjectsWithTag("Walkable");
        
	}

    public static int ConnectedControllers()
    {
        int controllers = 0;
        foreach(string controllerName in Input.GetJoystickNames())
        {
            if (controllerName != string.Empty) controllers++;
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
            return (Vector2)block.transform.position + Vector2.up * 0.32f;
        }
        /*
        foreach(GameObject block in blocks)
        {
            bool canSpawn = true;
            foreach(GameObject player in players)
            {
                if (player == null) continue;
                if((block.transform.position - player.transform.position).magnitude < 2)
                {
                    canSpawn = false;
                    break;
                }
            }
            if (canSpawn) return block.transform.position;
        }*/
        return GetRandomSpawnPoint(0.1f);   //fail safe, if no suitable spawn point is found, search for a block closer to players
    }

    public void SetPlayers(int amount)
    {
        players = new GameObject[amount];
        for (int i = 0; i < amount; i++)
        {
            SpawnPlayer(i);
        }
        initialized = true;
    }

    void SpawnPlayer(int playerID)
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
        player.GetComponent<PlayerHit>().color = (IdleAnimation.PlayerColor)playerID;
        player.AddComponent<IdleAnimation>();
        players[playerID] = player;
    }
	
	// Update is called once per frame
	void Update () {
        if (!initialized) SetPlayers(2);
    }
}