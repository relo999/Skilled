using UnityEngine;
using System.Collections;

public class TagMode : BasicMode
{
    GameObject tagObj = null;
    public GameObject currentTag { get; private set; }
    float pointsPerSecond = 10;

    public TagMode(ScoreManager.ScoreMode scoreMode = ScoreManager.ScoreMode.Health, int maxPoints = 200, int pointsPerKill = 5, float maxTime = 300)
    {
        this.scoreMode = scoreMode;
        this.maxPoints = maxPoints;
        this.pointsPerKill = pointsPerKill;
        this.maxTime = maxTime;
    }

    public override void ScoreUpdate()
    {
        if (currentTag == null) return;
        int playerID = currentTag.name.Contains("1") ? 0 : (currentTag.name.Contains("2") ? 1 : (currentTag.name.Contains("3") ? 2 : 3));
        ScoreManager.instance.ChangeScore(playerID, pointsPerSecond * Time.deltaTime);
    }

    public void SetTag(GameObject player)
    {
        if (tagObj == null)
        {
            Debug.Log("new");
            GameObject overlay = new GameObject("TAGobj");
            overlay.transform.parent = player.transform;
            overlay.transform.localPosition = new Vector2(0, 0.3f);
            overlay.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Blocks/BasicBlock");
            overlay.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder;
            tagObj = overlay;
            player.GetComponent<PlayerMovement>().SetMoveSpeed(1.3f);
            //LevelBounds.instance.RegisterObject(overlay);
        }
        else
        {
            Debug.Log("old");
            tagObj.transform.parent = player.transform;
            tagObj.transform.localPosition = new Vector2(0, 0.3f);
        }
        currentTag = player;

    }
    public void RemoveTag(GameObject player)
    {
        player.GetComponent<PlayerMovement>().ResetMoveSpeed();
        /*
        if (currentTag != player) return;
        currentTag = null;
        GameObject tagObj = GameObject.Find("TAGobj");
        LevelBounds.instance.UnRegisterObject(tagObj);
        if (tagObj != null) GameObject.Destroy(tagObj);*/
    }

}

