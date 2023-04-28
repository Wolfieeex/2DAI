using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        bool m_pathSeeking = true;
        NodeInformation m_newOpenInfo = new NodeInformation(start, null, 0.0f, Heuristic_Manhattan(start, end));
        
        openList.Add(m_newOpenInfo);

		while (m_pathSeeking)
        {
            if (openList.Count == 0) m_pathSeeking = false;
            else
            {
                float m_BestfCost = openList[0].fCost;
                NodeInformation m_BestNode = openList[0];
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].node == end)
                    {
                        m_pathSeeking = false;
                        pathNodes.Insert(0, openList[i]);
                        bool nextNodeParentExists = true;
                        while (nextNodeParentExists)
                        {
                            if (pathNodes[0].parent != null) pathNodes.Insert(0, pathNodes[0].parent);
                            else nextNodeParentExists = false;
                        }
                        break;
                    }

                    if (openList[i].fCost < m_BestfCost)
                    {
                        m_BestfCost = openList[i].fCost;
                        m_BestNode = openList[i];
                    }
                }

                if (!m_pathSeeking) break;
                
                float m_GCost = 1.0f;
                bool m_NeighbourCondition = false;
                for (int i = 0; i < 8; i++)
                {
                    if (i%2 == 0) m_GCost = 1.0f;
                    else m_GCost = Mathf.Sqrt(2.0f);
                    if (m_AllowDiagonal)
                    {
                        if (m_CutCorners) m_NeighbourCondition = m_BestNode.node.Neighbours[i].m_Walkable;
                        else
                        {
                            int a = i - 1;
                            int z = i + 1;
                            if (a < 0) a = 7;
                            if (z > 7) z = 0;
                            if (i%2 == 0) m_NeighbourCondition = m_BestNode.node.Neighbours[i].m_Walkable;
                            else m_NeighbourCondition = m_BestNode.node.Neighbours[z].m_Walkable && m_BestNode.node.Neighbours[a].m_Walkable && m_BestNode.node.Neighbours[i].m_Walkable;
                        }
                        
                    }
                    else  m_NeighbourCondition = i%2 == 0 && m_BestNode.node.Neighbours[i].m_Walkable;

                    if (m_NeighbourCondition)
                    {
                        bool m_NodeFoundOnList = false;
                        
                        for (int j = 0; j < openList.Count; j++)
                        {
                            if (openList[j].node == m_BestNode.node.Neighbours[i])
                            {
                                m_NodeFoundOnList = true;
                                float m_NewCost =  m_BestNode.gCost + m_GCost + Heuristic_Manhattan(openList[j].node, end);
                                if (m_NewCost < openList[j].fCost) openList[j].UpdateNodeInformation(m_BestNode, m_BestNode.gCost + m_GCost, Heuristic_Manhattan(openList[j].node, end));
                            }
                        }
                        if (!m_NodeFoundOnList)
                        {
                            for (int j = 0; j < closedList.Count; j++)
                            {
                                if (closedList[j].node == m_BestNode.node.Neighbours[i])
                                {
                                    m_NodeFoundOnList = true;
                                    float m_NewCost =  m_BestNode.gCost + m_GCost + Heuristic_Manhattan(closedList[j].node, end);
                                    if (m_NewCost < closedList[j].fCost) closedList[j].UpdateNodeInformation(m_BestNode, m_BestNode.gCost + m_GCost, Heuristic_Manhattan(closedList[j].node, end));
                                }
                            }
                        }
                        if (!m_NodeFoundOnList)
                        {
                            m_newOpenInfo = new NodeInformation(m_BestNode.node.Neighbours[i], m_BestNode, m_BestNode.gCost + m_GCost, Heuristic_Manhattan(m_BestNode.node.Neighbours[i], end));
                            openList.Add(m_newOpenInfo);
                        }
                    }
                }
                closedList.Add(m_BestNode);
                openList.Remove(m_BestNode);
            }
        }

        foreach (NodeInformation Info in pathNodes)
        {
            path.Add((Vector2)Info.node.transform.position);
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
}

