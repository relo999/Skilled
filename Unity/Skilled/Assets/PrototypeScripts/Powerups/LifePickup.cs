using UnityEngine;
using System.Collections;

public class LifePickup : ItemPickup {

    protected override void Pickup(GameObject player)
    {
        PlayerMovement mov = player.GetComponent<PlayerMovement>();
        if (!mov) mov = player.transform.parent.gameObject.GetComponent<PlayerMovement>();
        ScoreManager.instance.ChangeScore((int)mov.playerID, ScoreManager.instance.scoreMode == ScoreManager.ScoreMode.Health? -1 : 1);
        GameObject temp = new GameObject("Life animation");
        temp.transform.position = transform.position;
        temp.AddComponent<SpriteRenderer>();
        SheetAnimation SA = temp.AddComponent<SheetAnimation>();
        float time = 1.5f;
        SA.PlayAnimationUnC("Powerups/Powers/LifeUp/Life", false, 8f/time);
        temp.AddComponent<DestroyAfterSeconds>().Seconds = time;
        ScoreManager.gameData.LivesPicked++;
    }

 
}
