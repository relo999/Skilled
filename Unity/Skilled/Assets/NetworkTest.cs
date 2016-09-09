using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

/// <summary>
/// 
/// TODO
/// 
/// Connect to home server using udp    (for both clients)
/// save ip/port on home server
/// 
/// send clients eachothers ip/port
/// connect eachother
/// 
/// </summary>
public class NetworkTest : MonoBehaviour {

    public bool isAtStartup = true;
    NetworkClient myClient;
    string debugText = "";




    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            SendMessage();
        }
        Debug.Log(NetworkServer.connections.Count);
        if (isAtStartup)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SetupServer();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                SetupClient();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                SetupServer();
                SetupLocalClient();
            }
        }
    }

    void OnGUI()
    {
        if (isAtStartup)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Press z for server");
            GUI.Label(new Rect(2, 30, 150, 100), "Press x for client");
            GUI.Label(new Rect(2, 50, 150, 100), "Press c for both");
        }
        GUI.Label(new Rect(2, 80, 150, 100), debugText);
    }


    // Create a server and listen on a port
    public void SetupServer()
    {
        NetworkServer.Listen(4444);
        isAtStartup = false;
        Sender s = new Sender();
        s.Send();
    }

    public class MessageTest : MessageBase
    {
        public string text;
    }

    public class MyMsgType
    {
        public static short test = MsgType.Highest + 1;
    };

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("Local IP Address Not Found!");
    }

    // Create a client and connect to the server port
    public void SetupClient()
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.RegisterHandler(MyMsgType.test, OnMessage);
        //myClient.Connect("127.0.0.1", 4444);
        myClient.Connect("145.76.114.38", 4444);
        // Debug.Log(GetLocalIPAddress());
        Receiver r = new Receiver();
        r.StartListening();
        
        isAtStartup = false;
    }

    public void SendMessage()
    {
        MessageTest msg = new MessageTest();
        msg.text = "aaaaaaaaaa";

        NetworkServer.SendToAll(MyMsgType.test, msg);
    }

    // Create a local client and connect to the local server
    public void SetupLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.RegisterHandler(MyMsgType.test, OnMessage);
        isAtStartup = false;
    }

    public void OnMessage(NetworkMessage netMsg)
    {
        MessageTest msg = netMsg.ReadMessage<MessageTest>();
        Debug.Log("test: " + msg.text);
    }

    // client function
    public void OnConnected(NetworkMessage netMsg)
    {
        //ClientScene.Ready(myClient.connection);
        
        Debug.Log("Connected to server ");
        

        debugText = myClient.isConnected.ToString();
        //Network.Instantiate

        
        //GameObject.Instantiate(PlayerPrefab);
        
    }




    public class Receiver
    {
        private readonly UdpClient udp = new UdpClient(15000);
        public void StartListening()
        {
            this.udp.BeginReceive(Receive, new object());
        }
        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 15000);
            byte[] bytes = udp.EndReceive(ar, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            Debug.Log("message: " + message);
            //StartListening();
        }
    }
    public class Sender
    {
        public void Send()
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 15000);
            byte[] bytes = Encoding.ASCII.GetBytes("Foo");
            client.Send(bytes, bytes.Length, ip);
            client.Close();
        }
    }


}
