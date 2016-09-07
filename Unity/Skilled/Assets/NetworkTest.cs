using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkTest : MonoBehaviour {

    public bool isAtStartup = true;
    NetworkClient myClient;
    string debugText = "";

    void Update()
    {
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
    }

    public class MessageTest : MessageBase
    {
        public string text;
    }

    // Create a client and connect to the server port
    public void SetupClient()
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.Connect("127.0.0.1", 4444);
        Debug.Log(myClient.serverIp);
        MessageTest msg = new MessageTest();
        msg.text = "aaaaa";
        myClient.Send(999, msg);
        isAtStartup = false;
    }

    // Create a local client and connect to the local server
    public void SetupLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        isAtStartup = false;
    }

    // client function
    public void OnConnected(NetworkMessage netMsg)
    {
        ClientScene.Ready(myClient.connection);
        
        Debug.Log("Connected to server ");
        

        debugText = myClient.isConnected.ToString();
        //Network.Instantiate

        
        //GameObject.Instantiate(PlayerPrefab);
        
    }

}
