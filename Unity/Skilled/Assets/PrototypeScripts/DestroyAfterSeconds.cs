using UnityEngine;
using System.Collections;

public class DestroyAfterSeconds : MonoBehaviour {

   // [HideInInspector]
    public float Seconds = 1.5f;
    private float _timer;
	// Use this for initialization
	void Start () {
        _timer = Seconds;
	}
	
	// Update is called once per frame
	void Update () {
        _timer -= Time.deltaTime;
        if (_timer <= 0) GameObject.Destroy(gameObject);
	}
}
