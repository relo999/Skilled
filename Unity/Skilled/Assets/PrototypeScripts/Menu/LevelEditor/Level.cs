using UnityEngine;
using System.Collections;

public class Level{

    public Level()
    {
        for (int i = 0; i < Tile.LEVEL_WIDTH; i++)
        {
            for (int j = 0; j < Tile.LEVEL_HEIGHT; j++)
            {
                frontLayer[i, j] = -1;
                backLayer[i, j] = -1;
            }
        }
    }
    public int[,] frontLayer = new int[Tile.LEVEL_WIDTH, Tile.LEVEL_HEIGHT];
    public TileObject[,] gameLayer = new TileObject[Tile.LEVEL_WIDTH, Tile.LEVEL_HEIGHT];
    public int[,] backLayer = new int[Tile.LEVEL_WIDTH, Tile.LEVEL_HEIGHT];
    public string tileSet;
    public void SaveToFile(string fileName)
    {
        
    }
    public bool LoadFromFile(string fileName)
    {
        return false;
    }
}

public struct TileObject
{
    public int tileId;
    public int data;
}


