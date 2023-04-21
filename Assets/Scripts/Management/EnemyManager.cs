using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

// Class to spawn and monitor enemies for death
public class EnemyManager : MonoBehaviour, IEnemyObserver
{
    public Enemy[] enemies;
    public LevelGraph path;
    private int enemiesSpawned;
    private int enemiesKilled;

    void Start()
    {
        if (Enemy.PlayerList == null)
        {
            Enemy.PlayerList = new List<Collider>();
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in Players)
            {
                Enemy.PlayerList.Add(player.GetComponent<CharacterController>());
            }
        }
        path = ScriptableObject.CreateInstance<LevelGraph>();
        Enemy.levelGraph = path;
        enemiesSpawned = 0;
        enemiesKilled = 0;
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {

    }

    public void OnEnemyDeath(Enemy enemy)
    {
        enemiesKilled++;
        print("Enemies Killed: " + enemiesKilled);
    }

    IEnumerator SpawnEnemies()
    {
        while (enemiesSpawned < 10) {

			// instantiate a random airplane past the right egde of the screen, facing left
			Enemy enemy = Instantiate(enemies[Random.Range(0, enemies.Length)], new Vector3(Random.Range(-7, 10), 0, Random.Range(-7, 10)),
				Quaternion.identity);
            enemy.AddObserver(this);
            print("Enemies Spawned: " + enemiesSpawned++);

			// pause this coroutine for 3-10 seconds and then repeat loop
			yield return new WaitForSeconds(Random.Range(3, 10));
		}
        StopCoroutine(SpawnEnemies());
    }
}