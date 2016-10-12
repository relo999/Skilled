using UnityEngine;
using System.Collections;

public class SpriteAlphaTest : MonoBehaviour {

    SpriteRenderer SR;
    float currentA = 1;
    public float seconds;
    public float minAlpha;
    Color color;
	// Use this for initialization
	void Start () {
        SR = GetComponent<SpriteRenderer>();
        color = SR.color;
    }
	
	// Update is called once per frame
	void Update () {
        
        currentA = currentA <= minAlpha ? minAlpha : currentA - Time.deltaTime / 3.0f;
        color.a = currentA;
        SR.color = color;
    }
}
