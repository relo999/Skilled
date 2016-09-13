using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class UDPManager : MonoBehaviour {

    UdpClient client;

    IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);   //always the same server ip/port

    public string LocalIP { get {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("10.0.2.4", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        } }

    void Start()
    {
        client = new UdpClient();
        //SendToClient(StringToBytes("connect"), )

    }

    public void SendToClient(byte[] data, IPEndPoint ipendpoint)
    {
        client.Send(data, data.GetLength(0), ipendpoint);
    }

    void WriteDebug(byte[] data)
    {
        Debug.Log(Encoding.UTF8.GetString(data));
    }



    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            //sender.SendTo(SEND_IP, SEND_PORT, sender.StringToBytes("data from client"));
            //receiver.listener.Client.Bind((IPEndPoint)receiver.listener.Client.LocalEndPoint);

        }


    }

    public static byte[] StringToBytes(string text)
    {
        return Encoding.ASCII.GetBytes(text);
    }

    private void sendCallback(IAsyncResult res)
    {
        //not in use
    }
    private void receive(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);
        //Console.WriteLine(RemoteIpEndPoint.Port);
        //Process codes
        //ReceiveDel(received, RemoteIpEndPoint);

        Debug.Log(Encoding.UTF8.GetString(received));
        //Debug.Log(DateTime.Now.ToUniversalTime());

        client.BeginReceive(new AsyncCallback(receive), null);
    }


}
