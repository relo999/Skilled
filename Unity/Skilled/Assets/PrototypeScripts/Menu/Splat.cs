using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Splat : MonoBehaviour{

     Vector2 SCREEN_CENTER_WORLD = new Vector2(0, 2.5f);
     List<SplatPart> splatParts = new List<SplatPart>();

	public void DoSplat(Vector2 startPos, float delayS = 0)
    {
        int splatID = Random.Range(0, 5);
        char colorChar = ((SheetAnimation.PlayerColor)splatID).ToString().ToUpper()[0];
        Sprite[] splats = Resources.LoadAll<Sprite>("Feedback/Splat_" + colorChar);
        StartCoroutine(MakeSplat(startPos, splats, delayS));
    }

    IEnumerator MakeSplat(Vector2 startPos, Sprite[] sprites, float delayS)
    {
        yield return new WaitForSeconds(delayS);
        for (int i = 0; i < Random.Range(15f, 30f); i++)
        {
            GameObject newSplat = new GameObject("Splat");
            newSplat.transform.parent = gameObject.transform;
            newSplat.transform.localPosition = SCREEN_CENTER_WORLD;
            int size = Random.Range(0, sprites.Length);
            newSplat.AddComponent<SpriteRenderer>().sprite = sprites[size];
            SplatPart part = newSplat.AddComponent<SplatPart>();
            part.Initialize(size, Random.insideUnitCircle, (Vector3)startPos);
            splatParts.Add(part);
        }
    }

    public void RemoveSplats()
    {
        for (int i = splatParts.Count-1; i >= 0; i--)
        {
            SplatPart part = splatParts[i];
            splatParts.RemoveAt(i);
            GameObject.Destroy(part.gameObject);
        }
    }


    class SplatPart : MonoBehaviour
    {
        Vector2 startPos;
        int size;
        public bool doneMoving { get; private set; }
        float DistanceToMove;
        const int MAX_SIZE = 15;
        Vector2 direction;

        float timer = 0;
        float maxTime = 0.2f;
        bool initialized = false;

        public void Initialize(int size, Vector2 direction, Vector3 startPos)
        {
            initialized = true;

            this.size = size;
            this.direction = direction;
        
            transform.localPosition = startPos + (Vector3)(direction * Random.Range(0.0f, (float)size * 0.05f));
            startPos = transform.localPosition;

            Rigidbody2D rigid = gameObject.AddComponent<Rigidbody2D>();
            rigid.gravityScale = 0;
            this.DistanceToMove = (float)(size) * 3f * Random.Range(0.8f, 1.2f);
            rigid.AddForce(direction * DistanceToMove);
        }

        void Update()
        {
            if (!initialized) return;
            timer += Time.deltaTime;
            if(timer >= maxTime)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                this.enabled = false;
            }
        }
    }
}
