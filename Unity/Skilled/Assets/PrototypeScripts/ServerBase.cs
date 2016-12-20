using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using System.IO.Compression;
using UnityEngine.UI;

public class NetworkBase{

    //TODO test how well it syncs client/server by default
    //TODO else send specific match data..(ie.. player died, player got powerup)
    protected const int SERVER_PORT = 17000;
    protected const string SERVER_IP = "86.80.201.15";
    protected UdpClient serverClient;
    protected UDPClient Mainserver;
    public static int[] playerIDs;
    public UDPClient connectedClient = null;
    XmlSerializer xmlSerializer;
    public bool isReady = false;

    public string testString = "-1";
    public float testFloat = -1;


    public static float GameTimer = 0;

    public NetworkBase(UdpClient client)
    {
        xmlSerializer = new XmlSerializer(typeof(SerializeBase));
        Mainserver = new UDPClient(IPAddress.Parse(SERVER_IP), SERVER_PORT);
        serverClient = client;
        /*

        SerializeBase test = DeserializeClass(SerializeClass(new PlayerInput(1, 2, true, true)));
        if (test.GetType().Equals(typeof(PlayerInput)))
        {
            PlayerInput testI = (PlayerInput)test;
            Debug.Log(testI.xAxis);
        }
        
        SerializeBase test2 = DeserializeClass(SerializeClass(new PlayerUpdates(new PlayerInfo[] { new PlayerInfo(1, 2, 3) })));
        if (test2.GetType().Equals(typeof(PlayerUpdates)))
        {
            PlayerUpdates testI = (PlayerUpdates)test2;
            Debug.Log(testI.PlayerInfos[0].xPos);
        }
        */
        //Debug.Log(SerializeClass(new PlayerUpdates(new PlayerInfo[] { new PlayerInfo(1, 5.135f, 1.9943f) })).Length);
        //serverClient.BeginReceive(receive, null);
        //serverClient.BeginReceive(receiveCallback, serverClient);
    }


    public void UpdateDebugText()
    {

    }
    public virtual void Update()
    {

        //if (serverClient == null) return;

        GameTimer += Time.deltaTime;

        Text debugText = GameObject.Find("NetworkDebug").GetComponent<Text>();
        bool isClient = (this.GetType() == typeof(GameClient));
        debugText.text =  isClient? "client" : "server";
        debugText.text += "\nTime: " + System.Math.Round(GameTimer,4);
        //debugText.text += "\nLocal: " + GetLocalIPAddress() + ":" + GetLocalEndPoint().Port;
        if (isClient)
        {
            GameClient c = this as GameClient;
            debugText.text += "\nPing: " + c.Ping + "\n";
            debugText.text += "Packets/s: " + c.LastReceivedPackets;

            debugText.text += "\nConnected to: " + connectedClient.endPoint.Port;
        }
        else
        {
            GameServer s = this as GameServer;
            for (int i = 0; i < s.Clients.Count; i++)
            {
                if (s.Clients[i] != null)
                {
                    debugText.text += "\nOwn: " + ((IPEndPoint)s.sockets[i].Client.LocalEndPoint).Port + " -  Con: " + s.Clients[i].endPoint.Port;
                }
            }
        }
        
        //debugText.text += "\ncon: " + connectedClient.endPoint.Address + ":" + connectedClient.endPoint.Port;
        //debugText.text += "\ntest: " + testString;
        //debugText.text += "\nfloat: " + testFloat;

    }
    
    public void SendToClient(UDPClient client, byte[] data)
    {
       // if (!Encoding.ASCII.GetString(data).StartsWith("<")) //testing only
            Debug.Log("Sent: " + Encoding.UTF8.GetString(data));
        //serverClient.Send(data, data.GetLength(0), client.endPoint);
        serverClient.BeginSend(data, data.Length, client.endPoint, null, null);
    }

   protected void receive(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = serverClient.EndReceive(res, ref RemoteIpEndPoint);

        string stringData = Encoding.UTF8.GetString(received);
        Debug.Log("received base: " + stringData);
        HandleSerializedData(DeserializeClass(received));

        serverClient.BeginReceive(new AsyncCallback(receive), null);
    }

    public virtual void receiveCallback(IAsyncResult res)
    {
        //not in use
    }

    //client + server
    public class PlayerInput : SerializeBase
    {
        public PlayerInput(int playerID = 0, float xAxis = 0, bool Jump = false, bool JumpDown = false, bool JumpUp = false, bool Action = false)
        {
            this.playerID = playerID;
            this.xAxis = xAxis;
            this.Jump = Jump;
            this.JumpDown = JumpDown;
            this.JumpUp = JumpUp;
            this.Action = Action;
        }
        public PlayerInput() { }

        public int playerID;
        public float xAxis;
        public bool Jump;
        public bool JumpDown;
        public bool JumpUp;
        public bool Action;
    }

