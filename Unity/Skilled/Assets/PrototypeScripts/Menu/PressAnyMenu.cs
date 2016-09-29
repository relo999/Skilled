using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using UnityEngine.SceneManagement;

public class PressAnyMenu : MonoBehaviour {

    void Update()
    {
        if(Input.anyKeyDown) SceneManager.LoadScene("TestMenu");
    }

}
