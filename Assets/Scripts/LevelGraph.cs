using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtils.Graph
{
    [CreateAssetMenu(menuName = "ScriptableObjects/LevelGraph")]
    public class LevelGraph : ScriptableObject
    {

        public GraphNode StartNode;
        public GraphNode EndNode;
        // Start is called before the first frame update
        void Awake()
        {
            GraphNode topLeft = new GraphNode(new Vector3(-9, 0, 10));
            GraphNode topRight = new GraphNode(new Vector3(7, 0, 10));
            GraphNode bottomRight = new GraphNode(new Vector3(7, 0, -4));
            GraphNode bottomLeft = new GraphNode(new Vector3(-9, 0, -4));

            topLeft.addChild(topRight);
            topRight.addChild(bottomRight);
            bottomRight.addChild(bottomLeft);
            bottomLeft.addChild(topLeft);

            StartNode = topLeft;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}