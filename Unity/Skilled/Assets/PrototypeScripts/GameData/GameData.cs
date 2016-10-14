using UnityEngine;
using System.Collections;

public class GameData{

    public int Level;
    public float Time = 0;    //in seconds
    public int Players;
    public int[] PowerupsSpawned;
    public int[] PowerupsPicked;
    public int[] PowerupsUsed;
    public bool AfkEnd = false;
	public GameData()
    {
        int powAmount = ScoreManager.instance.itemPickups.Length;
        PowerupsSpawned = new int[powAmount];
        PowerupsPicked = new int[powAmount];
        PowerupsUsed = new int[powAmount];
    }
}
