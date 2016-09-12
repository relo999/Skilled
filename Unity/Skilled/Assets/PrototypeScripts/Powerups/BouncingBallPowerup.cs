using UnityEngine;
using System.Collections;
using System;

public class BouncingBallPowerup : PowerupBase {

    public BouncingBallPowerup(GameObject owner) : base(owner) { }
    float _throwForce = 50f;
    const int _MAXBALLS = 5;
    GameObject[] inGameBalls;

    protected override void Start()
    {
        inGameBalls = new GameObject[_MAXBALLS];
        //_cooldown = 0.3f;
    }


    protected override void Activate()
    {
        int availableID = -1;
        for (int i = 0; i < inGameBalls.GetLength(0); i++)
        {
            if (inGameBalls[i] == null)
            {
                availableID = i;
                break;
            }
        }
        if (availableID == -1) return;

        int rightModifier = owner.GetComponent<PlayerMovement>().LastMovedRight ? 1 : -1;
        float spawnDistance = 0.33f;
        GameObject ball = GameObject.Instantiate(PowerupManager.instance.PowerupsPrefabs[1], owner.transform.position + Vector3.right * rightModifier * spawnDistance, Quaternion.identity) as GameObject;
        ball.GetComponent<Bounce>().owner = owner;

        Rigidbody2D bRigid = ball.GetComponent<Rigidbody2D>();
        bRigid.AddForce(new Vector2(5 * rightModifier, 5) * _throwForce);

        inGameBalls[availableID] = ball;

        
    }

 

}
