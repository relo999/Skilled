using UnityEngine;
using System.Collections;
using System;

public class BombPowerup : PowerupBase
{
    bool _holdingBomb = false;
    GameObject bomb = null;
    private float _throwForce = 50f;
    public BombPowerup(GameObject owner) : base(owner)
    {
        _cooldown = 0.5f;   //override cooldown for each powerup
    }

    protected override void Activate()
    {
        if(!_holdingBomb)
        {
            bomb = GameObject.Instantiate(PowerupManager.instance.PowerupsPrefabs[0], owner.transform.position, Quaternion.identity) as GameObject; //TODO enums etc to choose powerup
            bomb.transform.parent = owner.transform;
            _holdingBomb = true;
            return;
        }
        else
        {
            bomb.transform.parent = null;
            Rigidbody2D bRigid =  bomb.AddComponent<Rigidbody2D>();
            bRigid.AddForce(new Vector2(5 * (owner.GetComponent<PlayerMovement>().LastMovedRight ? 1 : -1), 5) * _throwForce);   
            bomb.GetComponent<BoxCollider2D>().isTrigger = false;
            _holdingBomb = false;
            return;
        }
    }
}
