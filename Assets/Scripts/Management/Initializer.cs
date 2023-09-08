using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class Initializer : MonoBehaviour
{
    // These are all manager classes
    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _collider;
    [SerializeField] private GameObject _enemy;
    [SerializeField] private GameObject _event;
    [SerializeField] private GameObject _level;
    [SerializeField] private GameObject _player;

    // Assume Level will be added via Unity Editor
    public Level _levelData;

    // Camera needs to be added to the scene so Canvas can manipulate it
    [SerializeField] private GameObject _miniMapCamera;

    public bool InstantiatePlayer;
    public bool InstantiateLevel;

    void Awake()
    {
        // Make player add collider manager as observer
        GameObject.Instantiate(_event);
        Assert.IsNotNull(_levelData);
        _levelData.Awake();
        GameObject.Instantiate(_collider);
        GameObject.Instantiate(_level);
        GameObject.Instantiate(_enemy);
        GameObject.Instantiate(_miniMapCamera);
        GameObject.Instantiate(_canvas);
        GameObject.Instantiate(_player);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
