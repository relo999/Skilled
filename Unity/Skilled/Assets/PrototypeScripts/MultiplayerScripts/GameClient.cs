using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading;

public class GameClient : NetworkBase {

    PlayerMovement ownMovement;
    public const int EXPECTED_PACKETS = 64;    //what is set in GameServer, TODO detect automatically how many packets are supposed to be sent
    public int LastReceivedPackets = 0;
    int receivedPackets = 0;
    float timer = 0;
    DateTime pingStart;
    public int Ping = 0;
    bool isPinging = false;
    public int PacketLoss = 0;
    static PlayerMovement[] players;
    static PlayerUpdates newUpdates;


    public static GameClient instance = null;
    const float TickRate = 10;
    int intervalMS;
    int intervalS;
    static bool startedPing = false;
    static bool receivedPing = false;

    public int controllingPlayers = 0;

    public static PlayerInput lastInput;

    bool connectionSucces = false;

    float lastGameTimeReceived = -1;

    void StartTestConnection()
    {
        Thread testThread = new Thread(new ThreadStart(TestConnection));
        testThread.Start();
    }

    void TestConnection()
    {
        for (int i = 0; i < 3; i++)
        {
            Thread.Sleep(500);
            byte[] data = Encoding.ASCII.GetBytes("TestConnection");
            SendToClient(connectedClient, data);
        }
        Thread.Sleep(500);
        if(!connectionSucces)
        {
            FailedConnection();
        }
    }

    void FailedConnection()
    {
        
        byte[] data = Encoding.ASCII.GetBytes("RequestRelay," + connectedClient.endPoint.Address + ":" + connectedClient.endPoint.Port);
        SendToClient(Mainserver, data);
        SendToClient(Mainserver, data);
        connectedClient = null;
    }

    public override void receiveCallback(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = serverClient.EndReceive(res, ref RemoteIpEndPoint);
        serverClient.BeginReceive(new AsyncCallback(receiveCallback), null);
        
        string stringData = Encoding.ASCII.GetString(received);
        testFloat = 1;
        if(!stringData.StartsWith("<")) //testing only
            Debug.Log(stringData);
        if (!isReady && stringData.Contains(":"))    //it contains a ip:port
        {
            testFloat = 2;
            string[] splitData = stringData.Split(':');
            connectedClient = new NetworkBase.UDPClient(IPAddress.Parse(splitData[0]), int.Parse(splitData[1]));

            NetworkBase.playerIDs = new int[controllingPlayers];
            int otherPlayers = int.Parse(splitData[2]);
            for (int i = 0; i < controllingPlayers; i++)
            {
                NetworkBase.playerIDs[i] = otherPlayers + i;
            }
            byte[] data = Encoding.ASCII.GetBytes("test123");
            SendToClient(connectedClient, data);
            StartTestConnection();
            testFloat = 3;
        }
        testFloat = 8;

        if(stringData == "TestConnection")
        {
            connectionSucces = true;
        }

        if(stringData == "StartGame")
        {
            testFloat = 9;
            isReady = true;
            NetManager.hasStarted = true;
        }
            //Debug.Log("received client: " + (stringData.StartsWith("<")? "data" : stringData));
            
        //ping, in progress
        if (stringData == "PingResult")
        {
            testFloat = 10;
            receivedPackets--;

            receivedPing = true;

            isPinging = false;
            startedPing = false;

        }
        else
        {
            testFloat = 11;
            if (stringData.StartsWith("<"))
            {
                testFloat = 61;
                testFloat = received.Length;
                SerializeBase SB = DeserializeClass(received);
                testFloat = 63;
                HandleSerializedData(SB);
                testFloat = 62;
            }
        }
        //receiveCallback(res);
        testFloat = 4;
        //Debug.Log("started receiving..");
        receivedPackets++;
        //serverClient.BeginReceive(new AsyncCallback(receiveCallback), null);
    }

