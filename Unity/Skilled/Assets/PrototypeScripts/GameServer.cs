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
    int intervalS;
    PlayerMovement[] players;
    static PlayerInput[] inputs = new PlayerInput[4];
    List<UdpClient> subConnectors = new List<UdpClient>();
    List<UDPClient> Clients = new List<UDPClient>();
    static bool[] pingCallback = new bool[4];
    

    public override void Update()
    {
        base.Update();
        if (players == null)
        {
            players = GameObject.FindObjectsOfType<PlayerMovement>();
            for (int i = 0; i < NetworkBase.playerIDs.Length; i++)
            {
                for (int j = 0; j < players.Length; j++)
                {
                    players[j].OnlineGame = true;
                    if (NetworkBase.playerIDs[i] == (int)players[j].playerID) players[j].NetworkControl = true;
                }

            }
        }

        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == null) continue;
            DoPlayerInput(inputs[i]);
            inputs[i] = null;
        }



        
    }

    public override void receiveCallback(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = serverClient.EndReceive(res, ref RemoteIpEndPoint);

        string stringData = Encoding.ASCII.GetString(received);
        // string stringData = Encoding.UTF8.GetString(received);

        if (!stringData.StartsWith("<")) //testing only
            Debug.Log("received server: " + stringData);

        if (stringData.Contains("Ping")) 
        {
            for (int i = 0; i < connectedClients.Length; i++)
            {
                Debug.Log(connectedClients[i].endPoint + " : " + RemoteIpEndPoint);
                if(connectedClients[i].endPoint.Port == RemoteIpEndPoint.Port)
                {
                    pingCallback[i] = true;
                }
            }     
        }else
        HandleSerializedData(DeserializeClass(received));
        serverClient.BeginReceive(new AsyncCallback(receiveCallback), null);

    }
    public GameServer(UdpClient client) : base(client)
    {
        intervalS = (int)(1.0f / TickRate);
        intervalMS = (int)(intervalS * 1000.0f);

        


    }
    public void StartGame(UDPClient[] clients)
    {
        Debug.Log("started game");
        connectedClients = clients;
        connectedClient = clients[0];   //TODO testing only
        //updateTimer = new Timer(UpdateServer, null, intervalMS, Timeout.Infinite);
        isReady = true;
        //players = GameObject.FindObjectsOfType<PlayerMovement>();
        serverClient.BeginReceive(new AsyncCallback(receiveCallback), null);
    }


    
    //update loop at constant rate
    //used to push game data 
    public IEnumerator UpdateServer()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalS);

            PlayerInfo[] playerInfos = new PlayerInfo[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                Vector2 vel = players[i].gameObject.GetComponent<Rigidbody2D>().velocity;
                playerInfos[i] = new PlayerInfo((int)players[i].playerID, players[i].transform.position.x, players[i].transform.position.y, vel.x, vel.y);
            }
            PlayerUpdates updates = new PlayerUpdates(playerInfos);
            SendPlayerUpdates(updates);

            for (int i = 0; i < pingCallback.Length; i++)
            {
                if (pingCallback[i])
                {
                    pingCallback[i] = false;
                    byte[] pingcallback = UDPClient.StringToBytes("PingResult");
                    SendToClient(connectedClients[i], pingcallback);
                    SendToClient(connectedClients[i], pingcallback);
                    Debug.Log("sent pingresult");
                }
            }
            //Debug.Log("updating server");
            UpdateServer();
        }
    }

    //part of the gamestate, player information(position, velocity, powerups)
    public void SendPlayerUpdates(PlayerUpdates updates)
    {
        //Debug.Log("sending player updates...");
        foreach(UDPClient client in connectedClients)
        {
            SendToClient(client, SerializeClass(updates));
        }
    }

    protected void DoPlayerInput(PlayerInput input)
    {
        //update corresponding player object based on input
        PlayerMovement player = Array.Find(players, x => (int)x.playerID == input.playerID);
        player.DoMovement(input);
    }

    protected override void HandleSerializedData(SerializeBase data)
    {
        Type t = data.GetType();

        if (t.Equals(typeof(PlayerInput)))
        {
            PlayerInput input = (PlayerInput)data;
            inputs[input.playerID] = input;
            //find player object and execute movement method... players[input.playerid].doMovement(input.xAxis, input.Jump, input.Action);
            //create new PlayerUpdates(input.playerid, new playerInfo(players[input.playerid].tranform.position.x, players[input.playerid].tranform.position.y);
            //serialize PlayerUpdates....
            //send serialized Playerupdates to all (or all other?) clients
        }


    }
}
