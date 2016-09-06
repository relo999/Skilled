using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance { private set; get; }



    private int players = 2;    //TODO get number of players in game
    public Text[] scoreText;    //TODO based on number of players + find textfields through script
    int[] score;
    void Start()
    {
        ScoreManager.instance = this;
        score = new int[players]; 

    }
    public void ChangeScore(int playerID, int scoreChange)
    {
        if (playerID >= players)
        {
            Debug.LogError("tried to add score for player that doesnt exist");
            return;
        }
        score[playerID] += scoreChange;
        UpdateScore();
    }

    public void UpdateScore()
    {
        for (int i = 0; i < scoreText.GetLength(0); i++)
        {
            scoreText[i].text = score[i].ToString();
        }
    }


}
