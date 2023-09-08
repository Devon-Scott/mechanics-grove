using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.Assertions;
using UnityEngine.UI;


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
    [SerializeField] private GameObject BlackCover;

    private TMP_Text _healthText;
    private TMP_Text _livesText;
    private TMP_Text _moneyText;
    private TMP_Text _levelText;
    private TMP_Text _waveText;
    private Image _cover;
    private Material _coverMaterial;

    private ThirdPersonController _player;
    private PlayerStats _playerStats;

    void Awake()
    {      
        // Set up references to relevant game objects
        if (_levelData is null)
        {
            _levelData = FindObjectOfType<LevelManager>();
        }
        if (_eventManager is null)
        {
            _eventManager = FindObjectOfType<EventManager>();
        }
        _miniMapCamera = GameObject.FindWithTag("MiniMapCam");
  
        _healthText = Health.GetComponent<TMP_Text>();
        _livesText = Lives.GetComponent<TMP_Text>();
        _moneyText = Money.GetComponent<TMP_Text>();
        _levelText = Level.GetComponent<TMP_Text>();
        _waveText = Wave.GetComponent<TMP_Text>();
        _cover = BlackCover.GetComponent<Image>();
        _coverMaterial = _cover.material;

        _eventManager.NextWave.Subscribe(updateWave);
        _eventManager.FirstPlayerSpawn.Subscribe(CanvasInit);
        CameraInit();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Wave text is updated by Enemy Manager
        //FadeIn();
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
            camComponent.orthographic = true;
            float horizontalSize = Mathf.Abs((_levelData.maxX - _levelData.minX) / 2);
            float verticalSize = Mathf.Abs((_levelData.maxZ - _levelData.minZ) / 2);
            camComponent.orthographicSize = Mathf.Max(horizontalSize, verticalSize);
        }
    }

    // Only meant to be called when the level is loaded for the first time
    void CanvasInit(System.Object InitData = null)
    {
        _player = GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>();
        _playerStats = _player.GetComponent<PlayerStats>();
        _player.AddObserver(this);

        // Update text for each object
        OnPlayerHealth((int)_playerStats.Health);
        OnPlayerMoney(_playerStats.Money);
        OnPlayerLifeLost(_playerStats.Lives);
        _levelText.text = "Level: " + _levelData.levelNum;
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
        FadeIn(1f);
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
        FadeOut(1f);
        
    }

    private void RaiseEvent(bool FadIn)
    {

    }

    public void OnPlayerLifeLost(int lives)
    {
        // Modidy Lives text
        _livesText.text = "Lives: " + lives;
    }

    public void OnPlayerVictory(ThirdPersonController player)
    {

    }

    // Fade the screen from black (Alpha 1) to Transparent (Alpha 0)
    public void FadeIn(float time = 1.0f)
    {
        StartCoroutine(Fade(1, 0, time));
    }

    // Fade the screen from Transparent (Alpha 0) to Black (Alpha 1) 
    public void FadeOut(float time = 1.0f)
    {
        StartCoroutine(Fade(0, 1, time));
    }

    IEnumerator Fade(float min, float max, float time)
    {
        Color oldColor = BlackCover.GetComponent<Image>().color;
        float timeElapsed = 0f;
        while (timeElapsed < time)
        {
            oldColor.a = Mathf.Lerp(min, max, timeElapsed / time);
            BlackCover.GetComponent<Image>().color = oldColor;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        oldColor = _cover.color;
        oldColor.a = max;
        BlackCover.GetComponent<Image>().color = oldColor;
        if (max == 1)
        {
            // Use this argument cause it's empty and we don't need anything
            _eventManager.FadeOut.RaiseEvent(new LevelStartEvent());
        }
    }
}
