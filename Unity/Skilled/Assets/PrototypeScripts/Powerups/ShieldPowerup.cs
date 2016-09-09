﻿using UnityEngine;
using System.Collections;
using System;

public class ShieldPowerup : PowerupBase
{
    public ShieldPowerup(GameObject owner) : base(owner) { }
    PlayerHit hit;
    int playerID;
    protected override void Start()
    {
        hit = owner.GetComponent<PlayerHit>();
        hit.IC += ShieldEnd;
        hit.SetImmunity(true, false);
        playerID = owner.name.Contains("1") ? 0 : (owner.name.Contains("2") ? 1 : (owner.name.Contains("3") ? 2 : 3));

        if(PowerupManager.instance.OriginalSprites[playerID] == null)
            PowerupManager.instance.OriginalSprites[playerID] = owner.GetComponent<SpriteRenderer>().sprite;
        owner.GetComponent<SpriteRenderer>().sprite = PowerupManager.instance.ShieldSprites[playerID];
    }
    protected override void Activate()
    {
        //empty
    }

    public void ShieldEnd()
    {
        owner.GetComponent<PlayerHit>().IC -= ShieldEnd;
        owner.GetComponent<PowerupUser>().SetPowerup(new PowerupBase.EmptyPowerup(owner));
        owner.GetComponent<SpriteRenderer>().sprite = PowerupManager.instance.OriginalSprites[playerID];
    }
}