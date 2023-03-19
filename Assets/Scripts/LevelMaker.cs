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
        //AudioListener listen = camera.GetComponent<AudioListener>;
        Instantiate(SpawnPlate, graph.PlayerSpawnPlate, Quaternion.identity);
        Instantiate(player, graph.PlayerSpawnPlate + 3 * Vector3.up, Quaternion.identity);

        Vector3 start = new Vector3(-36, 0, 6);
        Vector3 end = new Vector3(13, 0, 6);
        makeTilesFromTo(start, end);
        
    }
    
    void makeTilesFromTo(Vector3 from, Vector3 to)
    {
        Vector3 fromTo = to - from;
        Vector3 start = fromTo;
        int X = (int)fromTo.x;
        int Y = (int)fromTo.y;
        int Z = (int)fromTo.z;

        for (int i = (int)from.x; i < (int)to.x; i += 3){
            Instantiate(mapPrefabs[0], fromTo, Quaternion.identity);
            fromTo.x += 3;
        }

        fromTo = start;
        for (int i = (int)from.z; i < (int)to.z; i += 3){
            Instantiate(mapPrefabs[0], fromTo, Quaternion.identity);
            fromTo.z += 3;
        }
        Debug.Log("tiles made");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
