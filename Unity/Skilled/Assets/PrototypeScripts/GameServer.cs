using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;

public class GameServer : NetworkBase {

    UDPClient[] connectedClients;

    public override void Update()
    {
        base.Update();
        PlayerMovement[] players = GameObject.FindObjectsOfType<PlayerMovement>();
        PlayerInfo[] playerInfos = new PlayerInfo[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerInfos[i] = new PlayerInfo((int)players[i].playerID, players[i].transform.position.x, players[i].transform.position.y);
        }
        PlayerUpdates updates = new PlayerUpdates(playerInfos);
        SendPlayerUpdates(updates);
        serverClient.BeginReceive(new AsyncCallback(receive), null);
    }

    protected override void receiveCallback(IAsyncResult res)
    {
        //not used atm
    }
    public GameServer(UdpClient client) : base(client)
    {

    }
    public void StartGame(UDPClient[] clients)
    {
        connectedClients = clients;
    }
    public void SendPlayerUpdates(PlayerUpdates updates)
    {
        foreach(UDPClient client in connectedClients)
        {
            SendToClient(client, SerializeClass(updates));
        }
    }

    protected override void HandleSerializedData(SerializeBase data)
    {
        Type t = data.GetType();

        if (t.Equals(typeof(PlayerInput)))
        {
            //update corresponding player object based on input
            Debug.Log("handling player input..");
            //pseudo code:
            PlayerInput input = (PlayerInput)data;
            PlayerMovement player = Array.Find(GameObject.FindObjectsOfType<PlayerMovement>(), x => (int)x.playerID == input.playerID);
            player.DoMovement(input);
            //find player object and execute movement method... players[input.playerid].doMovement(input.xAxis, input.Jump, input.Action);
            //create new PlayerUpdates(input.playerid, new playerInfo(players[input.playerid].tranform.position.x, players[input.playerid].tranform.position.y);
            //serialize PlayerUpdates....
            //send serialized Playerupdates to all (or all other?) clients
        }


    }
}
