using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/LevelGraph")]
public class LevelGraph : ScriptableObject
{
    public Vector3[,] NodeGraph;
    public Vector3 PlayerSpawnPlate;

    public int graphWidth, graphLength;
    // Start is called before the first frame update
    void Awake()
    {
        graphLength = 4;
        graphWidth = 2;
        NodeGraph = new Vector3[graphLength, graphWidth];
        
        NodeGraph[0, 0] = new Vector3(-36, 0, 6);
        NodeGraph[1, 0] = new Vector3(13, 0, 6);
        NodeGraph[1, 1] = new Vector3(-36, 0, -50);
        NodeGraph[2, 0] = new Vector3(-36, 0, 6);
        NodeGraph[3, 0] = new Vector3(-36, 0, 6);

        PlayerSpawnPlate = new Vector3(-5, 0, 25);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
