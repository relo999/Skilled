using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;

public class GameClient : NetworkBase {

    int playerID = 0;
    PlayerMovement ownMovement;
    protected override void receiveCallback(IAsyncResult res)
    {

    }
    public override void Update()
    {
        SendPlayerInput(ownMovement.input);
        serverClient.BeginReceive(receive, null);
    }
    public GameClient(UdpClient client) : base(client)
    { 

    }
    public void SendPlayerInput(PlayerInput input)
    {
        SendToClient(Mainserver, SerializeClass(input));
    }

    protected override void HandleSerializedData(SerializeBase data)
    {
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
