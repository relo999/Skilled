using UnityEngine;
using System.Collections;

public class KeepOnSceneChange : MonoBehaviour {

	void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
	

}
