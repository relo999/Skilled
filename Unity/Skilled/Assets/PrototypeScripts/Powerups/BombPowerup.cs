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
        _cooldown = 3f;   //override cooldown for each powerup
        owner.GetComponent<SpriteOverlay>().SetSprite("PowerUps/Powers/BombThrow/Bomb", owner.GetComponent<PlayerHit>().color);
    }

    public override void End()
    {
        owner.GetComponent<SpriteOverlay>().DestroySprite();
        if(bomb != null && _holdingBomb)
        {
            GameObject.Destroy(bomb);
        }
    }

    protected override void Activate()
    {
        if(!_holdingBomb)
        {
            bomb = GameObject.Instantiate(PowerupManager.instance.PowerupsPrefabs[0], owner.transform.position + Vector3.up * 0.2f, Quaternion.identity) as GameObject; //TODO enums etc to choose powerup
            bomb.transform.parent = owner.transform;
            _holdingBomb = true;
            return;
        }
        else
        {
            bomb.transform.parent = null;
            Rigidbody2D bRigid =  bomb.AddComponent<Rigidbody2D>();
            bRigid.freezeRotation = true;
            bRigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            bRigid.interpolation = RigidbodyInterpolation2D.Interpolate;
            bRigid.AddForce(new Vector2(5 * (owner.GetComponent<PlayerMovement>().LastMovedRight ? 1 : -1), 5) * _throwForce);   
            bomb.GetComponent<BoxCollider2D>().isTrigger = false;
            bomb.GetComponent<CircleCollider2D>().isTrigger = false;
            bomb.GetComponent<BombExplode>().StartCountdown(owner);
            bomb.AddComponent<LoopOutLevel>();
            _holdingBomb = false;
            _currentCooldown = _cooldown;
            ScoreManager.gameData.BombsUsed++;
            return;
        }
    }
}
