using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using MyUtils.Graph;

// Class to spawn and monitor enemies for death
public class EnemyManager : MonoBehaviour, IEnemyObserver
{
    public Enemy[] Enemies;
    public Level level;
    private int _enemiesSpawned;
    private int _enemiesKilled;
    private ColliderManager _colliderManager;
    private ParticleSystem spawnEffect;

    void Awake()
    {
        // https://docs.unity3d.com/ScriptReference/Object.FindAnyObjectByType.html
        // Assets\Scripts\Management\EnemyManager.cs(19,39): error CS0117: 'Object' does not contain a definition for 'FindAnyObjectByType'
        // DOES IT REALLY NOT HAVE A DEFINITION FOR IT???
        // Unity is shaming me because this method is slow but it won't let me use the faster ones
        
        level.Awake();
        Enemy.level = level;
        _enemiesSpawned = 0;
        _enemiesKilled = 0;
        spawnEffect = GetComponentInChildren<ParticleSystem>();
    }

    void Start()
    {
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
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {

    }

    public void OnEnemySpawn(Enemy enemy)
    {
        _enemiesSpawned++;
        spawnEffect.transform.position = enemy.transform.position;
        spawnEffect.Play();
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        _enemiesKilled++;
    }

    IEnumerator SpawnEnemies()
    {
        while (_enemiesSpawned < 10) {

			Enemy enemy = Instantiate(Enemies[Random.Range(0, Enemies.Length)], level.StartPoint, Quaternion.identity);
            enemy.AddObserver(this);
            enemy.AddObserver(_colliderManager);
        
			yield return new WaitForSeconds(Random.Range(2, 5));
		}
        StopCoroutine(SpawnEnemies());
    }
}