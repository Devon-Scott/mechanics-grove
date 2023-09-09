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
    public GameObject[] BorderObjects;
    public GameObject[] Obstacles;

    // Parent classes for the various objects that make up the level
    private GameObject Path;
    private GameObject Ground;
    private GameObject Border;
    private GameObject Obstacle;

    public GameObject Canvas;

    public float Width;
    public float Length;
    
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

        foreach (Vector3 point in level.PathPoints)
        {
            minX = (int)Mathf.Min(minX, point.x);
            minZ = (int)Mathf.Min(minZ, point.z);
            maxX = (int)Mathf.Max(maxX, point.x);
            maxZ = (int)Mathf.Max(maxZ, point.z);
        }
        Width = (maxX - minX) + 60;
        Length = (maxZ - minZ) + 60;
    }

    void Start()
    {
        
        // Data  for the size of our map, used for Camera

        //Destroy(GameObject.FindWithTag("MainCamera"));
        UnityEngine.Random.InitState(42);
        Path = new GameObject("Path");
        Ground = new GameObject("Ground");
        Border = new GameObject("Border");
        Obstacle = new GameObject("Obstacle");
        if (InstantiateLevel)
        {
            foreach (Edge edge in level._graph.edges)
            {
                InstantiateTiles(edge);
            }
            for (int x = (int)minX - 30; x <= maxX + 30; x += 20)
            {
                for (int z = (int)minZ - 30; z <= maxZ + 30; z += 20)
                {
                    InstantiateGround(x, z);
                }
            }
            InstantiateBorder();
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

        int pathLayer = LayerMask.NameToLayer("Path");
        LayerMask path = 1 << pathLayer;

        int terrainObstacleLayer = LayerMask.NameToLayer("Terrain_Obstacle");
        for (int i = 0; i < numOfDecorations; i++){
            float decX = (UnityEngine.Random.value * 20f) - 10f;
            float decZ = (UnityEngine.Random.value * 20f) - 10f;
            Vector3 position = new Vector3(x + decX, 0, z + decZ);
            // 15% chance to make a terrain obstacle, if it doesn't spawn on path. Tune to taste
            if (!Physics.CheckSphere(position, 1.5f, path) && (UnityEngine.Random.value * 100) > 85)
            {
                int index = (int)Mathf.Floor(UnityEngine.Random.Range(0, Obstacles.Length));
                GameObject obstacle = GameObject.Instantiate(Obstacles[index], position, rotation, Tile.transform);
                obstacle.layer = terrainObstacleLayer;
            }
            else
            {
                int index = (int)Mathf.Floor(UnityEngine.Random.Range(0, Decorations.Length));
                GameObject.Instantiate(Decorations[index], position, rotation, Tile.transform).transform.localScale = new Vector3(4, 8, 4);
            }

            
        }
    }

    public void InstantiateBorder()
    {
        float mountainWidth = 20;
        int startX = minX - 30;
        int endX = maxX + 30;
        int startZ = minZ - 30;
        int endZ = maxZ + 30;
        Vector3 topLeft = new Vector3(startX, 0, startZ);
        Vector3 topRight = new Vector3(endX, 0, startZ);
        Vector3 bottomLeft = new Vector3(startX, 0, endZ);
        Vector3 bottomRight = new Vector3(endX, 0, endZ);
        MakeMountains(topLeft, topRight, mountainWidth);
        MakeMountains(topRight, bottomRight, mountainWidth);
        MakeMountains(topLeft, bottomLeft, mountainWidth);
        MakeMountains(bottomLeft, bottomRight, mountainWidth);
    }

    void MakeMountains(Vector3 start, Vector3 end, float stepSize)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        int numIterations = Mathf.CeilToInt(distance / stepSize);

        for (int i = 0; i <= numIterations; i++)
        {
            Vector3 position = start + direction * i * stepSize;
            float degrees = UnityEngine.Random.value * 360;
            Quaternion rotation = Quaternion.AngleAxis(degrees, Vector3.up);
            int index = (int)Mathf.Round(UnityEngine.Random.value * (BorderObjects.Length - 1));
            GameObject.Instantiate(BorderObjects[index], position, rotation, Border.transform);
        }
    }
} 