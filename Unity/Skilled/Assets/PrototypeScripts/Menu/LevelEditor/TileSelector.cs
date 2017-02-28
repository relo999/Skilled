using UnityEngine;
using System.Collections;

public class TileSelector : MonoBehaviour
{


    public int SelectedTile { get; set; }

    GameObject[] _tiles;
    float _tilePositionOffset = 0;
    const float _startTilePositionOffset = 0;
    const float YPOS = 1;
    public int currentLayer = 1;
    private TilePlacer _tilePlacer = null;
    public GameObject layerSelect;

    void Start()
    {
        _tilePlacer = FindObjectOfType<TilePlacer>();
        ChangeLayer(1);
        SelectTile(1);

        UpdateTileRowPositions();
    }

    public void OnChangeLayer()
    {
        currentLayer++;
        currentLayer %= 3;
        ChangeLayer(currentLayer);

    }

    void ChangeLayer(int layer)
    {
        _tilePlacer.DeleteRenderedTiles();
        currentLayer = layer;
        if (currentLayer == 1) LoadTiles();
        else LoadForeBack();
        _tilePlacer.RenderCurrentLayer();

    }


    void DeleteTileSelect()
    {
        if (_tiles == null) return;
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] != null) GameObject.Destroy(_tiles[i]);
        }
    }

    void LoadForeBack()
    {
        DeleteTileSelect();
        _tiles = new GameObject[Tile.ForeBackground.Length];
        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i] = new GameObject("tileSelectObjVisual");
            _tiles[i].AddComponent<SpriteRenderer>().sprite = Tile.ForeBackground[i];
        }
        UpdateTileRowPositions();
    }
    void LoadTiles()
    {
        DeleteTileSelect();
        _tiles = new GameObject[Tile.TileMap.Count];
        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i] = new GameObject("tileSelectObjGame");
            Sprite tileSprite;
            Tile.TileMap.TryGetValue(i, out tileSprite);
            _tiles[i].AddComponent<SpriteRenderer>().sprite = tileSprite;
        }
        UpdateTileRowPositions();
    }

    void UpdateTileRowPositions()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i].transform.position = new Vector2(_startTilePositionOffset + _tilePositionOffset + i * Tile.TILE_SIZE, YPOS);
        }
    }

    void SelectTile(int tile)
    {
        _tiles[SelectedTile].transform.localScale = new Vector2(1, 1);
        SelectedTile = tile;
        _tiles[tile].transform.localScale = new Vector2(1.2f, 1.2f);
    }

    void Update()
    {
        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0))
        {            
            OnMouseClick(worldMousePos);
        }
        if(Input.GetMouseButtonDown(0))
        {
            OnMouseDown(worldMousePos);
        }
    }

    void OnMouseClick(Vector2 mousePosition)
    {
        int xPos = Mathf.RoundToInt(mousePosition.x / Tile.TILE_SIZE - _startTilePositionOffset - _tilePositionOffset);

        if (mousePosition.y < YPOS + Tile.TILE_SIZE / 2f && mousePosition.y > YPOS - Tile.TILE_SIZE / 2f && xPos >= 0)
        {
            SelectTile(xPos);
        }

        else _tilePlacer.OnMouseClick(mousePosition);
        

    }

    void OnMouseDown(Vector2 mousePosition)
    {
        Sprite sprite = layerSelect.GetComponent<SpriteRenderer>().sprite;
        if (mousePosition.x < layerSelect.transform.position.x + sprite.bounds.size.x / 2f && mousePosition.x > layerSelect.transform.position.x - sprite.bounds.size.x / 2f)
        {
            OnChangeLayer();
        }
    }


}
