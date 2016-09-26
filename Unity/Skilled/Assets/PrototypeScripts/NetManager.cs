using UnityEngine;
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

    const int SERVER_PORT = 17000;
    const string SERVER_IP = "86.80.201.15";
    // Use this for initialization
    void Start () {
        client = new UdpClient();
        Mainserver = new NetworkBase.UDPClient(IPAddress.Parse(SERVER_IP), SERVER_PORT);
    }
	
	// Update is called once per frame
	void Update () {
	    if(networkBase == null)
        {
            if (Input.GetKeyDown(KeyCode.N)) RequestMatch();
            
        }
        else
        {
            networkBase.Update();
        }
	}

    void RequestMatch()
    {
        byte[] data = NetworkBase.UDPClient.StringToBytes("connect");

        SendToClient(Mainserver, data);
        client.BeginReceive(new AsyncCallback(receive), null);
    }

    private void receive(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);

        string stringData = Encoding.UTF8.GetString(received);
        Debug.Log("received: " + stringData);
        if (stringData.Contains(":"))    //it contains a ip:port
        {
            string[] splitData = stringData.Split(':');
            Connectedclient = new NetworkBase.UDPClient(IPAddress.Parse(splitData[0]), int.Parse(splitData[1]));
            
            if (splitData[2] == "server") StartServer();
            if (splitData[2] == "client")
            {
                StartClient();
                networkBase.playerID = int.Parse(splitData[3]);
            }
            networkBase.connectedClient = Connectedclient;
            if(splitData[2] == "server")
            {
                GameServer server = networkBase as GameServer;
                server.StartGame(new NetworkBase.UDPClient[] { Connectedclient });
            }

            //this.Start();
        }

        client.BeginReceive(new AsyncCallback(receive), null);
    }

    public void SendToClient(NetworkBase.UDPClient Client, byte[] data)
    {
        Debug.Log("Sent: " + Encoding.UTF8.GetString(data));
        this.client.Send(data, data.GetLength(0), Client.endPoint);
    }
    void StartClient()
    {
        Debug.Log("Started client");
        networkBase = new GameClient(client);
    }
    void StartServer()
    {
        Debug.Log("Started server");
        networkBase = new GameServer(client);
        GameServer server = networkBase as GameServer;
        server.StartGame(new NetworkBase.UDPClient[1] { Connectedclient });
    }


}
