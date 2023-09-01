using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.Assertions;


/*
    Text fields:
    Health
    Lives
    Money
    Level
    Wave
*/

public class CanvasManager : MonoBehaviour, IPlayerObserver, IEnemyObserver
{
    // Assume this class has been initialized after the Level Manager
    [SerializeField] private LevelManager _levelData;
    [SerializeField] private EventManager _eventManager;
    //[SerializeField] private EnemyManager _enemyManager;

    [SerializeField] private GameObject _miniMapCamera;
    [SerializeField] private GameObject Health;
    [SerializeField] private GameObject Lives;
    [SerializeField] private GameObject Money;
    [SerializeField] private GameObject Level;
    [SerializeField] private GameObject Wave;

    private TMP_Text _healthText;
    private TMP_Text _livesText;
    private TMP_Text _moneyText;
    private TMP_Text _levelText;
    private TMP_Text _waveText;

    private ThirdPersonController _player;
    private PlayerStats _playerStats;

    // Start is called before the first frame update
    void Start()
    {
        // Set up camera to the perfect height and location
        CameraInit();

        // Set up references to relevant game objects
        if (_levelData is null)
        {
            _levelData = FindObjectOfType<LevelManager>();
        }
        if (_eventManager is null)
        {
            _eventManager = FindObjectOfType<EventManager>();
        }

        _player = GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>();
        _player.AddObserver(this);
        _playerStats = _player.GetComponent<PlayerStats>();
        _healthText = Health.GetComponent<TMP_Text>();
        _livesText = Lives.GetComponent<TMP_Text>();
        _moneyText = Money.GetComponent<TMP_Text>();
        _levelText = Level.GetComponent<TMP_Text>();
        _waveText = Wave.GetComponent<TMP_Text>();

        _eventManager.NextWave.Subscribe(updateWave);

        // Update text for each object
        OnPlayerHealth((int)_playerStats.Health);
        OnPlayerMoney(_playerStats.Money);
        OnPlayerLifeLost(_playerStats.Lives);
        _levelText.text = "Level: " + _levelData.levelNum;
        // Wave text is updated by Enemy Manager
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CameraInit()
    {
        if (_levelData)
        {
            Camera camComponent = _miniMapCamera.GetComponent<Camera>();
            Vector3 location = new Vector3((_levelData.minX + _levelData.maxX) / 2, 20, (_levelData.minZ + _levelData.maxZ) / 2);
            _miniMapCamera.transform.position = location;
            camComponent.orthographicSize = Mathf.Max(Mathf.Abs((_levelData.maxX - _levelData.minX) / 2), Mathf.Abs((_levelData.maxZ - _levelData.minZ) / 2));
        }
    }

    public void updateWave(System.Object waveData)
    {
        NewWaveEvent wave = (NewWaveEvent)waveData;
        _waveText.text = "Wave: " + wave.currentWave + "/" + wave.maxWaves;
    }

    public void OnEnemySpawn(Enemy enemy)
    {

    }

    public void OnEnemyDeath(Enemy enemy)
    {

    }

    public void OnEnemyVictory(Enemy enemy)
    {
        // Decrement the lives
    }

    public void OnPlayerSpawn(ThirdPersonController player)
    {

    }

    public void OnPlayerHealth(int health)
    {
        // Modify the health text appropriately
        _healthText.text = "Health: " + health;
    }

    public void OnPlayerMoney(int money)
    {
        // Modify the money text appropriately
        _moneyText.text = "Money: " + money;
    }

    public void OnPlayerDeath(ThirdPersonController player)
    {
        // Create a screen fade effect, respawn the player at the spawnpoint, then fade the screen back in
        // Fade in on spawn?
    }

    public void OnPlayerLifeLost(int lives)
    {
        // Modidy Lives text
        _livesText.text = "Lives: " + lives;
    }

    public void OnPlayerVictory(ThirdPersonController player)
    {

    }
}
