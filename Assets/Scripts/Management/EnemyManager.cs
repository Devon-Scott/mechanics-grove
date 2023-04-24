using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using MyUtils.Graph;

// Class to spawn and monitor enemies for death
public class EnemyManager : MonoBehaviour, IEnemyObserver
{
    public Enemy[] Enemies;
    public LevelGraph Path;
    private int _enemiesSpawned;
    private int _enemiesKilled;
    private ColliderManager _colliderManager;

    void Start()
    {
        // https://docs.unity3d.com/ScriptReference/Object.FindAnyObjectByType.html
        // Assets\Scripts\Management\EnemyManager.cs(19,39): error CS0117: 'Object' does not contain a definition for 'FindAnyObjectByType'
        // DOES IT REALLY NOT HAVE A DEFINITION FOR IT???
        // Unity is shaming me because this method is slow but it won't let me use the faster ones
        _colliderManager = GameObject.FindObjectOfType<ColliderManager>();
        if (Enemy.PlayerList == null)
        {
            Enemy.PlayerList = new List<Collider>();
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in Players)
            {
                Enemy.PlayerList.Add(player.GetComponent<CharacterController>());
            }
        }
        Path = ScriptableObject.CreateInstance<LevelGraph>();
        Enemy.levelGraph = Path;
        _enemiesSpawned = 0;
        _enemiesKilled = 0;
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {

    }

    public void OnEnemySpawn(Enemy enemy)
    {
        _enemiesSpawned++;
        print("Enemies Spawned: " + _enemiesSpawned);
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        _enemiesKilled++;
        print("Enemies Killed: " + _enemiesKilled);
    }

    IEnumerator SpawnEnemies()
    {
        while (_enemiesSpawned < 10) {

			Enemy enemy = Instantiate(Enemies[Random.Range(0, Enemies.Length)], new Vector3(Random.Range(-7, 10), 0, Random.Range(-7, 10)),
				Quaternion.identity);
            enemy.AddObserver(this);
            enemy.AddObserver(_colliderManager);
        
			yield return new WaitForSeconds(Random.Range(3, 10));
		}
        StopCoroutine(SpawnEnemies());
    }
}