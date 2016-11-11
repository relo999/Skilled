using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class GameData{

    public int Level;
    public float Time = 0;    //in seconds
    public int Players;
    public int[] Scores = new int[4];


    public int BombsSpawned = 0;
    public int BombsPicked = 0;
    public int BombsUsed = 0;

    public int BouncesSpawned = 0;
    public int BouncesPicked = 0;
    public int BouncesUsed = 0;

    public int ShieldsSpawned = 0;
    public int ShieldsPicked = 0;

    public int LivesSpawned = 0;
    public int LivesPicked = 0;

    public int BombSuitSpawned = 0;
    public int BombSuitPicked = 0;
    public int BombSuitUsed = 0;


    public int Jumped = 0;
   
    //public int 


    public bool AfkEnd = false;

    static int GamesPlayed = 0;
	public GameData()
    {
        GamesPlayed++;
    }

    public void WriteToFile()
    {
        string data =
            "Game " + GamesPlayed + Environment.NewLine +
            "Level " + Level + Environment.NewLine +
            "Players " + Players + Environment.NewLine +
            "Time " + (int) Time + Environment.NewLine +
            "Scores " + Scores[0] + "-" + Scores[1] + "-" + Scores[2] + "-" + Scores[3] + Environment.NewLine +

            "BombsSpawned " + BombsSpawned + Environment.NewLine +
            "BombsPicked " + BombsPicked + Environment.NewLine +
            "BombsUsed " + BombsUsed + Environment.NewLine +
            "BouncesSpawned " + BouncesSpawned + Environment.NewLine +
            "BouncesPicked " + BouncesPicked + Environment.NewLine +
            "BouncesUsed " + BouncesUsed + Environment.NewLine +
            "ShieldsSpawned " + ShieldsSpawned + Environment.NewLine +
            "ShieldsPicked " + ShieldsPicked + Environment.NewLine +
            "LivesSpawned " + LivesSpawned + Environment.NewLine +
            "LivesPicked " + LivesPicked + Environment.NewLine +

            "BombSuitSpawned " + BombSuitSpawned + Environment.NewLine +
            "BombSuitPicked " + BombSuitPicked + Environment.NewLine +
            "BombSuitUsed " + BombSuitUsed + Environment.NewLine +


            "Jumps " + Jumped + Environment.NewLine +

            "Afkend " + AfkEnd + Environment.NewLine +

             Environment.NewLine;
            
        //File.AppendAllText(Application.dataPath + "/PlaytestData.txt", data);
        //File.AppendAllText(Application.dataPath + "/PlaytestData.txt", data);
    }
}
