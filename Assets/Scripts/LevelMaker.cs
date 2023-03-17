using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaker : MonoBehaviour
{
    public GameObject[] mapPrefabs;
    public GameObject player;
    public GameObject SpawnPlate;
    public LevelGraph graph;

    private Vector3 locationNode;
    // Start is called before the first frame update
    void Start()
    {
        GameObject camera = GameObject.Find("Main Camera");
        //Destroy(camera);
        Instantiate(SpawnPlate, graph.PlayerSpawnPlate, Quaternion.identity);
        Instantiate(player, graph.PlayerSpawnPlate + 3 * Vector3.up, Quaternion.identity);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
