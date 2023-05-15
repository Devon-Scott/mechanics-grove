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
    [SerializeField]
    private GameObject spawnEffect;
    private int _enemiesToSpawn;
    private float _enemyCooldown;

    void Awake()
    {
        // https://docs.unity3d.com/ScriptReference/Object.FindAnyObjectByType.html
        // Assets\Scripts\Management\EnemyManager.cs(19,39): error CS0117: 'Object' does not contain a definition for 'FindAnyObjectByType'
        // DOES IT REALLY NOT HAVE A DEFINITION FOR IT???
        // Unity is shaming me because this method is slow but it won't let me use the faster ones
        
        level.Awake();
        Enemy.level = level;
        _enemiesToSpawn = level.EnemiesToSpawn;
        _enemyCooldown = level.EnemyCooldown;
        _enemiesSpawned = 0;
        _enemiesKilled = 0;
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
        ParticleManager.ParticleManagerInit(enemy.transform.position, spawnEffect);
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        _enemiesKilled++;
    }

    IEnumerator SpawnEnemies()
    {
        while (_enemiesSpawned < _enemiesToSpawn) {
            Vector2 offset = Random.insideUnitCircle * 1.5f;
            Vector3 spawnPoint = new Vector3(level.StartPoint.x + offset.x, level.StartPoint.y, level.StartPoint.z + offset.y);
			Enemy enemy = Instantiate(Enemies[Random.Range(0, Enemies.Length)], spawnPoint, Quaternion.identity);
            enemy.AddObserver(this);
            enemy.AddObserver(_colliderManager);
        
			yield return new WaitForSeconds(_enemyCooldown);
		}
        StopCoroutine(SpawnEnemies());
    }
}