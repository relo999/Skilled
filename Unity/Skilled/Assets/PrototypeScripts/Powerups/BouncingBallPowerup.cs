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
        owner.GetComponent<SpriteOverlay>().SetSprite("PowerUps/Powers/Shooter/Shooter", owner.GetComponent<PlayerHit>().color);
    }

    public override void End()
    {
        owner.GetComponent<SpriteOverlay>().DestroySprite();
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
        ball.GetComponent<Bounce>().goingRight = owner.GetComponent<PlayerMovement>().LastMovedRight;
        Physics2D.IgnoreCollision(ball.GetComponent<CircleCollider2D>(), owner.GetComponent<BoxCollider2D>());
        Physics2D.IgnoreCollision(ball.GetComponent<CircleCollider2D>(), owner.GetComponentInChildren<CircleCollider2D>());
        ScoreManager.gameData.BouncesUsed++;
        inGameBalls[availableID] = ball;

        
    }

 

}