    public IEnumerator UpdateClient()
    {
        while(true)
        {
            yield return new WaitForSeconds(intervalS);
            
            if (lastInput != null)
            {
                Debug.Log("sending input");
                SendPlayerInput(lastInput);
                lastInput = null;
            }
            if (startedPing)
            {
                startedPing = false;
                SendToClient(connectedClient, UDPClient.StringToBytes("Ping"));
                //SendToClient(connectedClient, UDPClient.StringToBytes("Ping"));
                Debug.Log("pinging to: " + connectedClient.endPoint);
            }


        }
        
    }

    public void StartPing()
    {
        pingStart = System.DateTime.Now;
        startedPing = true;
        Debug.Log("starting ping");
        //SendToClient(connectedClient, UDPClient.StringToBytes("Ping"));
        //SendToClient(connectedClient, UDPClient.StringToBytes("Ping")); //sent twice to ensure delivery
        isPinging = true;
    }

    void TimerHandler()
    {
        if (timer >= 1)
        {
            timer = 0;
            int packetLoss = EXPECTED_PACKETS - receivedPackets;
            packetLoss = packetLoss < 0 ? 0 : packetLoss;   
            PacketLoss = packetLoss;
            LastReceivedPackets = receivedPackets;
            if(PacketLoss >= EXPECTED_PACKETS/2)
            {
                SendToClient(connectedClient, Encoding.ASCII.GetBytes("Work?"));
            }
            receivedPackets = 0;

            if (!isPinging) StartPing();
            else
            {
                if ((DateTime.Now - pingStart).Seconds > 2)
                {
                    receivedPing = true;

                    isPinging = false;
                    startedPing = false;
                }
            }

        }
    }


    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        TimerHandler();
        
