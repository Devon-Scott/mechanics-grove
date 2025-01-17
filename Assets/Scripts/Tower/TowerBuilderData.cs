using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilderData : MonoBehaviour
{
    public GameObject[] Towers;
    public GameObject[] Meshes;
    public int[] Costs;

    private int _numOfTowers;

    public LayerMask unallowedTerrain;

    // Start is called before the first frame update
    void Start()
    {
        _numOfTowers = Towers.GetLength(0);
        Meshes = new GameObject[_numOfTowers];
        Costs = new int[_numOfTowers];
        for (int i = 0; i < _numOfTowers; i++)
        {
            Meshes[i] = new GameObject("TowerMesh " + i);
            Meshes[i].transform.position = new Vector3(-100, -100, -100);
            MeshFilter existingMeshFilter = Towers[i].GetComponent<MeshFilter>();
            MeshFilter newMeshFilter = Meshes[i].AddComponent<MeshFilter>();
            newMeshFilter.sharedMesh = existingMeshFilter.sharedMesh;
            newMeshFilter.transform.localScale = existingMeshFilter.transform.localScale;

            MeshRenderer existingRenderer = Towers[i].GetComponent<MeshRenderer>();
            MeshRenderer newMeshRender = Meshes[i].AddComponent<MeshRenderer>();
            newMeshRender.sharedMaterials = existingRenderer.sharedMaterials;

            Costs[i] = Towers[i].GetComponentInChildren<TowerBehaviour>().Cost;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
