using UnityEngine;
using System.Collections;

public class PowerupUser : MonoBehaviour {

    PlayerMovement.Controls controls;
    PowerupBase currentPowerup;
	// Use this for initialization
	void Start () {
        controls = GetComponent<PlayerMovement>().controls;
        SetPowerup(new PowerupBase.EmptyPowerup(gameObject));

	}

    public void SetPowerup(PowerupBase pwrup)
    {
        //if(currentPowerup != pwrup)   //if the powerup shouldnt be refreshed in case of timers and cooldowns etc
        currentPowerup = pwrup;
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
