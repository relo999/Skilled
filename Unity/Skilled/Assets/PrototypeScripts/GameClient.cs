﻿using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;

public class GameClient : NetworkBase {

    PlayerMovement ownMovement;
    const int EXPECTED_PACKETS = 64;    //what is set in GameServer, TODO detect automatically how many packets are supposed to be sent
    int receivedPackets = 0;
    float timer = 0;
    DateTime pingStart;
    public int Ping = 0;
    bool isPinging = false;
    public int PacketLoss = 0;
    static PlayerMovement[] players;
    static PlayerUpdates newUpdates;

    const float TickRate = 10;
    int intervalMS;
    int intervalS;
    static bool startedPing = false;
    static bool receivedPing = false;


    public static PlayerInput lastInput;

    public override void receiveCallback(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = serverClient.EndReceive(res, ref RemoteIpEndPoint);

        string stringData = Encoding.UTF8.GetString(received);
        if(!stringData.StartsWith("<")) //testing only
        Debug.Log("received client: " + (stringData.StartsWith("<")? "data" : stringData));

        
        //ping, in progress
        if(stringData.Contains("PingResult"))
        {
            
            receivedPackets--;

                receivedPing = true;
                
                isPinging = false;
                startedPing = false;
            
        }else
        HandleSerializedData(DeserializeClass(received));
        //receiveCallback(res);
        //Debug.Log("started receiving");
        receivedPackets++;
        serverClient.BeginReceive(new AsyncCallback(receiveCallback), null);
    }

    public IEnumerator UpdateClient()
    {
        while(true)
        {
            yield return new WaitForSeconds(intervalS);
            
            if (lastInput != null)
            {
                SendPlayerInput(lastInput);
                lastInput = null;
            }
            if (startedPing)
            {
                startedPing = false;
                SendToClient(connectedClient, UDPClient.StringToBytes("Ping"));
                SendToClient(connectedClient, UDPClient.StringToBytes("Ping"));
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
            Debug.Log("Packet loss: " + packetLoss);
            PacketLoss = packetLoss;
            receivedPackets = 0;

            if(!isPinging) StartPing();

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

        //SendToClient(connectedClient, UDPClient.StringToBytes("testClientSend"));
        intervalS = (int)(1.0f / TickRate);
        intervalMS = (int)(intervalS * 1000.0f);
        
    }

    public void StartClient()
    {
        //Debug.Log(GetLocalIPAddress() + " : " + GetLocalEndPoint().Port);
        isReady = true;
        serverClient.BeginReceive(new AsyncCallback(receiveCallback), null);
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

        PlayerUpdates updates = Pupdates;
        for (int i = 0; i < updates.PlayerInfos.Length; i++)
        {
            PlayerInfo info = updates.PlayerInfos[i];
            PlayerMovement playerMov = Array.Find(players, x => (int)x.playerID == info.playerID);
            GameObject player = playerMov.gameObject;

            int oldPositionIndex = playerMov.oldPositionPointer - (int)((Ping / 1000f) / 0.05f);
            while(oldPositionIndex < 0)
            {
                oldPositionIndex += 10;
                oldPositionIndex %= 10;
            }
            if ((playerMov.oldPositions[oldPositionIndex] - new Vector2(info.xPos, info.yPos)).magnitude < 0.5f) continue;
            //if (playerMov.NetworkControl) continue;
            

            player.transform.position = new Vector3(info.xPos, info.yPos, player.transform.position.z);
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(info.xVel, info.yVel);
        }
    }

    protected override void HandleSerializedData(SerializeBase data)
    {
        //return;
        Type t = data.GetType();

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
