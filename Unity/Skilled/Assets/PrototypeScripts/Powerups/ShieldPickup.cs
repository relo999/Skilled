using UnityEngine;
using System.Collections;

public class ShieldPickup : ItemPickup {

    protected override void Pickup(GameObject player)
    {
        player.GetComponent<PowerupUser>().SetPowerup(new ShieldPowerup(player));
    }
}
