using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EnemySpawner : MonoBehaviour {

    public static Queue EnemyQueue = new Queue();
    public static Queue EnemyWaves = new Queue();
    static float _spawnTime = 1;
    static float _spawnTimer = 0;
    public static List<EnemySpawner> spawners = new List<EnemySpawner>();
    int _ownSpawnersIndex;
    public static List<GameObject> aliveEnemies = new List<GameObject>();

    static int maxEnemiesInScene = 3;
    private static bool changedSpeed = false;
    public EnemyBase.Speed speed;
    static int currentSpeed = 2;    //0 = slow, 1 = med, 2 = fast
    public static int NextSpawner = 0;

    public static bool ReadFile = false;

	// Use this for initialization
	void Start () {
        spawners.Add(this);
        _ownSpawnersIndex = spawners.FindIndex(x => x == this);
        if(_ownSpawnersIndex == NextSpawner) StartCoroutine(FillQueue());

        if (changedSpeed && currentSpeed != (int)speed) Debug.LogError("ERROR: Spawners have different speeds");
        changedSpeed = true;
        currentSpeed = (int)speed;

        if(!ReadFile) ReadFromFile();
    }

    int GetAliveEnemies()
    {
        for (int i = aliveEnemies.Count-1; i > 0; i--)
        {
            if (!aliveEnemies[i]) aliveEnemies.RemoveAt(i);
        }
        return aliveEnemies.Count;
    }

    void ReadFromFile()
    {
        ReadFile = true;
        string enemyData = File.ReadAllText("EnemyWaves.txt");


        //last splitData is empty because file ends on newline
        string[] splitData = enemyData.Split(new string[] { ",", " ", "-", System.Environment.NewLine }, System.StringSplitOptions.None);
        for (int i = 0; i < splitData.Length-1; i++)
        {
            EnemyWave newWave = new EnemyWave(new int[splitData[i].Length]);
            for (int j = 0; j < splitData[i].Length; j++)
            {
                newWave.enemies[j] = int.Parse(splitData[i][j].ToString());
            }
            EnemyWaves.Enqueue(newWave);
        }
    }

    IEnumerator FillQueue(float delaySeconds = 0)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (EnemyWaves.Count > 0)
        {
            EnemyWave nextWave = (EnemyWave)EnemyWaves.Dequeue();
            //fill up enemy queue, done only by 1 instance of EnemySpawner


            for (int i = 0; i < nextWave.enemies.Length; i++)
            {
                EnemyQueue.Enqueue(nextWave.enemies[i]);
            }

        }
        else
        {
            for (int i = 0; i < Random.Range(3, 10); i++)
            {
                EnemyQueue.Enqueue(Random.Range(1, 6)); //5 different enemies, enemy naming starts at 1
            }

        }
    }

	
	// Update is called once per frame
	void Update () {
        if (Pauzed.IsPauzed) return;
        if (NextSpawner != _ownSpawnersIndex) return;
	    if(_spawnTimer > 0)
        {
            _spawnTimer -= Time.deltaTime;
        }
        else
        {
            
            GameObject newEnemy = SpawnEnemy();
            if (newEnemy)
            {
                aliveEnemies.Add(newEnemy);
                _spawnTimer = _spawnTime;
            }
        }
	}

    GameObject SpawnEnemy()
    {
        if (EnemyQueue.Count <= 0 || GetAliveEnemies() >= maxEnemiesInScene) return null;  //TODO check if current enemy amount does not exeed threshold
        GameObject newEnemy = GameObject.Instantiate(Resources.Load("Prefabs/Enemy" + (int)EnemyQueue.Dequeue())) as GameObject;
        EnemyBase EnemyScript = newEnemy.GetComponent<EnemyBase>();
        EnemyScript.speed = (EnemyBase.Speed)currentSpeed;
        newEnemy.transform.position = this.transform.position;
        NextSpawner = Random.Range(0, spawners.Count);
        if (EnemyQueue.Count <= 0) StartCoroutine(FillQueue(10));
        return newEnemy;
    }

    struct EnemyWave
    {
        public int[] enemies;
        public EnemyWave(int[] enemies) { this.enemies = enemies; }
    }

}
