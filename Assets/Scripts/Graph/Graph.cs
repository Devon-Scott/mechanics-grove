using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

namespace MyUtils.Graph
{
    // Just make it a graph
    // Nodes are just Vector3 points
    // Edges are just pairs of Vector3 with start and end
    public class Graph : IEnumerable
    {
        // Used for the purposes of getting every edge
        public class EdgeIterator : IEnumerator
        {
            private Graph _graph;
            private int index;
            private List<Tuple<Vector3, Vector3>> edges;

            public EdgeIterator(Graph g)
            {
                index = 0;
                _graph = g;
                edges = _graph.edges;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public Tuple<Vector3, Vector3> Current
            {
                get 
                {
                    try
                    {
                        return edges[index];
                    } 
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public bool MoveNext()
            {
                index++;
                return (index < edges.Count);
            }

            public void Reset(){ index = 0; }
        }

        public EdgeIterator GetEnumerator()
        {
            return new EdgeIterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }

        internal List< Vector3 > nodes;
        internal List< Tuple<Vector3, Vector3> > edges;

        public Graph()
        {
            nodes = new List<Vector3>();
            edges = new List< Tuple<Vector3, Vector3> >();
        }
        
        public void addNode(Vector3 point)
        {
            if (!nodes.Contains(point))
            {
                nodes.Add(point);
            }
        }

        public void addEdge(Vector3 start, Vector3 end)
        {
            addNode(start);
            addNode(end);
            edges.Add(new Tuple<Vector3, Vector3>(start, end));
        }

        public List<Vector3> getChildren(Vector3 point)
        {
            List<Vector3> children = new List<Vector3>();
            foreach(Tuple<Vector3, Vector3> edge in edges)
            {
                if (edge.Item1 == point)
                {
                    children.Add(edge.Item2);
                }
            }
            return children;
        }

        // This will return a Vector3 representing either a node itself, or the closest point
        // to position between the two closest nodes in the graph
        public Vector3 findClosestPointOnGraph(Vector3 position)
        {
            Vector3 closestParent = this.FindClosestNode(position);
            Vector3 closestChild = this.findClosestChild(closestParent, position);
            Vector3 closestPoint = this.findClosestPointBetween(closestParent, closestChild, position);
            return closestPoint;
        }

        // Basic utility function for finding distance between points
        public static float distanceBetween(Vector3 start, Vector3 end)
        {
            return (end - start).magnitude;
        }

        // Get the closest node to a point in the graph
        public Vector3 FindClosestNode(Vector3 position)
        {
            Vector3 closest = nodes[0];
            float closestDistance = distanceBetween(position, closest);
            foreach(Vector3 node in nodes)
            {
                float distance = distanceBetween(position, node);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = node;
                }
            }
            return closest;
        }

        Vector3 findClosestChild(Vector3 Parent, Vector3 position)
        {
            List<Vector3> children = getChildren(Parent);
            if (children.Count == 0)
            {
                return Parent;
            }
            Vector3 closestChild = children[0];
            float closestDistance = distanceBetween(position, closestChild);
            foreach (Vector3 child in children)
            {
                float distance = distanceBetween(position, child);
                if (distance < closestDistance)
                {
                    closestChild = child;
                    closestDistance = distance;
                }
            }
            return closestChild;
        }

        Vector3 findClosestPointBetween(Vector3 start, Vector3 end, Vector3 position)
        {
            // From my understanding, this creates an "origin" at the start node, and then the projection
            // is the vector from the start to the object onto the vector from the start to the end
            Vector3 point = Vector3.Project(position - start, end - start);

            // If point does not lie on the line between start and end, then presumably there exists
            // a set of nodes closer that should have been found by BFSFindClosestNode and FindClosestChild
            // But we check just in case
            if (pointIsInRange(start, end, start + point))
            {
                return start + point;
            }
            else 
            {
                float startDistance = (position - start).sqrMagnitude;
                float endDistance = (position - end).sqrMagnitude;
                if (startDistance <= endDistance)
                {
                    return start;
                }
                else 
                {
                    return end;
                }
            }
            
        }

        bool pointIsInRange(Vector3 start, Vector3 end, Vector3 point)
        {
            float dx = end.x - start.x;
            float dy = end.y - start.y;
            float dz = end.z - start.z;
            float innerProduct = (point.x - start.x) * dx + (point.y - start.y) * dy + (point.z - start.z) * dz;
            return 0 <= innerProduct && innerProduct <= dx * dx + dy * dy + dz * dz;
        }   
    }
}