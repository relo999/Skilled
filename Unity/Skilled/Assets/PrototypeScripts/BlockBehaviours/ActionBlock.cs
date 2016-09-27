using UnityEngine;
using System.Collections;

public abstract class ActionBlock : MonoBehaviour {

    public abstract void Activate(GameObject activator);

    private Bounds _bounds;

    void Start()
    {
        _bounds = GetComponent<SpriteRenderer>().bounds;
    }

    protected bool inBounds(Vector2 position)
    {
        if (_bounds.Contains(position)) return true;
        return false;
    }

    void Update()
    {
        
        BlockUpdate();
    }
    protected virtual void BlockUpdate()
    {
        //
    }
}
