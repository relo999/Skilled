using UnityEngine;
using System.Collections;

public class BouncingBallPickup : ItemPickup {

    protected override void Pickup(GameObject player)
    {
        PowerupUser PU = player.GetComponent<PowerupUser>();
        PU.EndPowerup();
        PU.SetPowerup(new BouncingBallPowerup(player));
    }
}
