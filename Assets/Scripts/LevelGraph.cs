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
            GraphNode topLeft = new GraphNode(new Vector3(-6, 0, 7));
            GraphNode topRight = new GraphNode(new Vector3(4, 0, 7));
            GraphNode bottomRight = new GraphNode(new Vector3(4, 0, -1));
            GraphNode bottomLeft = new GraphNode(new Vector3(-6, 0, -1));

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