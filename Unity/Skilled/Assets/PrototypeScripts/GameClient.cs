using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class GameClient : NetworkBase {

    PlayerMovement ownMovement;
    protected override void receiveCallback(IAsyncResult res)
    {

    }
    public override void Update()
    {
        Debug.Log("UPDATE");
        //SendToClient(connectedClient, Encoding.ASCII.GetBytes("send test...."));
        //Debug.Log("Sent player input");
       // if (ownMovement == null) SetPlayerID();
        //SendPlayerInput(ownMovement.input);
        //serverClient.BeginReceive(new AsyncCallback(receive), null);
        //IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8000);
        //byte[] data = serverClient.Receive(ref endpoint);
        //string stringData = Encoding.UTF8.GetString(data);
        //Debug.Log("received: " + stringData);
    }
    public GameClient(UdpClient client) : base(client)
    {
        //serverClient.BeginReceive(new AsyncCallback(receive), null);
    }
    public void SetPlayerID()
    { 
        ownMovement = Array.Find(GameObject.FindObjectsOfType<PlayerMovement>(), x => (int)x.playerID == this.playerID);
    }
    public void SendPlayerInput(PlayerInput input)
    {
        SendToClient(connectedClient, SerializeClass(input));
    }

    protected override void HandleSerializedData(SerializeBase data)
    {
        return;
        Type t = data.GetType();


        if (t.Equals(typeof(PlayerUpdates)))
        {
            //pseudo code:
            PlayerUpdates updates = (PlayerUpdates)data;
            for (int i = 0; i < updates.PlayerInfos.Length; i++)
            {
            PlayerInfo info = updates.PlayerInfos[i];
            GameObject player = Array.Find(GameObject.FindObjectsOfType<PlayerMovement>(), x => (int)x.playerID == info.playerID).gameObject;
                player.transform.position = new Vector3(info.xPos, info.yPos, player.transform.position.z);
            //find player object and execute set position method... players[info.playerID].SetPosition(info.xPos, info.yPos);
            //no need to update server after this
            }
        }
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
    }


}
