using UnityEngine;
using System.Collections;
using System;

public class BombPowerup : PowerupBase
{
    public BombPowerup()
    {
        _cooldown = 0.5f;   //override cooldown for each powerup
    }

    protected override void Activate()
    {
        Debug.Log("Bomb used");
    }
}
