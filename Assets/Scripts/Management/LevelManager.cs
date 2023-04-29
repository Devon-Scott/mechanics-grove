using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

public class LevelManager : MonoBehaviour
{
    public Level level;
    public GameObject pathTile;
    public GameObject groundTile;
    public GameObject SpawnPlate;
    public GameObject[] Decorations;
    private GameObject Path;
    private GameObject Ground;

    void Start()
    {
        UnityEngine.Random.InitState(42);
        Path = new GameObject("Path");
        Ground = new GameObject("Ground");
        foreach (Edge edge in level._graph.edges)
        {
            InstantiateTiles(edge);
        }
        for (int x = (int)level.MapCorner.x; x < level.MapCorner.x + level.Width; x += 10)
        {
            for (int z = (int)level.MapCorner.z; z < level.MapCorner.z + level.Width; z += 10)
            {
                InstantiateGround(x, z);
            }
        }
        //GameObject.Instantiate(SpawnPlate, level.PlayerSpawnPoint, Quaternion.identity);
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
        Vector3 location = new Vector3(x, 0, z);
        Quaternion rotation = Quaternion.identity;
        GameObject Tile = GameObject.Instantiate(groundTile, location, rotation, Ground.transform);
        int numOfDecorations = (int)(UnityEngine.Random.value * 30);
        for (int i = 0; i < numOfDecorations; i++){
            float decX = (UnityEngine.Random.value * 10f) - 5f;
            float decZ = (UnityEngine.Random.value * 10f) - 5f;
            int index = (int)Mathf.Floor(UnityEngine.Random.Range(0, Decorations.Length));
            Vector3 position = new Vector3(x + decX, 0, z + decZ);
            GameObject.Instantiate(Decorations[index], position, rotation, Tile.transform).transform.localScale = new Vector3(2, 2, 2);
        }
    }
} 