using UnityEngine;
using System.Collections;
using System;

public class TagMode : BasicMode
{
    //gameobject to hold the tagged graphics
    GameObject tagObj = null;

    //player gameobject that is currently the tagged
    public GameObject currentTag { get; private set; }

    float pointsPerSecond = 10;

    //tagged graphics 2d offset
    const float OFFSET_X = 0;
    const float OFFSET_Y = 0.3f;

    //increased movement speed for the tagged player
    const float MOVESPEED_MULTIPIER = 1.3f;

    int playerID = -1;



    public TagMode(ScoreManager.ScoreMode scoreMode = ScoreManager.ScoreMode.Health, int maxPoints = 200, int pointsPerKill = 5, float maxTime = 300)
    {
        this.scoreMode = scoreMode;
        this.maxPoints = maxPoints;
        this.pointsPerKill = pointsPerKill;
        this.maxTime = maxTime;
    }

    //get the ID(for scoring) from the current tagged player
    int GetPlayerID()
    {
        string playerName = currentTag.name;
        int tempPlayerID = -1;

        for (int i = 0; i < playerName.Length; i++)
        {
            //use the first digit in the name: "player1.../ player4..."
            if (Char.IsDigit(playerName[i]))
            {
                tempPlayerID = int.Parse(playerName[i].ToString()) - 1;
                break;
            }
        }
        return playerID;
    }

    //updated every frame
    public override void ScoreUpdate()
    {
        if (currentTag == null) return;

        if(playerID != -1) ScoreManager.instance.ChangeScore(playerID, pointsPerSecond * Time.deltaTime);  
    }
    
    //set a new player as the tagged player
    public void SetTag(GameObject player)
    {
        //if tag object does not exist yet, create one 
        if (tagObj == null)
        {
            tagObj = new GameObject("TAGobj");

            tagObj.transform.localPosition = new Vector2(OFFSET_X, OFFSET_Y);

            //set tag objects graphics
            SpriteRenderer tagObjRenderer = tagObj.AddComponent<SpriteRenderer>();
            tagObjRenderer.sprite = Resources.Load<Sprite>("Blocks/BasicBlock");    //default graphic
            tagObjRenderer.sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder;

            player.GetComponent<PlayerMovement>().SetMoveSpeed(MOVESPEED_MULTIPIER);

            playerID = GetPlayerID();

            //allow sprite to wrap around level
            LevelBounds.instance.RegisterObject(tagObj);
        }

        tagObj.transform.parent = player.transform;
        
        currentTag = player;
    }

    public void RemoveTag(GameObject player)
    {
        player.GetComponent<PlayerMovement>().ResetMoveSpeed();
    }

}

