using UnityEngine;
using System.Collections;

public class ChickenMode : BasicMode{

    public GameObject currentChicken { get; private set; }
    float pointsPerSecond = 10;


    public ChickenMode(ScoreManager.ScoreMode scoreMode = ScoreManager.ScoreMode.Points, int maxPoints = 200, int pointsPerKill = 5, float maxTime = 300)
    {
        this.scoreMode = scoreMode;
        this.maxPoints = maxPoints;
        this.pointsPerKill = pointsPerKill;
        this.maxTime = maxTime;
    }

    public override void ScoreUpdate()
    {
        if (currentChicken == null) return;
        int playerID = currentChicken.name.Contains("1") ? 0 : (currentChicken.name.Contains("2") ? 1 : (currentChicken.name.Contains("3") ? 2 : 3));
        ScoreManager.instance.ChangeScore(playerID, pointsPerSecond * Time.deltaTime);
    }
    
    public void SetChicken(GameObject player)
    {
        GameObject overlay = new GameObject("Chicken");
        overlay.transform.parent = player.transform;
        overlay.transform.localPosition = new Vector2(0, 0.3f);
        overlay.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Blocks/BasicBlock");
        overlay.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder;
        LevelBounds.instance.RegisterObject(overlay);
        currentChicken = player;

    }
    public void RemoveChicken(GameObject player)
    {
        if (currentChicken != player) return;
        currentChicken = null;
        GameObject chickenObj = GameObject.Find("Chicken");
        LevelBounds.instance.UnRegisterObject(chickenObj);
        if(chickenObj != null) GameObject.Destroy(chickenObj);
    }

}
