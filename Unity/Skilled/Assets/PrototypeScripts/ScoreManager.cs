using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance { private set; get; }

    public ScoreMode scoreMode = ScoreMode.Points;
    public enum ScoreMode
    {
        Health,
        Points
    }
    public GameObject[] itemPickups;
    private int players = 2;    //TODO get number of players in game
    public Text[] scoreText;    //TODO based on number of players + find textfields through script
    public int[] score { private set; get; }
    void Start()
    {
        ScoreManager.instance = this;
        score = new int[players]; 
        if(scoreMode == ScoreMode.Health)
        {
            for (int i = 0; i < score.GetLength(0); i++)
            {
                score[i] = 3;
            }
        }
        UpdateScore();

    }
    public void ChangeScore(int playerID, int scoreChange)
    {
        if (playerID >= players)
        {
            Debug.Log("tried to add score for player that doesnt exist");
            return;
        }
        if (scoreMode == ScoreMode.Health)
            scoreChange *= -1;

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
