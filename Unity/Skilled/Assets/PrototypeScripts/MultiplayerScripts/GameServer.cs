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
    float intervalMS;
    float intervalS;
    PlayerMovement[] players;
    static PlayerInput[] inputs = new PlayerInput[4];
    public List<UdpClient> sockets = new List<UdpClient>();
    public List<UDPClient> Clients = new List<UDPClient>();
    List<bool> connectionSucces = new List<bool>();
    static bool[] pingCallback = new bool[4];

    string lobbyName = null;




    void StartTestConnection()
    {
        Thread testThread = new Thread(new ThreadStart(TestConnection));
        testThread.Start();
    }

    void TestConnection()
    {
        Debug.Log("testing con...");
        connectionSucces.Clear();
        connectionSucces.AddRange(new bool[Clients.Count]);
        for (int i = 0; i < 3; i++)
        {
            Thread.Sleep(500);
            for (int j = 0; j < Clients.Count; j++)
            {
                UDPClient client = Clients[j];
                if (client == null) continue;
                UdpClient socket = sockets[j];
                byte[] data = Encoding.ASCII.GetBytes("TestConnection");
                socket.Send(data, data.Length, client.endPoint);
            }
   
            
        }
        Thread.Sleep(500);
        for (int i = 0; i < connectionSucces.Count; i++)
        {
            if (!connectionSucces[i])
            {
                Debug.Log("failed con.." + i);
                FailedConnection(i);
            }
            else
            {
                Debug.Log("succes con.." + i);
            }
        }
    }

    void FailedConnection(int index)
    {

        byte[] data = Encoding.ASCII.GetBytes("RequestRelay," + Clients[index].endPoint.Address + ":" + Clients[index].endPoint.Port);
        sockets[index].Send(data, data.Length, Mainserver.endPoint);
        Clients[index] = null;
    }



    public void StartLobby()
    {
        if (lobbyName == null) return;  //tried to start lobby without registering it to the main server
        byte[] data = Encoding.ASCII.GetBytes("StartLobby" + lobbyName);
        isReady = true;
        NetManager.hasStarted = true;
        SendToClient(Mainserver, data);
    }

    public void MakeLobby(string name, int players)
    {
        for (int i = 0; i < 3; i++)
        {
            Clients.Add(null);
        }
        lobbyName = name;
        byte[] data = new byte[1];
        SendToClient(new NetworkBase.UDPClient("0.0.0.9", 999), data);
        data = Encoding.ASCII.GetBytes("NewLobby" + players + name + "," + GetLocalIPAddress());
        SendToClient(Mainserver, data);
        NetworkBase.playerIDs = new int[players];
        for (int i = 0; i < players; i++)
        {
            NetworkBase.playerIDs[i] = i;
        }
        for (int i = 0; i < 3; i++)
        {
            AddLobbySocket();
        }
    }
    void AddLobbyConnection(UdpClient socket)
    {
        if(lobbyName != null)
        {
            byte[] data = Encoding.ASCII.GetBytes("AddLobbySocket" + lobbyName + "," + GetLocalIPAddress());
            socket.Send(data, data.Length, Mainserver.endPoint);
        }
    }
    public void AddLobbySocket()
    {
        UdpClient socket = new UdpClient();
        socket.BeginReceive(new AsyncCallback(receiveCallback), socket);
        sockets.Add(socket);
        AddLobbyConnection(socket);
        
    }








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
        testString = "inputs 1";
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == null) continue;
            testString = "inputs 2";
            DoPlayerInput(inputs[i]);
            inputs[i] = null;
        }



        
    }

    public override void receiveCallback(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = ((UdpClient)res.AsyncState).EndReceive(res, ref RemoteIpEndPoint);
        ((UdpClient)res.AsyncState).BeginReceive(new AsyncCallback(receiveCallback), res.AsyncState);
        //byte[] received = serverClient.EndReceive(res, ref RemoteIpEndPoint);
        string stringData = Encoding.ASCII.GetString(received);
        if (!stringData.StartsWith("<"))
            Debug.Log(stringData);
        else Debug.Log("input");
        if (stringData == "TestConnection")
        {
            
            Debug.Log("received testconnection");
            lock(connectionSucces)
                connectionSucces[sockets.FindIndex(x => x == (UdpClient)res.AsyncState)] = true;
                
            //FailedConnection(sockets.FindIndex(x => x == (UdpClient)res.AsyncState));
        }

        if(stringData.StartsWith("Failed to "))
        {
            NetManager.instance.Reset();
            return;
        }
        // string stringData = Encoding.UTF8.GetString(received);
        testFloat = 1;
        if (!isReady && stringData.Contains(":"))
        {
            string[] splitData = stringData.Split(':');
            for (int i = 0; i < sockets.Count; i++)
            {
                if (sockets[i] == (UdpClient)res.AsyncState)
                {
                    Clients[i] = new UDPClient(IPAddress.Parse(splitData[0]), int.Parse(splitData[1]));
                    break;
                }
            }
            StartTestConnection();
        }
        //if (!stringData.StartsWith("<")) //testing only
        //Debug.Log("received server: " + (stringData.StartsWith("<")? "data" : stringData));
        if (stringData == "Ping")
        {
            //Debug.Log("handling ping");
            for (int i = 0; i < connectedClients.Length; i++)
            {
                if (connectedClients[i] == null) continue;
                //Debug.Log(connectedClients[i].endPoint + " : " + RemoteIpEndPoint);
                if (connectedClients[i].endPoint.Port == RemoteIpEndPoint.Port)
                {
                    pingCallback[i] = true;
                }
            }
        }
        else
        {
            if (stringData.StartsWith("<"))
            {
                testFloat = 99199;
                Debug.Log("got input..");
                HandleSerializedData(DeserializeClass(received));

            }
        }
        //testFloat = 2;
        //Debug.Log("started receive server...");
        //((UdpClient)res.AsyncState).BeginReceive(new AsyncCallback(receiveCallback), res.AsyncState);

    }
    public GameServer(UdpClient client, int maxPlayers = 2) : base(client)
    {
        intervalS = (1.0f / TickRate);
        intervalMS = (intervalS * 1000.0f);

        


    }
    public void StartGame(UDPClient[] clients)
    {
        Debug.Log("started game");
        connectedClients = clients;
        connectedClient = clients[0];   //TODO testing only
        //updateTimer = new Timer(UpdateServer, null, intervalMS, Timeout.Infinite);
        isReady = true;
        //players = GameObject.FindObjectsOfType<PlayerMovement>();
        //serverClient.BeginReceive(new AsyncCallback(receiveCallback), null);
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
                }
            }
            //Debug.Log("updating server");
            UpdateServer();
        }
    }

    //part of the gamestate, player information(position, velocity, powerups)
    public void SendPlayerUpdates(PlayerUpdates updates)
    {
        Debug.Log("sending updates...");
        byte[] serializedData = SerializeClass(updates);
        for (int i = 0; i < Clients.Count; i++)
        {
            if (Clients[i] != null)
            {
                Debug.Log("sending updates to : " + Clients[i].endPoint.Port);
                sockets[i].Send(serializedData, serializedData.Length, Clients[i].endPoint);
            }
        }
        /*
        //Debug.Log("sending player updates...");
        foreach(UDPClient client in connectedClients)
        {
            SendToClient(client, SerializeClass(updates));
        }*/
    }

    protected void DoPlayerInput(PlayerInput input)
    {
        Debug.Log("doing input handling..");
        testString = "doing input...";
        //update corresponding player object based on input
        PlayerMovement player = Array.Find(players, x => (int)x.playerID == input.playerID);
        player.DoMovement(input);
    }

    protected override void HandleSerializedData(SerializeBase data)
    {
        testFloat = 1111;
        Type t = data.GetType();
        if (t.Equals(typeof(PlayerInput)))
        {
            PlayerInput input = (PlayerInput)data;
            lock (inputs)
            {
                inputs[input.playerID] = input;
            }
            testFloat = 2222;
            //find player object and execute movement method... players[input.playerid].doMovement(input.xAxis, input.Jump, input.Action);
            //create new PlayerUpdates(input.playerid, new playerInfo(players[input.playerid].tranform.position.x, players[input.playerid].tranform.position.y);
            //serialize PlayerUpdates....
            //send serialized Playerupdates to all (or all other?) clients
        }


    }
}
