﻿using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;

public class NetManager : MonoBehaviour {

    NetworkBase networkBase = null;
    UdpClient client;
    NetworkBase.UDPClient Mainserver;
    NetworkBase.UDPClient Connectedclient = null;
    bool startedServer = false;
    
    
    const int SERVER_PORT = 17000;
    const string SERVER_IP = "86.80.201.15";
    // Use this for initialization
    void Start () {
        client = new UdpClient();
        
        Mainserver = new NetworkBase.UDPClient(IPAddress.Parse(SERVER_IP), SERVER_PORT);
        instance = this;
    }

    public static NetManager instance = null;
    public static bool isServer = false;
    public static bool hasStarted = false;

    public void SendInput(NetworkBase.PlayerInput input)
    {
        if(!isServer)
        {

            GameClient.lastInput = input;

        }
    }
	// Update is called once per frame
	void Update () {
	    if(networkBase == null)
        {
            if (Input.GetKeyDown(KeyCode.N))RequestMatch();

            //testing
            //if (Input.GetKeyDown(KeyCode.N)) StartServer();
            //if (Input.GetKeyDown(KeyCode.N)) StartClient();
        }
        else
        {
            networkBase.Update();
            if (!startedServer && isServer && networkBase.isReady)
            {
                startedServer = true;
                GameServer server = networkBase as GameServer;
                StartCoroutine(server.UpdateServer());

            }
            if (!startedServer && !isServer && networkBase.isReady)
            {
                startedServer = true;
                GameClient gameclient = networkBase as GameClient;
                StartCoroutine(gameclient.UpdateClient());

            }
        }
	}

    void RequestMatch(int players = 1)
    {
        byte[] data = NetworkBase.UDPClient.StringToBytes("connect" + players);
        SendToClient(new NetworkBase.UDPClient("0.0.0.9", 999), data);
        //SendToClient(Mainserver, data);
        Debug.Log(NetworkBase.GetLocalIPAddress() + ":" + GetLocalEndPoint().Port);
        SendToClient(Mainserver, data);
        Debug.Log(NetworkBase.GetLocalIPAddress() + ":" + GetLocalEndPoint().Port);
        client.BeginReceive(new AsyncCallback(receive), null);
    }

    private void receive(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);

        string stringData = Encoding.UTF8.GetString(received);
        Debug.Log("received: " + stringData);
        if (Connectedclient == null && networkBase == null && stringData.Contains(":"))    //it contains a ip:port
        {
            string[] splitData = stringData.Split(':');
            Connectedclient = new NetworkBase.UDPClient(IPAddress.Parse(splitData[0]), int.Parse(splitData[1]));
            
            //if (splitData[2] == "server") StartServer();
            if (splitData[2] == "client")
            {
                string[] splitIDs = splitData[3].Split(',');
                NetworkBase.playerIDs = new int[splitIDs.Length];
                for (int i = 0; i < splitIDs.Length; i++)
                {
                    NetworkBase.playerIDs[i] = int.Parse(splitIDs[i]);
                }
           
                StartClient();
            
            }
            
            if(splitData[2] == "server")
            {
                //GameServer server = networkBase as GameServer;
                //server.StartGame(new NetworkBase.UDPClient[] { Connectedclient });
                string[] splitIDs = splitData[3].Split(',');
                NetworkBase.playerIDs = new int[splitIDs.Length];
                for (int i = 0; i < splitIDs.Length; i++)
                {
                    NetworkBase.playerIDs[i] = int.Parse(splitIDs[i]);
                }

                StartServer();
            }
            networkBase.connectedClient = Connectedclient;
            //Debug.Log(NetworkBase.GetLocalIPAddress() + ":" + GetLocalEndPoint());
            //return;
            SendToClient(Connectedclient, Encoding.ASCII.GetBytes("work?"));
            //Debug.Log(NetworkBase.GetLocalIPAddress() + ":" + GetLocalEndPoint());
            //this.Start();
        }
        
        client.BeginReceive(networkBase == null? new AsyncCallback(receive) : new AsyncCallback(networkBase.receiveCallback), null);
    }

    //not in use
    void StopListening()
    {
        //client.Close();
        //IPEndPoint p = new IPEndPoint(IPAddress.Any, 999);
        //client.EndReceive(null, ref p);
    }

    public IPEndPoint GetLocalEndPoint()    //to get own port
    {
        return (IPEndPoint)client.Client.LocalEndPoint;
    }
    public void SendToClient(NetworkBase.UDPClient Client, byte[] data)
    {
        Debug.Log("Sent: " + Encoding.UTF8.GetString(data));
        this.client.Send(data, data.GetLength(0), Client.endPoint);
    }
    void StartClient()
    {
        hasStarted = true;
        Debug.Log("Started client");
        networkBase = new GameClient(client);
        GameClient gclient = networkBase as GameClient;
        gclient.StartClient();
    }
    void StartServer()
    {
        hasStarted = true;
        isServer = true;
        Debug.Log("Started server");
        networkBase = new GameServer(client);
        GameServer server = networkBase as GameServer;
        server.StartGame(new NetworkBase.UDPClient[] { Connectedclient });
        
    }


}
