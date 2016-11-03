using UnityEngine;
using System.Collections;
using System;

public class BombPickup : ItemPickup
{
    protected override void Pickup(GameObject player)
    {
        PowerupUser PU = player.GetComponent<PowerupUser>();
        if (!PU) PU = player.transform.parent.gameObject.GetComponent<PowerupUser>();
        PU.EndPowerup();
        PU.SetPowerup(new BombPowerup(player));
        ScoreManager.gameData.BombsPicked++;
    }
}
