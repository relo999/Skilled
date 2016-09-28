using UnityEngine;
using System.Collections;

public class LoopOutLevel : MonoBehaviour {

    public bool isClone = false;
	void Start()
    {
        if(!isClone)
        //if(!gameObject.name.Contains("Clone"))
            LevelBounds.Instance.RegisterObject(gameObject);
        //Debug.Log(gameObject.name + " reg");
    }
    void OnDestroy()
    {
        if(!isClone)
        //if (!gameObject.name.Contains("Clone"))
            LevelBounds.Instance.UnRegisterObject(gameObject);
        //Debug.Log(gameObject.name + " un reg");
    }
}
