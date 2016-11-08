using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	Camera.main.RenderWithShader(Shader.Find("LinearBurn"),("1"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
