using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalkColliders : MonoBehaviour {

    List<GameObject> walkables;
    List<GameObject> passthroughs;
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
        GameObject[] temps = new GameObject[walkables.Count];
        walkables.CopyTo(temps);
        List<GameObject> walkablesCopy = new List<GameObject>();
        walkablesCopy.AddRange(temps);
        GameObject allColliderHolder = new GameObject("Collider holder");
        allColliderHolder.transform.localPosition = Vector2.zero;
        while (walkables.Count > 0)
        {
            //get any block in the level
            GameObject current = walkables[0];

            //make an object to hold the new collider (so it doesn't interfere with potential scripts attached to the blocks)
            GameObject colliderHolder = new GameObject("WalkingColliderHorizontal");
            colliderHolder.transform.position = current.transform.position;
            colliderHolder.transform.parent = allColliderHolder.transform;

            //make a new collider with size equal to the tile size of the blocks
            BoxCollider2D currentCol = colliderHolder.AddComponent<BoxCollider2D>();
            currentCol.size = new Vector2(BLOCK_SIZE, BLOCK_SIZE);
            walkables.Remove(current);
            walkablesCopy.Remove(current);

            //search for amount of neighbouring blocks and change collider size accordingly
            int leftCount = FindLeftCount(walkables, current);
            int rightCount = FindRightCount(walkables, current);

            currentCol.size = new Vector2(currentCol.size.x + BLOCK_SIZE * leftCount + BLOCK_SIZE * rightCount, currentCol.size.y);
            currentCol.offset = new Vector2(currentCol.offset.x - HALF_BLOCK_SIZE * leftCount + HALF_BLOCK_SIZE * rightCount, currentCol.offset.y);


            int upCount = FindUpCount(walkablesCopy, current);
            int downCount = FindDownCount(walkablesCopy, current);
            if(upCount > 0 || downCount > 0)
            {
                GameObject colliderHolderV = new GameObject("WalkingColliderVertical"); 
                colliderHolderV.transform.position = current.transform.position;
                colliderHolderV.transform.parent = allColliderHolder.transform;
                BoxCollider2D currentColV = colliderHolderV.AddComponent<BoxCollider2D>();
                currentColV.size = new Vector2(BLOCK_SIZE, BLOCK_SIZE);
                currentColV.size = new Vector2(currentColV.size.x + 0.05f, (currentColV.size.y + BLOCK_SIZE * upCount + BLOCK_SIZE * downCount) -0.1f);
                currentColV.offset = new Vector2(currentColV.offset.x, currentColV.offset.y + HALF_BLOCK_SIZE * upCount - HALF_BLOCK_SIZE * downCount);
            }

        }



        passthroughs = new List<GameObject>();
        passthroughs.AddRange(GameObject.FindGameObjectsWithTag("PassThrough"));


        while (passthroughs.Count > 0)
        {
            //get any block in the level
            GameObject current = passthroughs[0];

            //make an object to hold the new collider (so it doesn't interfere with potential scripts attached to the blocks)
            GameObject colliderHolder = new GameObject("PassThroughCollider");
            colliderHolder.transform.position = current.transform.position;
            colliderHolder.transform.parent = allColliderHolder.transform;

            //make a new collider with size equal to the tile size of the blocks
            BoxCollider2D currentCol = colliderHolder.AddComponent<BoxCollider2D>();
            currentCol.size = new Vector2(BLOCK_SIZE, 0.05f);

            BoxCollider2D currentColTrigger = colliderHolder.AddComponent<BoxCollider2D>();
            currentColTrigger.isTrigger = true;
            currentColTrigger.size = new Vector2(BLOCK_SIZE, 0.25f);

            DisableColliders(current);

            colliderHolder.AddComponent<PassThrough>();
            passthroughs.Remove(current);

            //search for amount of neighbouring blocks and change collider size accordingly
            int leftCount = FindLeftCount(passthroughs, current);
            int rightCount = FindRightCount(passthroughs, current);

            currentCol.size = new Vector2(currentCol.size.x + BLOCK_SIZE * leftCount + BLOCK_SIZE * rightCount, currentCol.size.y);
            currentCol.offset = new Vector2(currentCol.offset.x - HALF_BLOCK_SIZE * leftCount + HALF_BLOCK_SIZE * rightCount, 0.13f);

            currentColTrigger.size = new Vector2(currentColTrigger.size.x + BLOCK_SIZE * leftCount + BLOCK_SIZE * rightCount + BLOCK_SIZE*2, currentColTrigger.size.y);
            currentColTrigger.offset = new Vector2(currentColTrigger.offset.x - HALF_BLOCK_SIZE * leftCount + HALF_BLOCK_SIZE * rightCount, -0.02f);

        }


    }

    void DisableColliders(GameObject current)
    {
        //disable indiviual colliders/ scripts and put them on the collective collider
        GameObject.Destroy(current.GetComponent<PassThrough>());
        BoxCollider2D[] colliders = current.GetComponents<BoxCollider2D>();
        for (int i = colliders.Length-1; i >= 0; i--)
        {
            GameObject.Destroy(colliders[i]);
        }
    }

    int FindUpCount(List<GameObject> list, GameObject current, int count = 0)
    {
        //current.GetComponent<SpriteRenderer>().sprite = null; //debug only

        //if the next predicted position is outside the level, add an extra collider bit to ensure smooth transition when wrapping to other side of the level
        float nextYPos = current.transform.position.y + 0.32f;
        if (nextYPos > LevelBounds.instance.bounds.center.y + LevelBounds.instance.bounds.size.y / 2f) return count + 1;

        //find a block that is directly next to the currently selected block
        GameObject leftNext = list.Find(x => x.transform.position.y > current.transform.position.y && Vector2.Distance((Vector2)x.transform.position, (Vector2)current.transform.position) < 0.33f);
        if (leftNext == null) return count;
        if (list == passthroughs) DisableColliders(leftNext);
        list.Remove(leftNext);
        return FindUpCount(list, leftNext, count + 1);
    }

    int FindDownCount(List<GameObject> list, GameObject current, int count = 0)
    {
        //current.GetComponent<SpriteRenderer>().sprite = null; //debug only

        //if the next predicted position is outside the level, add an extra collider bit to ensure smooth transition when wrapping to other side of the level
        float nextYPos = current.transform.position.y - 0.32f;
        if (nextYPos < LevelBounds.instance.bounds.center.y - LevelBounds.instance.bounds.size.y / 2f) return count + 1;

        //find a block that is directly next to the currently selected block
        GameObject leftNext = list.Find(x => x.transform.position.y < current.transform.position.y && Vector2.Distance((Vector2)x.transform.position, (Vector2)current.transform.position) < 0.33f);
        if (leftNext == null) return count;
        if (list == passthroughs) DisableColliders(leftNext);
        list.Remove(leftNext);
        return FindDownCount(list, leftNext, count + 1);
    }

    int FindLeftCount(List<GameObject> list, GameObject current, int count = 0)
    {
        //current.GetComponent<SpriteRenderer>().sprite = null; //debug only

        //if the next predicted position is outside the level, add an extra collider bit to ensure smooth transition when wrapping to other side of the level
        float nextXPos = current.transform.position.x - 0.32f;
        if (nextXPos < LevelBounds.instance.bounds.center.x - LevelBounds.instance.bounds.size.x / 2f) return count + 1;

        //find a block that is directly next to the currently selected block
        GameObject leftNext = list.Find(x => x.transform.position.x < current.transform.position.x && Vector2.Distance((Vector2)x.transform.position, (Vector2)current.transform.position) < 0.33f);
        if (leftNext == null) return count;
        if(list == passthroughs) DisableColliders(leftNext);
        list.Remove(leftNext);
        return FindLeftCount(list, leftNext, count + 1);
    }

    int FindRightCount(List<GameObject> list, GameObject current, int count = 0)
    {
        //current.GetComponent<SpriteRenderer>().sprite = null; //debug only

        float nextXPos = current.transform.position.x + 0.32f;
        if (nextXPos > LevelBounds.instance.bounds.center.x + LevelBounds.instance.bounds.size.x / 2f) return count + 1;

        GameObject rightNext = list.Find(x => x.transform.position.x > current.transform.position.x && Vector2.Distance((Vector2)x.transform.position, (Vector2)current.transform.position) < 0.33f);
        if (rightNext == null) return count;
        if (list == passthroughs) DisableColliders(rightNext);
        list.Remove(rightNext);
        return FindRightCount(list, rightNext, count + 1);
    }

}
