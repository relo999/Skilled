using UnityEngine;
using System.Collections;

public class BasicMode{

    public ScoreManager.ScoreMode scoreMode;
    public int maxPoints;
    public int pointsPerKill;
    public float maxTime;

    public BasicMode(ScoreManager.ScoreMode scoreMode = ScoreManager.ScoreMode.Points, int maxPoints = 10, int pointsPerKill = 1, float maxTime = 300)
    {
        this.scoreMode = scoreMode;
        this.maxPoints = maxPoints;
        this.pointsPerKill = pointsPerKill;
        this.maxTime = maxTime;
    }
	
    public virtual void ScoreUpdate()
    {

    }

}
