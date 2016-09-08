using UnityEngine;
using System.Collections;

public class ItemBlock : MonoBehaviour {

    void DropItem(Vector2 position) //TODO Which item to drop?
    {
        int itemToDrop = 0; //first item (bomb?)
        itemToDrop = Random.Range(0, ScoreManager.instance.itemPickups.GetLength(0));
        GameObject item = GameObject.Instantiate(ScoreManager.instance.itemPickups[itemToDrop],transform.position + Vector3.up * 0.32f, Quaternion.identity) as GameObject;

    }

    void OnCollisionEnter2D(Collision2D c)
    {
        //copied HitAbove to avoid script execution order issues for now
        PlayerHit hit = c.collider.gameObject.GetComponent<PlayerHit>();
        if (!hit) return;

        if (hit.gameObject.transform.position.y < this.transform.position.y)
        {
            GetComponent<Animation>().Play();
            PlayerHit[] hits = FindObjectsOfType<PlayerHit>();
            for (int i = hits.GetLength(0) - 1; i >= 0; i--)
            {
                if (GetComponent<Collider2D>().IsTouching(hits[i].gameObject.GetComponent<Collider2D>()) && hits[i].transform.position.y > transform.position.y && hits[i].gameObject.GetComponent<PlayerMovement>().Grounded)
                {
                    hits[i].OnDeath(hit.gameObject);
                }
            }
            hit.GetComponent<PlayerMovement>().ForceStopJump();
            DropItem(transform.position);
            GameObject.Destroy(gameObject);

        }
    }
}
