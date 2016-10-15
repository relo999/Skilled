using UnityEngine;
using System.Collections;

public class ShieldPickup : ItemPickup {

    protected override void Pickup(GameObject player)
    {
        PowerupUser PU = player.GetComponent<PowerupUser>();
        PU.EndPowerup();
        PU.SetPowerup(new ShieldPowerup(player));
        ScoreManager.gameData.ShieldsPicked++;
    }
}
