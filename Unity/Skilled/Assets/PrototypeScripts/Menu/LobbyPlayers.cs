using UnityEngine;
using System.Collections.Generic;

public class LobbyPlayers {

    const int PLAYERS_MAX = 4;

    public GameObject[] Players { get; private set; }
    private GameObject[] _joinObjects;
    public bool EveryoneReady { get { return CheckReady(); } }
    List<int> addedPlayers = new List<int>();
    
    public void Initialize()
    {
        Players = new GameObject[PLAYERS_MAX];
        _joinObjects = new GameObject[PLAYERS_MAX];
        for (int i = 0; i < PLAYERS_MAX; i++)
        {
            _joinObjects[i] = GameObject.Find("Join_" + (i + 1));
        }
    }

    private bool CheckReady()
    {
        int readyPlayers = 0;
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] != null) readyPlayers++;
        }
        return readyPlayers >= 2;

    }

    private bool GetAvailablePlayerID(out int ID)
    {
        for (int i = 0; i < PLAYERS_MAX; i++)
        {
            if (Players[i] == null)
            {
                ID = i;
                return true;
            }
        }
        ID = -1;
        return false;
    }

    public void AddPlayer(int controlID)
    {
        if (addedPlayers.Contains(controlID)) return;
        int newID;
        if(GetAvailablePlayerID(out newID))
        {
            GameObject player = SpawnPlayer(newID, controlID);
            player.transform.position = _joinObjects[newID].transform.position;
            SpriteRenderer joinSpriteRenderer = _joinObjects[newID].GetComponent<SpriteRenderer>();
            joinSpriteRenderer.sprite = Resources.Load<Sprite>("Menu/Ready_" + joinSpriteRenderer.sprite.name[joinSpriteRenderer.sprite.name.Length-1]);
            addedPlayers.Add(controlID);
            Players[newID] = player;
        }
    }

    public void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) AddPlayer(5);

        if (Input.GetKeyDown(KeyCode.L)) AddPlayer(6);
        for (int i = 1; i < PLAYERS_MAX +1; i++)
        {
            for (int j = 1; j < 20; j++)    //20 joystick buttons
            {
                KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + i + "Button" + j);
                if (Input.GetKey(key)) AddPlayer(i);
            }
        }

    }

    private GameObject SpawnPlayer(int playerID , int controlsID)
    {
        GameObject player = GameObject.Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
        player.name += playerID + 1;
        PlayerMovement mov = player.GetComponent<PlayerMovement>();
        mov.playerID = (TeamUtility.IO.PlayerID)playerID;

        int newControls = controlsID <= 4 ? 1 : controlsID - 4;
        mov.controls = (PlayerMovement.Controls)newControls;
        player.GetComponent<PlayerHit>().color = (SheetAnimation.PlayerColor)playerID;
        player.AddComponent<SheetAnimation>();
        GameObject.Destroy(player.GetComponent<PlayerHit>());
        GameObject.Destroy(player.GetComponent<PowerupUser>());
        return player;
    }

}
