using UnityEngine;
using System.Collections;
using System;

public abstract class PowerupBase{

    protected float _cooldown = 0.5f;   //seconds
    protected float _currentCooldown = 0f;
    protected GameObject owner;

    public PowerupBase(GameObject owner)
    {
        this.owner = owner;
        Start();
    }
	public void Use()
    {
        if (_currentCooldown > 0) return;
        Activate();
    }

    protected virtual void Start() { }
    protected abstract void Activate();
    
    public virtual void End()
    {
        //cleanup sprites etc
    }

    public virtual void Update(float deltaTime)
    {
        _currentCooldown = _currentCooldown > 0 ? _currentCooldown - deltaTime : 0;
    }

    public class EmptyPowerup : PowerupBase
    {
        public EmptyPowerup(GameObject owner) : base(owner) { _cooldown = 0f; }
        protected override void Activate() { }
        
    }
}
