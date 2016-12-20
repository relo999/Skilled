using UnityEngine;
using System.Collections.Generic;

public class SceneLoader{

    public enum Scenes
    {
        StartMenu,
        MainMenu,
        Local,
        Online,
        Options
    }

    private static readonly Dictionary<Scenes, string> SceneMapping = new Dictionary<Scenes, string>() {
        { Scenes.StartMenu, "StartScene"},
        { Scenes.MainMenu, "MainMenu"},
        { Scenes.Local, "LocalMenu"},
        { Scenes.Online, "LocalMenu"},  //TODO
        { Scenes.Options, "LocalMenu"}  //TODO
    };

    public static void LoadScene(Scenes newScene)
    {
        string sceneName;

        if (SceneMapping.TryGetValue(newScene, out sceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            return;
        }
        Debug.LogError("Scene " + newScene.ToString() + " not mapped");
    }

   

}
