using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {

    public static Queue EnemyQueue = new Queue();
    static float _spawnTime = 3;
    static float _spawnTimer = 0;
    public static List<EnemySpawner> spawners = new List<EnemySpawner>();
    int _ownSpawnersIndex;

    public static int NextSpawner = 0;

	// Use this for initialization
	void Start () {
        spawners.Add(this);
        _ownSpawnersIndex = spawners.FindIndex(x => x == this);
        if(_ownSpawnersIndex == NextSpawner) StartCoroutine(FillQueue());
	}

    IEnumerator FillQueue(float delaySeconds = 0)
    {
        yield return new WaitForSeconds(delaySeconds);
        //fill up enemy queue, done only by 1 instance of EnemySpawner
        if (EnemyQueue.Count == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                EnemyQueue.Enqueue(Random.Range(1, 4));
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (NextSpawner != _ownSpawnersIndex) return;
	    if(_spawnTimer > 0)
        {
            _spawnTimer -= Time.deltaTime;
        }
        else
        {
            _spawnTimer = _spawnTime;
            SpawnEnemy();
        }
	}

    void SpawnEnemy()
    {
        if (EnemyQueue.Count <= 0) return;
        GameObject newEnemy = GameObject.Instantiate(Resources.Load("Prefabs/Enemy" + (int)EnemyQueue.Dequeue())) as GameObject;
        newEnemy.transform.position = this.transform.position;
        NextSpawner = Random.Range(0, spawners.Count);
        if (EnemyQueue.Count <= 0) StartCoroutine(FillQueue(10));
    }
}
