using UnityEngine;
using System.Collections;

public class BouncingBallPickup : ItemPickup {

    protected override void Pickup(GameObject player)
    {
        player.GetComponent<PowerupUser>().SetPowerup(new BouncingBallPowerup(player));
    }
}
