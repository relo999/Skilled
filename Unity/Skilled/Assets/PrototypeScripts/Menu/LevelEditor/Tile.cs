using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile{

    public const int LEVEL_WIDTH = 20;
    public const int LEVEL_HEIGHT = 16;
    public const float TILE_SIZE = 0.32f;
    public static Dictionary<int, Sprite> TileMap = new Dictionary<int, Sprite>()
    {
        {1, Resources.Load<Sprite>("Blocks/BasicBlock") },
        {2, Resources.Load<Sprite>("Blocks/PassthroughBlocks") },
        {3, Resources.Load<Sprite>("Blocks/ItemBlockEmpty") },
        {4, Resources.LoadAll<Sprite>("Blocks/Switchblock")[0] },
        {5, Resources.LoadAll<Sprite>("Blocks/SwitchBlockOut")[0] },
        {6, Resources.Load<Sprite>("Items/BounceBlock") }

    };

    public static Sprite[] ForeBackground = Resources.LoadAll<Sprite>("Fore&Background/ForeBackgroundObject");


    public static void UpdateTiles(GameObject[,] tiles, Sprite[] tileSet, Level level = null)
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                if (tiles[x, y] == null || tiles[x,y].tag != "BasicBlock") continue;
                int sprite = 0;
                bool left = false;
                bool right = false;
                bool top = false;
                bool bot = false;
                for (int i = 0; i < tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < tiles.GetLength(1); j++)
                    {
                        if (tiles[i, j] == null) continue;

                        if(tiles[i,j].transform.position.x == tiles[x,y].transform.position.x)
                        {
                            float yDiff = tiles[i, j].transform.position.y - tiles[x, y].transform.position.y;
                            if (yDiff > TILE_SIZE -.1f && yDiff < TILE_SIZE + .1f) top = true;
                            else if (yDiff > -TILE_SIZE -.1f && yDiff < -TILE_SIZE + .1f) bot = true;
                        }
                        else
                        if (tiles[i, j].transform.position.y == tiles[x, y].transform.position.y)
                        {
                            float xDiff = tiles[i, j].transform.position.x - tiles[x, y].transform.position.x;
                            if (xDiff > TILE_SIZE -.1f && xDiff < TILE_SIZE + .1f) right = true;
                            else if (xDiff > -TILE_SIZE -.1f && xDiff < -TILE_SIZE + .1f) left = true;
                        }
                    }
                }



                if (left && right && top && bot)
                {

                    Sprite currentSprite = tiles[x, y].GetComponent<SpriteRenderer>().sprite;
                    if (currentSprite != tileSet[0] && currentSprite != tileSet[8] && currentSprite != tileSet[13])
                    {
                        int random = Random.Range(0, 3);
                        if (random == 0) sprite = 0;
                        if (random == 1) sprite = 8;
                        if (random == 2) sprite = 13;
                    }
                }
                else if (!left && !right && !top && bot) sprite = 1;
                else if (!left && !right && !top && !bot) sprite = 2;             
                else if (!left && !right && top && bot) sprite = 3;
                else if (!left && right && !top && bot) sprite = 4;
                else if (left && right && !top && bot) sprite = 5;
                else if (left && !right && !top && bot) sprite = 6;
                else if (!left && right && !top && !bot) sprite = 7;
                else if (left && right && !top && !bot) sprite = 9;
                else if (left && !right && !top && !bot) sprite = 10;
                else if (!left && !right && top && !bot) sprite = 11;
                else if (!left && right && top && bot) sprite = 12;
                else if (left && !right && top && bot) sprite = 14;
                else if (!left && right && top && !bot) sprite = 15;
                else if (left && right && top && !bot) sprite = 16;
                else if (left && !right && top && !bot) sprite = 17;

                if (level != null) level.gameLayer[x, y].data = sprite;
                tiles[x, y].GetComponent<SpriteRenderer>().sprite = tileSet[sprite];
            }
        }


    }


}
