using UnityEngine;
using System.Collections;
using System;

public class BouncingBallPowerup : PowerupBase {

    public BouncingBallPowerup(GameObject owner) : base(owner) { }
    float _throwForce = 50f;
    public int MaxBalls = 5;
    GameObject[] inGameBalls;

    protected override void Start()
    {
        inGameBalls = new GameObject[MaxBalls];
        //_cooldown = 0.3f;
    }

    public override void End()
    {

    }


    protected override void Activate()
    {
        if(MaxBalls != Bounce.Max_Balls && Bounce.Max_Balls != 0)
        {
            MaxBalls = Bounce.Max_Balls;
            GameObject[] temp = new GameObject[MaxBalls];
            Array.Copy(inGameBalls, temp, MaxBalls);

            inGameBalls = new GameObject[MaxBalls];
            for (int i = 0; i < MaxBalls; i++)
            {
                inGameBalls[i] = temp[i];
            }
        }
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
        bRigid.velocity = new Vector2(5 * rightModifier, 5) * _throwForce;

        inGameBalls[availableID] = ball;

        
    }

 

}