    //client + server
    [XmlInclude(typeof(PlayerInfo))]
    [XmlInclude(typeof(PlayerInput))]
    [XmlInclude(typeof(PlayerUpdates))]
    public abstract class SerializeBase  //abstract?
    {
        public float gameTime;
        //public SerializeBase() { }
    }
    //client + server
    public class PlayerInfo : SerializeBase
    {
        public int playerID;
        public float xPos;
        public float yPos;
        public float xVel;
        public float yVel;
        public PlayerInfo(int playerID, float xPos, float yPos, float xVel, float yVel)
        {
            this.playerID = playerID;
            this.xPos = xPos;
            this.yPos = yPos;
            this.xVel = xVel;
            this.yVel = yVel;
        }
        public PlayerInfo() { }
    }

    //client + server
    public class PlayerUpdates : SerializeBase
    {
        public PlayerInfo[] PlayerInfos;
        public PlayerUpdates(PlayerInfo[] PlayerInfos)
        {
            this.PlayerInfos = PlayerInfos;
        }
        public PlayerUpdates() { }
    }

    //client + server
    


    /*
    // Use this for initialization
    void Start () {
        bFormatter = new BinaryFormatter();

        //PlayerUpdates testUpdatesIn = new PlayerUpdates(new PlayerInfo[1] { new PlayerInfo(1, 1.1f,2.2f) });
       // PlayerUpdates testUpdatesOut = (PlayerUpdates)DeserializeClass(SerializeClass(testUpdatesIn));
        SerializeBase input;    //send input with udp message
        SerializeBase output;   //get output from received udp message

        input = new PlayerUpdates(new PlayerInfo[1] { new PlayerInfo(1, 1.1f, 2.2f) });
        output = DeserializeClass(SerializeClass(input));

        //server + client
        HandleSerializedData(output);


        // Debug.Log(testUpdatesOut.PlayerInfos[0].playerID);
        // PlayerInput testInput = new PlayerInput(0, 1.1f, true, false);
        // PlayerInput testOutput = (PlayerInput)DeserializeClass(SerializeClass(testInput));
    }*/

    //server + client but different 
    protected virtual void HandleSerializedData(SerializeBase data)
    {
        //BOTH
        Debug.Log("dont do this pls");
        Type t = data.GetType();


        //SERVER
        //pseudo code:
        //if(t.Equals(typeof(ConnectionInfo)))
        //{
        //ConnectionInfo connectionInfo = (ConnectionInfo)data;
            //if(connectionInfo.requestMatch)
            //{
              //save client data and wait for another client with the same request
              //return;
            //}
        //}


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

        //SERVER
        if (t.Equals(typeof(PlayerInput)))
        {
            //update corresponding player object based on input

            //pseudo code:
            //PlayerInput input = (PlayerInput)data;
            //find player object and execute movement method... players[input.playerid].doMovement(input.xAxis, input.Jump, input.Action);
            //create new PlayerUpdates(input.playerid, new playerInfo(players[input.playerid].tranform.position.x, players[input.playerid].tranform.position.y);
            //serialize PlayerUpdates....
            //send serialized Playerupdates to all (or all other?) clients
        }

        //CLIENT
        if (t.Equals(typeof(PlayerUpdates)))
        {
            //pseudo code:
            //PlayerUpdates updates = (PlayerUpdates)data;
            //for (int i = 0; i < updates.PlayerInfos.Length; i++)
            //{
                //PlayerInfo info = updates.PlayerInfos[i];
                //find player object and execute set position method... players[info.playerID].SetPosition(info.xPos, info.yPos);
                //no need to update server after this
            //}
        }
    }

    //server + client
    public byte[] SerializeClass(SerializeBase input)
    {
        input.gameTime = GameTimer;
        using (var memStream = new MemoryStream())        {
            xmlSerializer.Serialize(memStream, input);
            //Debug.Log(memStream.ToArray().ToString());
            //Debug.Log(memStream.ToArray().Length);    //check bytesize
            return memStream.ToArray();
        }
       
    }
    //server + client
    public SerializeBase DeserializeClass(byte[] data)
    {
        using (var memStream = new MemoryStream())
        { 
            memStream.Write(data, 0, data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (SerializeBase)xmlSerializer.Deserialize(memStream);
        }
    }

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
    public IPEndPoint GetLocalEndPoint()    //returns 0.0.0.0 as ip sometimes, use GetLocalIPAdress instead for the IP
    {
        return (IPEndPoint)serverClient.Client.LocalEndPoint;
    }




    public class UDPClient
    {

        public IPEndPoint endPoint;

        public UDPClient(IPAddress ip, int port)
        {
            endPoint = new IPEndPoint(ip, port);
        }
        public UDPClient(string ip, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public static byte[] StringToBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

    }


}
