using UnityEngine;
using System.Collections;

public struct LevelData {

    public const int TILE_SIZE = 32;
    public int[] BotLayer;
    public int[] MidLayer;
    public int[] TopLayer;
    public int Width;
    public int ColorId;
    public int ThemeId;
    public GameObject Parent;
}
