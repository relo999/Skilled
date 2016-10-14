using UnityEngine;
using System.Collections;

public class LifePickup : ItemPickup {

    protected override void Pickup(GameObject player)
    {
        ScoreManager.instance.ChangeScore((int)player.GetComponent<PlayerMovement>().playerID, -1);
    }
}
