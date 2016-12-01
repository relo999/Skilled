using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.Threading;

public struct ConnectionInfo
{

}

public class NetConnector {

	public static void Connect(IPEndPoint endpoint, Action<ConnectionInfo> callback)
    {
        new Thread(() => TryConnect(endpoint, callback)).Start();   
    }

    private static void TryConnect(IPEndPoint endpoint, Action<ConnectionInfo> callback)
    {
        ConnectionInfo cInfo = new ConnectionInfo();

        bool tryingConnection = true;
        while(tryingConnection)
        {

        }
        callback(cInfo);
    }

}
