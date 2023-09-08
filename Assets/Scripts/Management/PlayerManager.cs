using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class PlayerManager : MonoBehaviour//, IPlayerObserver
{
    [SerializeField] private EventManager eventManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private CanvasManager canvasManager;
    // Reference loaded in unity editor
    [SerializeField] private GameObject playerCharacter;

    [SerializeField] private GameObject _playerReference;
    [SerializeField] private ThirdPersonController _player;
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private Vector3 spawnPoint;

    // Used on test level to avoid complications
    public bool InstantiatePlayer;
    void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        levelManager = FindObjectOfType<LevelManager>();
        canvasManager = FindObjectOfType<CanvasManager>();

        eventManager.FadeOut.Subscribe(ReSpawnRoutine);
        eventManager.GameOver.Subscribe(GameOver);
        eventManager.Victory.Subscribe(VictoryRoutine);
        spawnPoint = levelManager.spawnPoint + (2 * Vector3.up);

        _playerReference = GameObject.Instantiate(playerCharacter, spawnPoint, Quaternion.identity);
        _player = _playerReference.GetComponentInChildren<ThirdPersonController>();
        Assert.IsNotNull(_player);
        _playerStats = _player.GetComponent<PlayerStats>();
        Assert.IsNotNull(_playerStats);
    }
    // Start is called before the first frame update
    void Start()
    {
        eventManager.FirstPlayerSpawn.RaiseEvent(new LevelStartEvent());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // IMPLEMENT ALL OF THESE
    // public void OnPlayerSpawn(ThirdPersonController player);
    // public void OnPlayerHealth(int health);
    // public void OnPlayerMoney(int money);
    // public void OnPlayerDeath(ThirdPersonController player);
    // public void OnPlayerLifeLost(int lives);
    // public void OnPlayerVictory(ThirdPersonController player);

    public void ReSpawnRoutine(System.Object FadeOutEvent)
    {
        StartCoroutine(WaitTest(0.5f)) ;
    }

    public void GameOver(System.Object eventData)
    {
        SceneManager.LoadSceneAsync("Game Over");
    }

    public void VictoryRoutine(System.Object eventData)
    {
        PlayerPrefs.SetInt("Score", _playerStats.Score);
        SceneManager.LoadSceneAsync("Victory");
    }

    IEnumerator WaitTest(float time)
    {
        float timeElapsed = 0;
        while (timeElapsed < time)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _player._controller.enabled = false;
        _player.transform.position = spawnPoint;
        _player.stateStack.ChangeState(_player.CombatState);
        _player._animator.SetTrigger(_player._animIDRevived);
        _playerStats.Money /= 2;
        _playerStats.Lives -= 1;
        _playerStats.Health = _playerStats.MaxHealth;
        _player._controller.enabled = true;
        _player.OnSpawn();
    }
}
