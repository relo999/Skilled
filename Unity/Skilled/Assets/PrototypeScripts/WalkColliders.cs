using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalkColliders : MonoBehaviour {

    List<GameObject> walkables;
    const float BLOCK_SIZE = 0.32f;
    const float HALF_BLOCK_SIZE = BLOCK_SIZE / 2f;

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
            //get any block in the level
            GameObject current = walkables[0];

            //make an object to hold the new collider (so it doesn't interfere with potential scripts attached to the blocks)
            GameObject colliderHolder = new GameObject("WalkingCollider");
            colliderHolder.transform.position = current.transform.position;

            //make a new collider with size equal to the tile size of the blocks
            BoxCollider2D currentCol = colliderHolder.AddComponent<BoxCollider2D>();
            currentCol.size = new Vector2(BLOCK_SIZE, BLOCK_SIZE);
            walkables.Remove(current);

            //search for amount of neighbouring blocks and change collider size accordingly
            int leftCount = FindLeftCount(current);
            int rightCount = FindRightCount(current);
            currentCol.size = new Vector2(currentCol.size.x + BLOCK_SIZE * leftCount + BLOCK_SIZE * rightCount, currentCol.size.y);
            currentCol.offset = new Vector2(currentCol.offset.x - HALF_BLOCK_SIZE * leftCount + HALF_BLOCK_SIZE * rightCount, currentCol.offset.y);
        }

    }

    int FindLeftCount(GameObject current, int count = 0)
    {
        //current.GetComponent<SpriteRenderer>().sprite = null; //debug only

        //if the next predicted position is outside the level, add an extra collider bit to ensure smooth transition when wrapping to other side of the level
        float nextXPos = current.transform.position.x - 0.32f;
        if (nextXPos < LevelBounds.instance.bounds.center.x - LevelBounds.instance.bounds.size.x / 2f) return count + 1;

        //find a block that is directly next to the currently selected block
        GameObject leftNext = walkables.Find(x => x.transform.position.x < current.transform.position.x && Vector2.Distance((Vector2)x.transform.position, (Vector2)current.transform.position) < 0.33f);
        if (leftNext == null) return count;
        walkables.Remove(leftNext);
        return FindLeftCount(leftNext, count + 1);
    }

    int FindRightCount(GameObject current, int count = 0)
    {
        //current.GetComponent<SpriteRenderer>().sprite = null; //debug only

        float nextXPos = current.transform.position.x + 0.32f;
        if (nextXPos > LevelBounds.instance.bounds.center.x + LevelBounds.instance.bounds.size.x / 2f) return count + 1;

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
