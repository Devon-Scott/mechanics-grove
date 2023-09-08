using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyUtils.Graph;

/*
    Level Maker Class, uses the Level Graph data to instantiate a path,
    then creates decorations, ground, and other objects
*/
public class LevelManager : MonoBehaviour
{
    public Level level;
    public EventManager eventManager;

    public Vector3 spawnPoint;

    public GameObject pathTile;
    public GameObject groundTile;
    public GameObject SpawnPlate;
    public GameObject Player;
    public GameObject[] Decorations;
    private GameObject Path;
    private GameObject Ground;

    public GameObject Canvas;

    
    public int minX = 0, minZ = 0, maxX= 0, maxZ = 0;
    public int levelNum;

    public bool InstantiatePlayer;
    public bool InstantiateLevel;

    void Awake()
    {
        // Initializer enforces these two are loaded before Level Manager
        Initializer init = FindObjectOfType<Initializer>();
        eventManager = FindObjectOfType<EventManager>();
        InstantiateLevel = init.InstantiateLevel;
        level = init._levelData;
        //level.Awake();
        levelNum = level.LevelNum;
        spawnPoint = level.PlayerSpawnPoint;
    }

    void Start()
    {
        
        // Data  for the size of our map, used for Camera
        foreach (Vector3 point in level.PathPoints)
        {
            minX = (int)Mathf.Min(minX, point.x);
            minZ = (int)Mathf.Min(minZ, point.z);
            maxX = (int)Mathf.Max(maxX, point.x);
            maxZ = (int)Mathf.Max(maxZ, point.z);
        }
        //Destroy(GameObject.FindWithTag("MainCamera"));
        UnityEngine.Random.InitState(42);
        Path = new GameObject("Path");
        Ground = new GameObject("Ground");
        if (InstantiateLevel)
        {
            foreach (Edge edge in level._graph.edges)
            {
                InstantiateTiles(edge);
            }
            for (int x = (int)level.MapCorner.x; x < level.MapCorner.x + level.Width; x += 20)
            {
                for (int z = (int)level.MapCorner.z; z < level.MapCorner.z + level.Width; z += 20)
                {
                    InstantiateGround(x, z);
                }
            }
            GameObject.Instantiate(SpawnPlate, level.PlayerSpawnPoint, Quaternion.identity);
        }
        if (InstantiatePlayer)
        {
            GameObject.Instantiate(Player, spawnPoint + (2 * Vector3.up), Quaternion.identity);
        }
        // add player observer?

    }

    void InstantiateTiles(Edge edge)
    {
        Vector3 direction = (edge.end - edge.start).normalized * 3;
        Vector3 location = edge.start - (Vector3.up / 3);
        float maxDistance = Graph.distanceBetween(edge.start, edge.end);
        float yRotation = -Mathf.Atan(direction.z / direction.x) * Mathf.Rad2Deg + 90;
        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);
        while (Graph.distanceBetween(location, edge.start) < maxDistance - 1.5)
        {
            GameObject tile = GameObject.Instantiate(pathTile, location, rotation, Path.transform);
            location += direction;
            if (Graph.distanceBetween(location, edge.start) >= maxDistance - 1.5)
            {
                location = edge.start + (direction / 3) * (float)(maxDistance - 1.5) - (Vector3.up / 3);
                GameObject lastTile = GameObject.Instantiate(pathTile, location, rotation, Path.transform);              
                break;
            }
        }
    }

    void InstantiateGround(int x, int z)
    {
        float groundHeight = -0.95f ;
        Vector3 location = new Vector3(x, groundHeight, z);
        Quaternion rotation = Quaternion.identity;
        GameObject Tile = GameObject.Instantiate(groundTile, location, rotation, Ground.transform);
        Tile.transform.localScale = new Vector3(1, 0.5f, 1);
        int numOfDecorations = (int)(UnityEngine.Random.value * 75);
        for (int i = 0; i < numOfDecorations; i++){
            float decX = (UnityEngine.Random.value * 20f) - 10f;
            float decZ = (UnityEngine.Random.value * 20f) - 10f;
            int index = (int)Mathf.Floor(UnityEngine.Random.Range(0, Decorations.Length));
            Vector3 position = new Vector3(x + decX, 0, z + decZ);
            GameObject.Instantiate(Decorations[index], position, rotation, Tile.transform).transform.localScale = new Vector3(2, 8, 2);
        }
    }
} 