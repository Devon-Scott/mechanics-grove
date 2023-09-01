using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class Initializer : MonoBehaviour
{

    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _collider;
    [SerializeField] private GameObject _enemy;
    [SerializeField] private GameObject _event;
    [SerializeField] private GameObject _level;
    // Assume Level will be added via Unity Editor
    public Level _levelData;

    public bool InstantiatePlayer;
    public bool InstantiateLevel;

    void MakeObject(MonoBehaviour newObject)
    {
        GameObject.Instantiate(newObject);
        
    }

    void Awake()
    {

        GameObject.Instantiate(_event);
        Assert.IsNotNull(_levelData);
        _levelData.Awake();
        GameObject.Instantiate(_level);
        GameObject.Instantiate(_collider);
        GameObject.Instantiate(_enemy);
        GameObject.Instantiate(_canvas);
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
