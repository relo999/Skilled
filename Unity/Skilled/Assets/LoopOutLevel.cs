using UnityEngine;
using System.Collections;

public class LoopOutLevel : MonoBehaviour {

	void Start()
    {
        LevelBounds.Instance.RegisterObject(gameObject);
    }
    void OnDestroy()
    {
        LevelBounds.Instance.UnRegisterObject(gameObject);
    }
}
