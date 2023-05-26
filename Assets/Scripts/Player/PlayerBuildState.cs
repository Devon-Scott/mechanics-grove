using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using MyUtils.Graph;

public class PlayerBuildState : MonoBehaviour
{
    public GameObject[] Towers;
    public GameObject[] Meshes;
    
    [HideInInspector]
    public bool BuildState;

    [SerializeField]
    private LayerMask unallowedTerrain;

    private CharacterController _controller;
    private StarterAssetsInputs _input;

    private int _towerIndex;
    private int _numOfTowers;

    private GameObject _towerMesh;
    private bool _firstUpdate;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();

        _numOfTowers = Towers.GetLength(0);
        Meshes = new GameObject[_numOfTowers];
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
        }

        _towerIndex = 0;
        _firstUpdate = true;
        BuildState = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (BuildState)
        {
            if (_firstUpdate)
            {
                _towerMesh = Instantiate(Meshes[_towerIndex]);
                _firstUpdate = false;
            }
            _towerMesh.transform.position = transform.position + transform.forward * 3.5f;

            if (_input.attack)
            {
                if (CanBuildTower(_towerMesh.transform.position))
                {
                    Instantiate(Towers[_towerIndex], _towerMesh.transform.position, Quaternion.identity);
                    BuildState = false;
                }
                else 
                {
                    // Play Error Sound
                }
            }
        } 
        else
        {
            Destroy(_towerMesh);
            _firstUpdate = true;
        }
    }

    bool CanBuildTower(Vector3 position)
    {
        return !Physics.CheckSphere(position, 2.5f, unallowedTerrain);
    }
}
