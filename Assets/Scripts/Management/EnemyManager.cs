using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using MyUtils.Graph;
using StarterAssets;

// Class to spawn and monitor enemies for death
public class EnemyManager : MonoBehaviour, IEnemyObserver
{
    [SerializeField] private EventManager _eventManager;
    [SerializeField] private LevelManager _levelManager;

    public GameObject[] Enemies;
    public Level level;
    private int _enemiesSpawned;
    [SerializeField] private int _enemiesKilled;
    private int _enemiesPassed;
    private ColliderManager _colliderManager;
    [HideInInspector]
    public int currentWave;
    [HideInInspector]
    public int waves;
    private int[] _enemiesToSpawn;
    private int[] _typesToSpawn;
    private int _enemyCooldown;
    private int _waveCooldown;
    [SerializeField] private int _maxEnemies;

    public GameObject[] Players;
    //private CanvasManager canvas;

    void Awake()
    {
        currentWave = 1;
        _enemiesSpawned = 0;
        _enemiesKilled = 0;
        _enemiesPassed = 0;
        _maxEnemies = 0;
    }

    void Start()
    {
        if (_levelManager is null)
        {
            _levelManager = FindObjectOfType<LevelManager>();
        }
        if (_eventManager is null)
        {
            _eventManager = FindObjectOfType<EventManager>();
        }
        level = _levelManager.level;
        Enemy.level = level;
        waves = level.Waves;
        _enemiesToSpawn = level.EnemiesToSpawn;
        _typesToSpawn = level.TypesToSpawn;
        _enemyCooldown = level.EnemyCooldown;
        _waveCooldown = level.WaveCooldown;

        for (int i = 0; i < waves; i++)
        {
            _maxEnemies += _enemiesToSpawn[i];
        }

        _colliderManager = GameObject.FindObjectOfType<ColliderManager>();
        _eventManager.FirstPlayerSpawn.Subscribe(GetPlayerReference);
        // if (canvas is null)
        // {
        //     canvas = GameObject.FindAnyObjectByType<CanvasManager>();
        // }
        StartCoroutine(SpawnEnemies());
    }

    public void GetPlayerReference(System.Object eventData)
    {
        Enemy.PlayerList = new List<Collider>();
        Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in Players)
        {
            Enemy.PlayerList.Add(player.GetComponent<CharacterController>());
        }
    }

    void Update()
    {

    }

    public void OnEnemySpawn(Enemy enemy)
    {
        _enemiesSpawned++;
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        _enemiesKilled++;
        if (_enemiesKilled >= _maxEnemies)
        {   
            _eventManager.Victory.RaiseEvent(new LevelStartEvent());
        }
    }

    public void OnEnemyVictory(Enemy enemy)
    {
        _enemiesPassed++;
    }

    IEnumerator SpawnEnemies()
    {
        while (currentWave <= waves)
        {
            NewWaveEvent wave = new NewWaveEvent(currentWave, waves);
            _eventManager.NextWave.RaiseEvent(wave);
            while (_enemiesSpawned < _enemiesToSpawn[currentWave - 1]) {
                Vector2 offset = Random.insideUnitCircle * 1.5f;
                Vector3 spawnPoint = new Vector3(level.StartPoint.x + offset.x, level.StartPoint.y, level.StartPoint.z + offset.y); 
                Vector3 direction = level._graph.findClosestChild(spawnPoint, spawnPoint);
                
                GameObject enemyPrefab = Enemies[0]; // Change this index as needed
                GameObject enemyGameObject = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
                Enemy enemy = enemyGameObject.GetComponent<Enemy>();
                
                enemy.AddObserver(this);
                enemy.AddObserver(_colliderManager);
                //enemy.AddObserver(player);
                foreach(GameObject player in Players)
                {
                    enemy.AddObserver(player.GetComponent<ThirdPersonController>());
                }
            
                yield return new WaitForSeconds(_enemyCooldown);
            }
            currentWave++;
            _enemiesSpawned = 0;
            yield return new WaitForSeconds(_waveCooldown);
        }
        StopCoroutine(SpawnEnemies());
    }
}