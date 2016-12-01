using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OwnedMode : BasicMode
{
    struct Own
    {
        public Own(int playerID, int ownerID, GameObject overlay)
        {
            this.playerID = playerID;
            this.ownerID = ownerID;
            this.overlay = overlay;
        }
        public int playerID;
        public int ownerID;
        public GameObject overlay;
    }
    List<Own> ownedList = new List<Own>();
    
    float pointsPerSecond = 10;


    public OwnedMode(ScoreManager.ScoreMode scoreMode = ScoreManager.ScoreMode.Points, int maxPoints = 200, int pointsPerKill = 5, float maxTime = 300)
    {
        this.scoreMode = scoreMode;
        this.maxPoints = maxPoints;
        this.pointsPerKill = pointsPerKill;
        this.maxTime = maxTime;
    }

    public override void ScoreUpdate()
    {
        foreach(Own own in ownedList)
        {
            ScoreManager.instance.ChangeScore(own.ownerID, pointsPerSecond * Time.deltaTime);
        }
    }

    public void SetOwned(GameObject player, GameObject owner)
    {
        int playerID = player.name.Contains("1") ? 0 : (player.name.Contains("2") ? 1 : (player.name.Contains("3") ? 2 : 3));
        int ownerID = owner.name.Contains("1") ? 0 : (owner.name.Contains("2") ? 1 : (owner.name.Contains("3") ? 2 : 3));
        Own own = ownedList.Find(x => x.playerID == playerID);
        if (own.playerID == 0 && own.ownerID == 0)
        {
            own = new Own(playerID, ownerID, null);
            ownedList.Add(own);
        }
        else
        {
            own.ownerID = ownerID;
        }

        if (own.overlay != null)
        {
            LevelBounds.instance.UnRegisterObject(own.overlay.GetComponentInChildren<SpriteRenderer>().gameObject);
            GameObject.Destroy(own.overlay);
        }
        GameObject overlay = new GameObject("Owned");
        overlay.transform.parent = player.transform;
        
        overlay.transform.localPosition = new Vector2(0, 0.3f);
        GameObject overlayRenderer = new GameObject("OwnedRenderer");
        overlayRenderer.transform.parent = overlay.transform;
        //overlayRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        overlayRenderer.transform.localPosition = Vector2.zero;
        overlayRenderer.AddComponent<SpriteRenderer>().sprite = owner.GetComponent<SpriteRenderer>().sprite;
        overlayRenderer.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder;
        own.overlay = overlay;
        LevelBounds.instance.RegisterObject(overlayRenderer);


    }


}
