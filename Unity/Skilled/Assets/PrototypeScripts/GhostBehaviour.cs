using UnityEngine;
using System.Collections;

public class GhostBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

            if (hit)
            {
                //Debug.Log(hit.transform.name);
                ActionBlock activation = hit.transform.gameObject.GetComponent<ActionBlock>();
                if (activation) activation.Activate(gameObject);
                //return hit.transform.gameObject;
            }
        }
    }
}
