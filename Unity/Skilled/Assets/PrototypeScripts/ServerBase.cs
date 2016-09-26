using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class NetworkBase{

    //TODO test how well it syncs client/server by default
    //TODO else send specific match data..(ie.. player died, player got powerup)
    protected const int SERVER_PORT = 17000;
    protected const string SERVER_IP = "86.80.201.15";
    protected UdpClient serverClient;
    protected UDPClient Mainserver;
    BinaryFormatter bFormatter;
    public int playerID;
    
    public NetworkBase(UdpClient client)
    {
        bFormatter = new BinaryFormatter();
        Mainserver = new UDPClient(IPAddress.Parse(SERVER_IP), SERVER_PORT);
        serverClient = client;
        serverClient.BeginReceive(receive, null);
    }

    public virtual void Update()
    {

    }

    public void SendToClient(UDPClient client, byte[] data)
    {
        Debug.Log("Sent: " + Encoding.UTF8.GetString(data));
        serverClient.Send(data, data.GetLength(0), client.endPoint);
    }

   protected void receive(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = serverClient.EndReceive(res, ref RemoteIpEndPoint);

        string stringData = Encoding.UTF8.GetString(received);
        Debug.Log("received: " + stringData);
        HandleSerializedData(DeserializeClass(received));
        receiveCallback(res);
        serverClient.BeginReceive(new AsyncCallback(receive), null);
    }

    protected virtual void receiveCallback(IAsyncResult res)
    {

    }

    //client + server
    [Serializable]
    public class PlayerInput : SerializeBase
    {
        public PlayerInput(int playerID = 0, float xAxis = 0, bool Jump = false, bool Action = false)
        {
            this.playerID = playerID;
            this.xAxis = xAxis;
            this.Jump = Jump;
            this.Action = Action;
        }
        public int playerID;
        public float xAxis;
        public bool Jump;
        public bool Action;
    }

    //client + server
    [Serializable]
    public abstract class SerializeBase
    {

    }
    //client + server
    [Serializable]
    public class PlayerInfo : SerializeBase
    {
        public int playerID;
        public float xPos;
        public float yPos;
        public PlayerInfo(int playerID, float xPos, float yPos)
        {
            this.playerID = playerID;
            this.xPos = xPos;
            this.yPos = yPos;
        }
    }

    //client + server
    [Serializable]
    public class PlayerUpdates : SerializeBase
    {
        public PlayerInfo[] PlayerInfos;
        public PlayerUpdates(PlayerInfo[] PlayerInfos)
        {
            this.PlayerInfos = PlayerInfos;
        }
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
    protected byte[] SerializeClass(SerializeBase input)
    {
        using (var memStream = new MemoryStream())
        {
            bFormatter.Serialize(memStream, input);
            return memStream.ToArray();
        }
    }
    //server + client
    protected SerializeBase DeserializeClass(byte[] data)
    {
        using (var memStream = new MemoryStream())
        { 
            memStream.Write(data, 0, data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (SerializeBase)bFormatter.Deserialize(memStream);
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
