using UnityEngine;
using System.Collections;

public class TilePlacer : MonoBehaviour {

    

    GameObject[,] tiles = new GameObject[Tile.LEVEL_WIDTH, Tile.LEVEL_HEIGHT];
    TileSelector _tileSelector;

    Sprite[] tileSet;

    Level level;

    void Start()
    {
        level = new Level();
        _tileSelector = FindObjectOfType<TileSelector>();
        SetTileSet(0);   
    }

    public void DeleteRenderedTiles()
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if (tiles[i, j] != null)
                    GameObject.Destroy(tiles[i, j]);
            }
        }
    }

    public void RenderCurrentLayer()
    {

        switch (_tileSelector.currentLayer)
        {          
            case 0:
                for (int i = 0; i < level.gameLayer.GetLength(0); i++)
                {
                    for (int j = 0; j < level.gameLayer.GetLength(1); j++)
                    {
                        int currentTile = level.frontLayer[i, j];
                        if (currentTile == -1) continue;
                        _tileSelector.SelectedTile = currentTile;
                        TileObject temp;
                        tiles[i, j] = PlaceTile(i, j, out temp);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < level.gameLayer.GetLength(0); i++)
                {
                    for (int j = 0; j < level.gameLayer.GetLength(1); j++)
                    {
                        TileObject currentObj = level.gameLayer[i, j];
                        if (currentObj.tileId == 0) continue;
                        _tileSelector.SelectedTile = currentObj.tileId;
                        tiles[i,j] = PlaceTile(i, j, out currentObj, currentObj.data);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < level.gameLayer.GetLength(0); i++)
                {
                    for (int j = 0; j < level.gameLayer.GetLength(1); j++)
                    {
                        int currentTile = level.backLayer[i, j];
                        if (currentTile == -1) continue;
                        _tileSelector.SelectedTile = currentTile;
                        TileObject temp;
                        tiles[i, j] = PlaceTile(i, j, out temp);
                    }
                }
                break;

        }


    }

    void SetTileSet(int index)
    {
        string path = "";
        switch (index)
        {
            case 0:
                path = "Castle";
                break;
            case 1:
                path = "Future";
                break;
            case 2:
                path = "Cave";
                break;
            case 3:
                path = "Ground";
                break;
        }
        tileSet = Resources.LoadAll<Sprite>("Blocks/" + path);
        level.tileSet = "Blocks/" + path;
    }



    public void OnMouseClick(Vector2 mousePosition)
    {
        int xPos = Mathf.RoundToInt(mousePosition.x / Tile.TILE_SIZE);
        int yPos = Mathf.RoundToInt(mousePosition.y / Tile.TILE_SIZE);
        if (xPos > 0 && xPos < Tile.LEVEL_WIDTH && yPos > 0 && yPos < Tile.LEVEL_HEIGHT)
        {
            TileObject tileObject;
            if (tiles[xPos, yPos] != null) GameObject.Destroy(tiles[xPos, yPos]);
            tiles[xPos, yPos] = PlaceTile(xPos, yPos, out tileObject);
            if(_tileSelector.currentLayer == 1)
                level.gameLayer[xPos, yPos] = tileObject;
            if (_tileSelector.currentLayer == 0)
                level.frontLayer[xPos, yPos] = tileObject.tileId;
            if (_tileSelector.currentLayer == 2)
                level.backLayer[xPos, yPos] = tileObject.tileId;
            Tile.UpdateTiles(tiles, tileSet);
        }
    }


    GameObject PlaceTile(int x, int y, out TileObject tileObject, int extraData = -1)
    {
        GameObject tile = new GameObject("tile" + _tileSelector.currentLayer);
        Sprite tileSprite = null;
        if (_tileSelector.currentLayer != 1)
        {
            tileSprite = Tile.ForeBackground[_tileSelector.SelectedTile];
        }
        else {

            if (_tileSelector.SelectedTile == 1)
            {
                tile.tag = "BasicBlock";
                if (extraData != -1) tileSprite = tileSet[extraData];
            }
            else Tile.TileMap.TryGetValue(_tileSelector.SelectedTile, out tileSprite);
        }
        tile.AddComponent<SpriteRenderer>().sprite = tileSprite;
        tile.transform.position = new Vector2(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE);
        SpriteLayer.Layers newLayer = SpriteLayer.Layers.Block1;
        if (_tileSelector.currentLayer == 0) newLayer = SpriteLayer.Layers.Foreground1;
        if (_tileSelector.currentLayer == 2) newLayer = SpriteLayer.Layers.Background2;
        tile.AddComponent<SpriteLayer>().layer = newLayer;
        tileObject = new TileObject();
        tileObject.tileId = _tileSelector.SelectedTile;
        return tile;
    }
}
