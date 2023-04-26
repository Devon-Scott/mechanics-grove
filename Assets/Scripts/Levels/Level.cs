using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

[CreateAssetMenu(menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    private Graph _graph;
    public Vector3[] Points;
    public Vector3 StartPoint;
    public Vector3 EndPoint;
    [SerializeField]
    public Edge[] Edges;
    public Vector3 MapCorner;
    public float Width;
    public float Length;

    void Awake()
    {
        _graph = new Graph();
        foreach(Edge edge in Edges)
        {
            _graph.addEdge(edge);
        }
    }
}