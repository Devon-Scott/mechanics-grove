using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using UnityEngine;
using MyUtils.Graph;

[CreateAssetMenu(menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    [HideInInspector]
    public Graph _graph;
    public Vector3[] PathPoints;
    [SerializeField]
    [Tooltip("The [i, j] = n entry indicates that the ith point is a child of the jth point if n > 0")]
    Array2DInt ParentChildMatrix;
    [HideInInspector]
    public Vector3 StartPoint;
    public Vector3 EndPoint;
    public Vector3 SpawnPoint;

    [Tooltip("Which element in Path Points will the enemy move towards first?")]
    public int StartPointIndex;
    [SerializeField]
    public Edge[] Edges;
    public Vector3 MapCorner;
    public float Width;
    public float Length;

    // Needs to be called by the Game manager, or the Level manager
    public void Awake()
    {
        Debug.Log("Level Awake");
        Vector2 GridSize = ParentChildMatrix.GridSize;
        _graph = new Graph();

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            { 
                if (ParentChildMatrix.GetCell(x, y) > 0)
                {
                    Edge newEdge = new Edge(PathPoints[x], PathPoints[y]);
                    _graph.addEdge(newEdge);
                }
            }
        }
        int edgeCount = _graph.edges.Count;
        StartPoint = PathPoints[StartPointIndex];
    }
    
}