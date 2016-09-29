using UnityEngine;
using System.Collections;
using TeamUtility.IO;

/// <summary>
/// for testing purposes only
/// if controller binding was already setup in a different scene (main menu) it does nothing,
/// else make new controller binding
/// </summary>
public class DisableOnStart : MonoBehaviour {

	void Awake()
    {
        if (FindObjectOfType<ControllerBind>() == null)
        {
            //gameObject.GetComponent<InputManager>().enabled = true;
            //gameObject.GetComponent<ControllerBind>().enabled = true;
            gameObject.AddComponent<InputManager>();
            gameObject.AddComponent<ControllerBind>();
        }
            
    }
}
