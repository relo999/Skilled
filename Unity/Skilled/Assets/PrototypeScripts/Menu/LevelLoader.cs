using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LevelLoader{

    const string LEVEL_PATH = "Assets/Resources/Levels/";

    const string CONFIG = "_Config.txt";

    const string LEVEL_MID = "_Tile Layer Mid";
    const string LEVEL_BOT = "_Tile Layer Bottom";
    const string LEVEL_TOP = "_Tile Layer Top";

    const string EXTENSION = ".csv";

    public static void LoadLevel(string name)
    {
        LevelData levelData = new LevelData();
        levelData.Width = 20;
        ReadConfig(levelData, LEVEL_PATH + name);
        levelData.BotLayer = LoadLevelPart(LEVEL_PATH + name + LEVEL_BOT + EXTENSION);  //visual background
        levelData.MidLayer = LoadLevelPart(LEVEL_PATH + name + LEVEL_MID + EXTENSION);  //gameplay
        levelData.TopLayer = LoadLevelPart(LEVEL_PATH + name + LEVEL_TOP + EXTENSION);  //visual foreground
        MakeLevel(levelData);
    }

    private static void ReadConfig(LevelData leveldata, string name)
    {
        string configData = File.ReadAllText(name + CONFIG);
        string[] lines = configData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        foreach(string line in lines)
        {
            string[] splitLine = line.Split(':');
            switch(splitLine[0].ToLower())
            {
                case "c":
                    leveldata.ColorId = int.Parse(splitLine[1]);
                    break;
                case "t":
                    leveldata.ThemeId = int.Parse(splitLine[1]);
                    break;
            }
        }
    }

    private static int[] LoadLevelPart(string path)
    {
        string levelData = File.ReadAllText(path);
        string[] splitData = levelData.Split(new string[] { ",", Environment.NewLine }, StringSplitOptions.None);
        int[] parsedData = new int[splitData.Length];
        for (int i = 0; i < parsedData.Length; i++)
        {
            if ((i == 0 || i == splitData.Length - 1) && string.IsNullOrEmpty(splitData[i])) { parsedData[i] = -1; continue; };
            parsedData[i] = int.Parse(splitData[i]);
        }
        return parsedData;
    }

    private static void MakeLevel(LevelData leveldata)
    {
        leveldata.Parent = new GameObject("Level");
        MakeLayer(leveldata.BotLayer, leveldata, true, SpriteLayer.Layers.Background2);
        MakeLayer(leveldata.MidLayer, leveldata);
        MakeLayer(leveldata.TopLayer, leveldata, true, SpriteLayer.Layers.Foreground1);
    }

    private static void MakeLayer(int[] layerData, LevelData leveldata, bool isVisual = false, SpriteLayer.Layers layer = SpriteLayer.Layers.Block1)
    {
        for(int i = 0; i < layerData.Length; i++)
        {
            int currentTile = layerData[i];
            GameObject newTile = isVisual? MakeVisual(currentTile, layer, leveldata) : MakeTile(currentTile, layer, leveldata);
            if (newTile != null)
            {
                newTile.transform.parent = leveldata.Parent.transform;
                newTile.transform.localPosition = new Vector2(i % leveldata.Width * (LevelData.TILE_SIZE * 0.01f), ((layerData.Length / leveldata.Width) * 0.01f) - (i / leveldata.Width * (LevelData.TILE_SIZE * 0.01f)));

            }
        }
    }

    private static GameObject MakeVisual(int id, SpriteLayer.Layers layer, LevelData leveldata)
    {
        if (id < 0) return null;
        GameObject newVisual = new GameObject("visual " + id);
        newVisual.AddComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Fore&Background/ForeBackgroundObject")[id];
        newVisual.AddComponent<SpriteLayer>().layer = layer;
        if (leveldata.ColorId != 0)
            newVisual.AddComponent<SpriteColor>().palette = (SpriteColor.Palette)(leveldata.ColorId - 1);
        return newVisual;
    }

    private static GameObject MakeTile(int id, SpriteLayer.Layers layer, LevelData leveldata)
    {
        if (id < 0) return null;

        GameObject newTile = null;

        switch (id)
        {
            default:
                break;
            case 1:
                newTile = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Block"));
                Sprite blockSprite = null;
                switch (leveldata.ThemeId)
                {
                    case 0:
                        blockSprite = Resources.LoadAll<Sprite>("Blocks/Castle")[0];
                        break;
                    case 1:
                        blockSprite = Resources.LoadAll<Sprite>("Blocks/Cave")[0];
                        break;
                }

                newTile.GetComponentInChildren<SpriteRenderer>().sprite = blockSprite;
                break;
            case 2:
                newTile = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PassThroughBlock"));
                break;
            case 4:
                newTile = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SwitchBlock"));
                break;

        }
        if (newTile != null)
            newTile.AddComponent<SpriteLayer>().layer = layer;

        return newTile;
    }

}
