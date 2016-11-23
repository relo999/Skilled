using UnityEngine;
using System.Collections;

public class EnemySplit : EnemyBase {


    int[] splitIDs = new int[2];

    // Use this for initialization
    void Start () {
        Init();
        SetSplitIDs();
	}

    void SetSplitIDs()
    {
        splitIDs[0] = (int)speed + 1;
        splitIDs[1] = (int)speed == 0 || (int)speed == 1 ? 0 : 2;
    }

    protected override void OnSpeedChanged()
    {
        SetSplitIDs();
    }

    protected override void OnPlayerHit(PlayerHit player, bool above)
    {
        if (!above)
        {
            player.OnDeath(gameObject);
        }
    }
    protected override void HitCallback(PlayerHit player)
    {
        int splitIn = 2;
        Sprite smallSplitSprite = Resources.Load<Sprite>("SinglePlayer/Enemies/Splitter/SSplitter");
        Object enemyPrefab = Resources.Load("Prefabs/Enemy1");
        for (int i = 0; i <splitIn; i++)
        {
            GameObject split = GameObject.Instantiate(enemyPrefab) as GameObject;
            split.transform.position = this.transform.position;
            split.name = splitIDs[i] + " SmallSplitter";
            split.GetComponent<SpriteRenderer>().sprite = smallSplitSprite;
            EnemyBase enemyBase = split.GetComponent<EnemyBase>();
            enemyBase.Init();
            enemyBase.CorrectSprite();
            enemyBase.DoStun = false;
            enemyBase.spawnInvulnerability = 0.1f;
            enemyBase.faceRight = i == 0 || i % 2 == 0; //alternating facing left and right
        }
       

        

        player.BounceUp(player.gameObject);
        OnDeath(player);
    }

    protected override void EnemyUpdate()
    {

    }


}
