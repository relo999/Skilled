using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading;

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


public class Client
{
    const int SERVER_PORT = 17000;
    const string SERVER_IP = "86.80.201.15";
    //int ListenPort;
    UdpClient serverClient;

    UDPClient Mainserver;
    UDPClient Connectedclient = null;

    public Client()
    {
        Mainserver = new UDPClient(IPAddress.Parse(SERVER_IP), SERVER_PORT);

        serverClient = new UdpClient();

        byte[] data = UDPClient.StringToBytes("connect");

        Debug.Log("Client ready");
        SendToClient(Mainserver, data);

        serverClient.BeginReceive(receive, null);

    }


    public IPEndPoint GetLocalEndPoint()
    {
        return (IPEndPoint)serverClient.Client.LocalEndPoint;
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


	//don't use on main thread
    public void ReadInput()
    {

        SendToClient(Connectedclient, UDPClient.StringToBytes(Console.ReadLine()));
        serverClient.BeginReceive(new AsyncCallback(receive), null);

    }

    public void Start()
    {
        Thread t = new Thread(() => { ReadInput(); });
        t.IsBackground = true;
        t.Start();
    }
    public void SendToClient(UDPClient client, byte[] data)
    {
        Debug.Log("Sent: " + Encoding.UTF8.GetString(data));
        serverClient.Send(data, data.GetLength(0), client.endPoint);
    }


    private void receive(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = serverClient.EndReceive(res, ref RemoteIpEndPoint);

        string stringData = Encoding.UTF8.GetString(received);
        Debug.Log("received: " + stringData);
        if (stringData.Contains(":"))    //it contains a ip:port
        {
            Debug.Log("ready to type");
            string[] splitData = stringData.Split(':');
            Connectedclient = new UDPClient(IPAddress.Parse(splitData[0]), int.Parse(splitData[1]));
            this.Start();
        }

        serverClient.BeginReceive(new AsyncCallback(receive), null);
    }



}
