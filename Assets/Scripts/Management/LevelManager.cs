using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

public class LevelManager : MonoBehaviour
{
    public Level level;
    public GameObject prefabTile;
    private GameObject Path;

    void Start()
    {
        Path = new GameObject("Path");
        foreach (Edge edge in level._graph.edges)
        {
            InstantiateTiles(edge);
        }
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
            GameObject tile = GameObject.Instantiate(prefabTile, location, rotation, Path.transform);
            location += direction;
            if (Graph.distanceBetween(location, edge.start) >= maxDistance - 1.5)
            {
                location = edge.start + (direction / 3) * (float)(maxDistance - 1.5) - (Vector3.up / 3);
                GameObject lastTile = GameObject.Instantiate(prefabTile, location, rotation, Path.transform);              
                break;
            }
        }
    }
}