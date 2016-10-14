using UnityEngine;
using System.Collections;

public class AnimationCallbackDestroy : MonoBehaviour {
    public void AnimationDoneCallback(GameObject g)
    {
        Debug.Log(true);
        GameObject.Destroy(g);
    }
}
