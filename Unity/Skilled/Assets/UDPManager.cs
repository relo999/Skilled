using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class UDPManager : MonoBehaviour {
    Receiver receiver = new Receiver();
    Sender sender = new Sender();
    int SEND_PORT = 11000;
    string SEND_IP = "12345";    //TODO HOME SERVER IP
    int RECEIVE_PORT = 12000;

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
        SEND_IP = LocalIP;  //TODO REMOVE THIS WHEN HAVE HOME SERVER IP

        receiver.ReceiveDel += WriteDebug;
        //receiver.InitListener(RECEIVE_PORT);
        sender.SendTo(SEND_IP, SEND_PORT, sender.StringToBytes("connect"));
        receiver.InitListener(((IPEndPoint)sender.sending_socket.LocalEndPoint).Port);
        //receiver.InitListener(9999);
        
    }

    void WriteDebug(byte[] data)
    {
        Debug.Log(Encoding.UTF8.GetString(data));
    }



    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            sender.SendTo(SEND_IP, SEND_PORT, sender.StringToBytes("data from client"));
            //receiver.listener.Client.Bind((IPEndPoint)receiver.listener.Client.LocalEndPoint);

        }


    }



    public class Receiver
    {
        public int ListenPort;
        public UdpClient listener;
        //public Socket sending_socket;
        IPEndPoint groupEP;
        public delegate void ProcessReceivedData(byte[] data);
        public ProcessReceivedData ReceiveDel;

        public Receiver()
        {
            ReceiveDel = processReceivedData;
            //Sending = SendExtra;
        }

        public void InitListener(int Port)
        {
            ListenPort = Port;
            //ListenPort = 15000;
            //listener.se
            listener = new UdpClient();
            listener.Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            listener.ExclusiveAddressUse = false;
            listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            groupEP = new IPEndPoint(IPAddress.Any, ListenPort);
            listener.Client.Bind(groupEP);
            Debug.Log("now listening to: " + ListenPort);
             listener.BeginReceive(new AsyncCallback(receive), null);
        }


        ~Receiver()
        {

        }
        private void receive(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            byte[] received = listener.EndReceive(res, ref RemoteIpEndPoint);

            //Process codes
            ReceiveDel(received);
            //Debug.Log(Encoding.UTF8.GetString(received));
            //Debug.Log(DateTime.Now.ToUniversalTime());
            listener.BeginReceive(new AsyncCallback(receive), null);
        }
        private void processReceivedData(byte[] data)
        {
            //string stringData = Encoding.UTF8.GetString(data);
        }

        /*
        public void SendTo(string IP, int Port, byte[] data)
        {

            IPAddress send_to_address = IPAddress.Parse(IP);
            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, Port);

            listener.Client.SendTo(data, sending_end_point);
            Sending();
        }
        public delegate void SendDelegate();
        public SendDelegate Sending;

        void SendExtra()
        {
            //empty
        }*/


    }

    public class Sender
    {
        public Socket sending_socket;
        public delegate void SendDelegate();
        public SendDelegate Sending;

        public Sender()
        {
            sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Sending = SendExtra;
            sending_socket.ExclusiveAddressUse = false;
            sending_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }
        ~Sender()
        {

        }
        public byte[] StringToBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        void SendExtra()
        {
            //empty
        }

        public void SendTo(string IP, int Port, byte[] data)
        {
            
            IPAddress send_to_address = IPAddress.Parse(IP);
            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, Port);

            sending_socket.SendTo(data, sending_end_point);
            Sending();
        }

    }
}
