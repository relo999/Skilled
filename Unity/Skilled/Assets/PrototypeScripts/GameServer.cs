using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using System.Collections.Generic;

public class GameServer : NetworkBase {

    UDPClient[] connectedClients;
    Timer updateTimer;
    const float TickRate = 64;
    int intervalMS;
    PlayerMovement[] players;
    static List<PlayerInput> inputs = new List<PlayerInput>();
    public override void Update()
    {
        base.Update();
        if (players == null)
        {
            players = GameObject.FindObjectsOfType<PlayerMovement>();
        }

        for (int i = inputs.Count-1; i >= 0; i--)
        {
            DoPlayerInput(inputs[i]);
            inputs.RemoveAt(i);
        }


    }

    public override void receiveCallback(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = serverClient.EndReceive(res, ref RemoteIpEndPoint);

        string stringData = Encoding.UTF8.GetString(received);
        Debug.Log("received server: " + stringData);
        serverClient.BeginReceive(new AsyncCallback(receive), null);

    }
    public GameServer(UdpClient client) : base(client)
    {
        intervalMS = (int)(1.0f / TickRate * 1000.0f);
    }
    public void StartGame(UDPClient[] clients)
    {
        Debug.Log("started game");
        connectedClients = clients;
        connectedClient = clients[0];   //TODO testing only
        //updateTimer = new Timer(UpdateServer, null, intervalMS, Timeout.Infinite);
        
        //players = GameObject.FindObjectsOfType<PlayerMovement>();
        serverClient.BeginReceive(new AsyncCallback(receiveCallback), null);
    }


    
    //update loop at constant rate
    //used to push game data 
    public IEnumerator UpdateServer()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalMS / 1000.0f);

            PlayerInfo[] playerInfos = new PlayerInfo[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                playerInfos[i] = new PlayerInfo((int)players[i].playerID, players[i].transform.position.x, players[i].transform.position.y);
            }
            PlayerUpdates updates = new PlayerUpdates(playerInfos);
            SendPlayerUpdates(updates);
            UpdateServer();
        }
    }

    //part of the gamestate, player information(position, velocity, powerups)
    public void SendPlayerUpdates(PlayerUpdates updates)
    {
        Debug.Log("sending player updates...");
        foreach(UDPClient client in connectedClients)
        {
            SendToClient(client, SerializeClass(updates));
        }
    }

    protected void DoPlayerInput(PlayerInput input)
    {
        //update corresponding player object based on input
        Debug.Log("handling player input..");
        //pseudo code:
        PlayerMovement player = Array.Find(players, x => (int)x.playerID == input.playerID);
        player.DoMovement(input);
    }

    protected override void HandleSerializedData(SerializeBase data)
    {
        Debug.Log("handling input");
        Type t = data.GetType();

        if (t.Equals(typeof(PlayerInput)))
        {
            inputs.Add((PlayerInput)data);
            //find player object and execute movement method... players[input.playerid].doMovement(input.xAxis, input.Jump, input.Action);
            //create new PlayerUpdates(input.playerid, new playerInfo(players[input.playerid].tranform.position.x, players[input.playerid].tranform.position.y);
            //serialize PlayerUpdates....
            //send serialized Playerupdates to all (or all other?) clients
        }


    }
}