        if(receivedPing)
        {
            receivedPing = false;
            isPinging = false;
            Ping = (System.DateTime.Now - pingStart).Milliseconds;
        }
        if(players == null)
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
        if (newUpdates != null) DoPlayerUpdates(newUpdates);
        //SendToClient(connectedClient, Encoding.ASCII.GetBytes("send test...."));
        //Debug.Log("Sent player input");
       // if (ownMovement == null) SetPlayerID();
        //SendPlayerInput(ownMovement.input);
        //serverClient.BeginReceive(new AsyncCallback(receive), null);
        //IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8000);
        //byte[] data = serverClient.Receive(ref endpoint);
        //string stringData = Encoding.UTF8.GetString(data);
        //Debug.Log("received: " + stringData);
    }
    public GameClient(UdpClient client) : base(client)
    {
        instance = this;
        //SendToClient(connectedClient, UDPClient.StringToBytes("testClientSend"));
        intervalS = (int)(1.0f / TickRate);
        intervalMS = (int)(intervalS * 1000.0f);
        
    }

    public void JoinLobby(string name, int players)
    {
        byte[] data = Encoding.ASCII.GetBytes("JoinLobby" + players + name + "," + GetLocalIPAddress() + ":" + GetLocalEndPoint().Port);
        SendToClient(Mainserver, data);
    }

    public void StartClient()
    {
        //Debug.Log(GetLocalIPAddress() + " : " + GetLocalEndPoint().Port);
        isReady = true;
        //serverClient.BeginReceive(new AsyncCallback(receiveCallback), null);
    }
    public void SetPlayerID()
    {
        for (int i = 0; i < playerIDs.Length; i++)
        {
            Array.Find(players, x => (int)x.playerID == playerIDs[i]).NetworkControl = true;
        }

    }
    public void SendPlayerInput(PlayerInput input)
    {
        SendToClient(connectedClient, SerializeClass(input));
    }

    protected void DoPlayerUpdates(PlayerUpdates Pupdates)
    {
        string test123 = "";
        PlayerUpdates updates = Pupdates;
        for (int i = 0; i < updates.PlayerInfos.Length; i++)
        {
            PlayerInfo info = updates.PlayerInfos[i];
            PlayerMovement playerMov;


            playerMov = Array.Find(players, x => (int)x.playerID == info.playerID);

            GameObject player = playerMov.gameObject;

            //int oldPositionIndex = playerMov.oldPositionPointer - (int)Mathf.Round((GameTimer - Pupdates.gameTime) / 0.015f);

            /*
            int oldPositionIndex = playerMov.oldPositionPointer - (int)((GameTimer - Pupdates.gameTime) / 0.015f);
            oldPositionIndex = oldPositionIndex < 0 ? oldPositionIndex + playerMov.oldPositions.Length : oldPositionIndex;
            */
            //int oldPositionIndex = 5;
         
            if(true) //!playerMov.NetworkControl)
            {
                if (playerMov.networkPositions.Count < 3 ||  playerMov.networkPositions[playerMov.networkPositions.Count-1].GameTime < updates.gameTime)
                {

                    playerMov.networkPositions.Add(new PlayerMovement.NetworkPosition(new Vector2(info.xPos, info.yPos), updates.gameTime, new Vector2(info.xVel, info.yVel)));
               
                }
                    


            }
            /*
            //Debug.Log((playerMov.oldPositions[oldPositionIndex] - new Vector2(info.xPos, info.yPos)).magnitude);
            if (((playerMov.oldPositions[oldPositionIndex] - new Vector2(info.xPos, info.yPos)).magnitude > 3.55f && playerMov.NetworkControl) || !playerMov.NetworkControl)
            {
                player.transform.position = new Vector3(info.xPos, info.yPos, player.transform.position.z);
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(info.xVel, info.yVel);

            }
            playerMov.testClone.transform.position = new Vector3(info.xPos, info.yPos, player.transform.position.z);
            playerMov.testClone2.transform.position = playerMov.oldPositions[oldPositionIndex];
            */
            /*  //following debug information for players
            for (int j = 0; j < playerMov.testClones.Length; j++)
            {
                playerMov.testClones[j].transform.position = playerMov.oldPositions[j];
            }*/


            /*
            else if(playerMov.NetworkControl)
            {
                Vector2 diffPos = playerMov.oldPositions[oldPositionIndex] - new Vector2(info.xPos, info.yPos);
                test123 += diffPos.magnitude;
                for (int j = 0; j < playerMov.oldPositions.Length; j++)
                {
                    playerMov.oldPositions[j] -= (Vector2)diffPos;
                }
                player.transform.position -= (Vector3)diffPos;
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(info.xVel, info.yVel);
            }*/


        }
        testString = test123;
    }

    protected override void HandleSerializedData(SerializeBase data)
    {

        //return;
        //Debug.Log("handling data...");
        //Debug.Log("gametimer: " + data.gameTime);
        testFloat = 99;
        if (GameTimer - data.gameTime > (float)Ping/2000f + 0.01f || GameTimer - data.gameTime < (float)Ping/2000f - 0.01f) GameTimer = data.gameTime + ((float)Ping/2000f);
        Type t = data.GetType();
        testFloat = 91;
        if (t.Equals(typeof(PlayerUpdates)))
        {
            newUpdates = (PlayerUpdates)data;
            //pseudo code:
      /*
            PlayerUpdates updates = (PlayerUpdates)data;
            for (int i = 0; i < updates.PlayerInfos.Length; i++)
            {
            PlayerInfo info = updates.PlayerInfos[i];
            GameObject player = Array.Find(players, x => (int)x.playerID == info.playerID).gameObject;
            player.transform.position = new Vector3(info.xPos, info.yPos, player.transform.position.z);

    
            //find player object and execute set position method... players[info.playerID].SetPosition(info.xPos, info.yPos);
            //no need to update server after this
            }
    */
        }
        //CLIENT
        //if(t.Equals(typeof(MatchInfo)))
        //{
        //MatchInfo matchInfo = (MatchInfo)data;
        //if(matchInfo.gameEnd)
        //{
        //get matchInfo.scores
        //goto lobby menu, ready for next match
        //return;
        //}
        //if(matchInfo.playerdied)//do something? TODO design syncing 
        //}
    }


}
