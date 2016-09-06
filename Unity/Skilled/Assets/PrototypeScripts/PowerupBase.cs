using UnityEngine;
using System.Collections;

public abstract class PowerupBase{

    protected float _cooldown = 0.5f;   //seconds
    protected float _currentCooldown = 0f;
    protected GameObject owner;

    public PowerupBase(GameObject owner)
    {
        this.owner = owner;
    }
	public void Use()
    {
        if (_currentCooldown > 0) return;
        Activate();
        Debug.Log("Powerup activated");
    }
    protected abstract void Activate();
  
    public void Update(float deltaTime)
    {
        _currentCooldown = _currentCooldown > 0 ? _currentCooldown - deltaTime : 0;
    }
}
