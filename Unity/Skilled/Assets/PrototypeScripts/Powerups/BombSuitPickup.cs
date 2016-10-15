using UnityEngine;
using System.Collections;
using System;

public class BombSuitPickup : ItemPickup
{
    protected override void Pickup(GameObject player)
    {
        PowerupUser PU = player.GetComponent<PowerupUser>();
        PU.EndPowerup();
        PU.SetPowerup(new BombSuitPowerup(player));
        ScoreManager.gameData.BombSuitPicked++;
    }
}
