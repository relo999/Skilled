using UnityEngine;
using System.Collections;
using System;

public class BombPickup : ItemPickup
{
    protected override void Pickup(GameObject player)
    {
        player.GetComponent<PowerupUser>().SetPowerup(new BombPowerup(player));
    }
}
