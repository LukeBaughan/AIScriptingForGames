
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;

//[System.Serializable]
//public class Pathfinding_AStar : PathFinding
//{
//    [System.Serializable]
//    class NodeInformation
//    {
//        public GridNode node;
//        public NodeInformation parent;
//        public float gCost;
//        public float hCost;
//        public float fCost;

//        public NodeInformation(GridNode node, NodeInformation parent, float gCost, float hCost)
//        {
//            this.node = node;
//            this.parent = parent;
//            this.gCost = gCost;
//            this.hCost = hCost;
//            fCost = gCost + hCost;
//        }

//        public void UpdateNodeInformation(NodeInformation parent, float gCost, float hCost)
//        {
//            this.parent = parent;
//            this.gCost = gCost;
//            this.hCost = hCost;
//            fCost = gCost + hCost;
//        }
//    }

//    public Pathfinding_AStar(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners) { }

//    public override void GeneratePath(GridNode start, GridNode end)
//    {
//        List<NodeInformation> openList = new List<NodeInformation>();
//        List<NodeInformation> closedList = new List<NodeInformation>();
//        List<NodeInformation> pathNodes = new List<NodeInformation>();
//        List<Vector2> path = new List<Vector2>();

//        // My Code
//        // g = distance from current node to neighbor node
//        // h = distance from neighbor node to end node
//        // f = total distance = g + h
//        // Use Manhattan for finding g and Euclidean for h

//        bool pathFound = false;
//        float lowest_fCost = float.MaxValue;
//        // Sets the current node to the first node and adds it to the open list
//        NodeInformation currentNodeInfo = new(start, null, Heuristic_Manhattan(start, end), Heuristic_Euclidean(start, end));
//        openList.Add(currentNodeInfo);

//        int test = 0;

//        while (!pathFound && test < 100)
//        {
//            test++;

//            pathNodes.Add(currentNodeInfo);

//            // If the node is the end node, then a path has been found
//            if (currentNodeInfo.node == end)
//            {
//                pathFound = true;
//                Debug.Log("PATH FOUND");
//            }
//            // If there are no neighbor nodes in the open list, then the path is not possible
//            if (openList.Count == 0)
//            {
//                Debug.Log("NO PATH POSSIBLE");
//            }

//            // Adds all of the current node's neighbors to the open list (if the node isn't null)
//            foreach (GridNode neighborNode in currentNodeInfo.node.Neighbours)
//            {
//                if (neighborNode != null)
//                {
//                    NodeInformation tempNodeInfo = new(neighborNode, currentNodeInfo, Heuristic_Manhattan(currentNodeInfo.node, neighborNode), Heuristic_Euclidean(neighborNode, end));
//                    // Checks if the next node is not null an is not already on the open list
//                    if (!openList.Contains(tempNodeInfo) && tempNodeInfo.fCost < lowest_fCost)
//                    {
//                        openList.Add(tempNodeInfo);
//                        lowest_fCost = tempNodeInfo.fCost;
//                        currentNodeInfo = tempNodeInfo;
//                        //PrintNodeInformation(openList[openList.Count - 1]);
//                    }
//                }
//            }

//            /*
//             * foreach(NodeInformation nodeInfo in openList)
//            {
//                PrintNodeInformation(nodeInfo);

//                // If the node has a lower f cost as the current lowest f cost, set it as the new lowest f cost
//                // and set the current node to the be the node with the lowest f cost
//                if (nodeInfo.fCost < lowest_fCost)
//                {
//                    lowest_fCost = nodeInfo.fCost;
//                    currentNodeInfo = nodeInfo;
//                }
//            }
//            */

//            //Debug.Log("lowest fCost: " + lowest_fCost);
//            //PrintNodeInformation(currentNodeInfo);

//            Debug.Log("end loop");

//        }

//        Debug.Log("");

//        foreach (NodeInformation node in openList)
//        {
//            //PrintNodeInformation(node);
//        }

//        // My Code End

//        //drawPath
//        Grid.ResetGridNodeColours();

//		foreach (NodeInformation node in closedList)
//		{
//			node.node.SetClosedInPathFinding();
//		}

//		foreach (NodeInformation node in openList)
//		{
//			node.node.SetOpenInPathFinding();
//		}

//		foreach (NodeInformation node in pathNodes)
//		{
//			node.node.SetPathInPathFinding();
//		}

//		m_Path = path;
//    }

//    private void PrintNodeInformation(NodeInformation node)
//    {
//        if (node.parent == null)
//        {
//            Debug.Log("Position: " + node.node + ", Parent: NONE" + ", g: " + node.gCost + ", h: " + node.hCost + ", f: " + node.fCost);
//        }
//        else
//        {
//            Debug.Log("Position: " + node.node + ", Parent: " + node.parent.node + ", g: " + node.gCost + ", h: " + node.hCost + ", f: " + node.fCost);
//        }
//    }

//}