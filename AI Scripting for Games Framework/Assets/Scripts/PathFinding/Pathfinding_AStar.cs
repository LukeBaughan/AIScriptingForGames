using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Pathfinding_AStar : PathFinding
{
    [System.Serializable]
    class NodeInformation
    {
        public GridNode node;
        public NodeInformation parent;
        public float gCost;
        public float hCost;
        public float fCost;

        public NodeInformation(GridNode node, NodeInformation parent, float gCost, float hCost)
        {
            this.node = node;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
            fCost = gCost + hCost;
        }

        public void UpdateNodeInformation(NodeInformation parent, float gCost, float hCost)
        {
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
            fCost = gCost + hCost;
        }
    }

    public Pathfinding_AStar(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners) { }

    public override void GeneratePath(GridNode start, GridNode end)
    {
        List<NodeInformation> openList = new List<NodeInformation>();
        List<NodeInformation> closedList = new List<NodeInformation>();
        List<NodeInformation> pathNodes = new List<NodeInformation>();
        List<Vector2> path = new List<Vector2>();

        // My Code
        // g = distance from current node to neighbour node
        // h = distance from neighbour node to end node
        // f = total distance = g + h
        // Use Manhattan for finding g and Euclidean for h

        bool pathFound = false;
        float lowest_fCost;
        // Sets the current node to the first node and adds it to the open list
        NodeInformation currentNodeInfo = new(start, null, 0, Heuristic_Euclidean(start, end));
        openList.Add(currentNodeInfo);

        while (!pathFound)
        {
            // If the current node is the end node, end the loop
            if (currentNodeInfo.node == end)
            {
                pathFound = true;
                break;
            }
            // If there are no nodes in the open list, end the loop
            if (openList.Count() == 0)
                break;
            
            bool neighborNodeFound = false;
            bool touchingCorner = false;

            // Checks if the current node is next to a wall node
            for (int i = 0; i < currentNodeInfo.node.Neighbours.Count(); i += 2)
            {
                if (currentNodeInfo.node.Neighbours[i].m_Walkable == false)
                {
                    GridNode wallNode = currentNodeInfo.node.Neighbours[i];

                    // If the neighbour node is a wall, check the wall's neighbours to see how many walls it is connected to
                    int wallNeighbourCount = 0;
                    for (int j = 0; j < wallNode.Neighbours.Count(); j++)
                    {
                        if (wallNode.Neighbours[j].m_Walkable == false)
                            wallNeighbourCount++;
                    }
                    // If the wall is connected to 3 or less other walls, then it is a corner node
                    if (wallNeighbourCount <= 3)
                        touchingCorner = true;
                }
            }

            int neighbourIncrement;

            if (m_CutCorners)
            {
                // If m_AllowDiagonal = true, set the increment to one so that all of the neighbour nodes are checked
                // If m_AllowDiagonal = false, set the increment to two so that the corner nodes aren't checked
                if (m_AllowDiagonal)
                    neighbourIncrement = 1;
                else
                    neighbourIncrement = 2;
            }
            else
            {
                // If a neighbour node is a corner wall, don't get the next corner neighbour node (to prevent cutting the corner)
                if (m_AllowDiagonal && !touchingCorner)
                    neighbourIncrement = 1;
                else
                    neighbourIncrement = 2;
            }


            // Goes through each neighbour node of the current node (skips the corner neighbours)
            for (int i = 0; i < currentNodeInfo.node.Neighbours.Count(); i = i + neighbourIncrement)
            {
                GridNode neighborNode = currentNodeInfo.node.Neighbours[i];

                // Checks if the node is null (could be out of the level/ a collider)
                if (neighborNode != null)
                {
                    // Checks if the node is already in the open list
                    bool nodeInOpenList = false;
                    foreach (NodeInformation nodeInfo in openList)
                    {
                        if (nodeInfo.node == neighborNode)
                            nodeInOpenList = true;
                    }

                    // Checks if the node is already in the closed list
                    bool nodeInClosedList = false;
                    foreach (NodeInformation nodeInfo in closedList)
                    {
                        if (nodeInfo.node == neighborNode)
                            nodeInClosedList = true;
                    }

                    // Add the node info to the open list if it isnt already in the open list or closed list, and if it is a walkable node
                    if (!nodeInOpenList && !nodeInClosedList && neighborNode.m_Walkable == true)
                    {
                        NodeInformation neighborNodeInfo = new NodeInformation(neighborNode, currentNodeInfo,
                            currentNodeInfo.gCost + Heuristic_Manhattan(currentNodeInfo.node, neighborNode), Heuristic_Euclidean(neighborNode, end));
                        openList.Add(neighborNodeInfo);
                        neighborNodeFound = true;
                    }
                }
            }

            // If the node has no more available neighbor nodes, add it to the closed list
            if (!neighborNodeFound)
            {
                openList.Remove(currentNodeInfo);
                closedList.Add(currentNodeInfo);
            }

            lowest_fCost = float.MaxValue;

            // Goes through every node in the open list to find the node with the lowest f cost (which will become the next current node)
            foreach (NodeInformation nodeInfo in openList)
            {
                if (nodeInfo.fCost < lowest_fCost && !closedList.Contains(nodeInfo))
                {
                    lowest_fCost = nodeInfo.fCost;
                    currentNodeInfo = nodeInfo;
                }
            }
        }

        if (pathFound)
        {
            bool pathNodesFilled = false;

            // Adds all of the path nodes to the path list (currentNodeInfo is the end node to begin with)
            while (!pathNodesFilled)
            {
                pathNodes.Add(currentNodeInfo);

                // Ends the loop when it reaches the start node
                if (currentNodeInfo.node == start)
                    pathNodesFilled = true;
                // If the node is not the start node, set the current node to it's parent
                else
                    currentNodeInfo = currentNodeInfo.parent;
            }

            // Loops through the path nodes list backwards and adds each node's position to the path list
            for (int i = pathNodes.Count - 1; i >= 0; i--)
            {
                path.Add(new Vector2(pathNodes[i].node.transform.position.x, pathNodes[i].node.transform.position.y));
            }
        }

        Grid.ResetGridNodeColours();

        foreach (NodeInformation node in closedList)
        {
            node.node.SetClosedInPathFinding();
        }

        foreach (NodeInformation node in openList)
        {
            node.node.SetOpenInPathFinding();
        }

        foreach (NodeInformation node in pathNodes)
        {
            node.node.SetPathInPathFinding();
        }

        m_Path = path;
    }

    private void PrintNodeInformation(NodeInformation node)
    {
        if (node.parent == null)
        {
            Debug.Log("Position: " + node.node + ", Parent: NONE" + ", g: " + node.gCost + ", h: " + node.hCost + ", f: " + node.fCost);
        }
        else
        {
            Debug.Log("Position: " + node.node + ", Parent: " + node.parent.node + ", g: " + node.gCost + ", h: " + node.hCost + ", f: " + node.fCost);
        }
    }
}
