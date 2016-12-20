using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LevelLoader{

    const string LEVEL_PATH = "Assets/Resources/Levels/";

    const string LEVEL_MID = "_Tile Layer Mid";
    const string LEVEL_BOT = "_Tile Layer Bottom";
    const string LEVEL_TOP = "_Tile Layer Top";

    const string EXTENSION = ".csv";

    public static void LoadLevel(string name)
    {
        LevelData levelData = new LevelData();
        levelData.Width = 20;
        levelData.BotLayer = LoadLevelPart(LEVEL_PATH + name + LEVEL_BOT + EXTENSION);
        levelData.MidLayer = LoadLevelPart(LEVEL_PATH + name + LEVEL_MID + EXTENSION);
        levelData.TopLayer = LoadLevelPart(LEVEL_PATH + name + LEVEL_TOP + EXTENSION);
        MakeLevel(levelData);
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
        GameObject level = new GameObject("Level");
        for (int i = 0; i < leveldata.MidLayer.Length; i++)
        {
            int currentTile = leveldata.MidLayer[i];
            GameObject newTile = MakeTile(currentTile);
            if (newTile != null)
            {
                newTile.transform.parent = level.transform;
                newTile.transform.localPosition = new Vector2(i % leveldata.Width * (LevelData.TILE_SIZE * 0.01f), i / leveldata.Width * (LevelData.TILE_SIZE * 0.01f));

            }
        }
    }

    private static GameObject MakeTile(int id)
    {
        if (id < 0) return null;
        GameObject newTile = new GameObject("Tile " + id.ToString());
        SpriteRenderer newRenderer = newTile.AddComponent<SpriteRenderer>();
        switch (id)
        {
            default:
                break;
            case 22:
                newRenderer.sprite = Resources.LoadAll<Sprite>("Blocks/Castle")[0];
                break;
            case 0:
                newRenderer.sprite = Resources.Load<Sprite>("Blocks/PassthroughBlocks");
                break;

        }

        return newTile;
    }

}
