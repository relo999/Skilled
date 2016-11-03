using UnityEngine;
using System.Collections;


/// <summary>
/// information about the inital game state when starting a new game
/// </summary>
public class GameState{

    public GameState() { }

    public string LevelName;
    public bool[] Players;  //0,0,0,1 means only player 4 is connected
    public bool HealthMode;
    public int Score;   //starting lives if healthmode, game end score if scoremode

    
}
