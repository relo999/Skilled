using UnityEngine;
using System.Collections;
using TeamUtility.IO;

public class PowerupUser : MonoBehaviour {

    PlayerMovement.Controls controls;
    PowerupBase currentPowerup;
    PowerupBase lastingPowerup;
    private PlayerID _playerID;
	// Use this for initialization
	void Start () {
        controls = GetComponent<PlayerMovement>().controls;
        SetPowerup(new PowerupBase.EmptyPowerup(gameObject));
        SetLastingPowerup(new PowerupBase.EmptyPowerup(gameObject));
        _playerID = this.gameObject.GetComponent<PlayerMovement>().playerID;
    }


    public void EndPowerup()
    {
        if (currentPowerup != null) currentPowerup.End();
        currentPowerup = new PowerupBase.EmptyPowerup(gameObject);
    }
    public void SetPowerup(PowerupBase pwrup)
    {
        //if(currentPowerup != pwrup)   //if the powerup shouldnt be refreshed in case of timers, cooldowns and sprite updates
        if(currentPowerup != null)
            currentPowerup.End();
        currentPowerup = pwrup;
    }
    public void SetLastingPowerup(PowerupBase pwrup)
    {
        if (lastingPowerup != null)
            lastingPowerup.End();
        lastingPowerup = pwrup;
    }

    public void EndAllPowerups()
    {
        if (currentPowerup != null)
            currentPowerup.End();
        if (lastingPowerup != null)
            lastingPowerup.End();
        currentPowerup = new PowerupBase.EmptyPowerup(gameObject);
        lastingPowerup = new PowerupBase.EmptyPowerup(gameObject);
        BombExplode BE = GetComponentInChildren<BombExplode>();
        if (BE) GameObject.Destroy(BE.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if(currentPowerup != null) currentPowerup.Update(Time.deltaTime);
        if (Input.GetKeyDown(controls == PlayerMovement.Controls.WASD ? KeyCode.Space : KeyCode.L) || InputManager.GetButtonDown("Action", _playerID))
        {
            if (currentPowerup != null) currentPowerup.Use();
        }
	}
}
