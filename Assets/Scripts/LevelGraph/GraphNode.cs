using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtils.Graph
{
    public class GraphNode
    {
        public List<GraphNode> parentNodes;
        public Vector3 Location;
        public List<GraphNode> childNodes;
        
        public GraphNode(Vector3 vector)
        {
            this.parentNodes = new List<GraphNode>();
            this.childNodes = new List<GraphNode>();
            this.Location = vector;
        }

        public void addParent(GraphNode node)
        {
            parentNodes.Add(node);
        }

        public void addChild(GraphNode node)
        {
            childNodes.Add(node);
        }
        
        public GraphNode getNode()
        {
            return this;
        }

        public List<GraphNode> getParents()
        {
            return this.parentNodes;
        }

        public List<GraphNode> getChildren()
        {
            return this.childNodes;
        }
    }
}
