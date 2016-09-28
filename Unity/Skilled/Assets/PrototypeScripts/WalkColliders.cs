using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalkColliders : MonoBehaviour {

    List<GameObject> walkables;
    // Use this for initialization
    void Start () {
        MakeColliders();
	}
	
	void MakeColliders()
    {

        walkables = new List<GameObject>();
        walkables.AddRange(GameObject.FindGameObjectsWithTag("Walkable"));


        while (walkables.Count > 0)
        {
            GameObject current = walkables[0];

            GameObject colliderHolder = new GameObject("WalkingCollider");
            colliderHolder.transform.position = current.transform.position;

            BoxCollider2D currentCol = colliderHolder.AddComponent<BoxCollider2D>();
            currentCol.size = new Vector2(0.32f, 0.32f);
            walkables.Remove(current);

            int leftCount = FindLeftCount(current);
            int rightCount = FindRightCount(current);
            currentCol.size = new Vector2(currentCol.size.x + 0.32f * leftCount + 0.32f * rightCount, currentCol.size.y);
            currentCol.offset = new Vector2(currentCol.offset.x - 0.16f * leftCount + 0.16f * rightCount, currentCol.offset.y);
            //current.transform.position += Vector3.up * 0.1f;
        }

    }

    void MakeCollider()
    {

    }
    int FindLeftCount(GameObject current, int count = 0)
    {
        //current.GetComponent<SpriteRenderer>().sprite = null;
        GameObject leftNext = walkables.Find(x => x.transform.position.x < current.transform.position.x && Vector2.Distance((Vector2)x.transform.position, (Vector2)current.transform.position) < 0.33f);
        if (leftNext == null) return count;
        walkables.Remove(leftNext);
        return FindLeftCount(leftNext, count + 1);
    }

    int FindRightCount(GameObject current, int count = 0)
    {
        //current.GetComponent<SpriteRenderer>().sprite = null;
        GameObject leftNext = walkables.Find(x => x.transform.position.x > current.transform.position.x && Vector2.Distance((Vector2)x.transform.position, (Vector2)current.transform.position) < 0.33f);
        if (leftNext == null)
        {
            //TODO find edge of level block and connect it somehow to other end block (if there is one) (now player can get stuck on edge)
            return count;
        }
        walkables.Remove(leftNext);
        return FindRightCount(leftNext, count + 1);
    }

}
