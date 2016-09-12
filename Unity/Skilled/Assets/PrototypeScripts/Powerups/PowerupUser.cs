using UnityEngine;
using System.Collections;

public class PowerupUser : MonoBehaviour {

    PlayerMovement.Controls controls;
    PowerupBase currentPowerup;
    PowerupBase lastingPowerup;
	// Use this for initialization
	void Start () {
        controls = GetComponent<PlayerMovement>().controls;
        SetPowerup(new PowerupBase.EmptyPowerup(gameObject));
        SetLastingPowerup(new PowerupBase.EmptyPowerup(gameObject));
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
        lastingPowerup = pwrup;
    }
	
	// Update is called once per frame
	void Update () {
        if(currentPowerup != null) currentPowerup.Update(Time.deltaTime);
        if (Input.GetKeyDown(controls == PlayerMovement.Controls.WASD ? KeyCode.Space : KeyCode.L))
        {
            if (currentPowerup != null) currentPowerup.Use();
        }
	}
}
