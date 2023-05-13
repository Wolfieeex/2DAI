using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AIPlatformPlayer : MonoBehaviour
{
    [System.Serializable]
    class NodeInformation
    {
        public PlatformerNavMeshNode node;
        public NodeInformation parent;
        public float gCost;
        public float hCost;
        public float fCost;

        public NodeInformation(PlatformerNavMeshNode node, NodeInformation parent, float gCost, float hCost)
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

    public enum ConnectionType
    {
        none,
        platform,
        drop,
        jump
    }

    public float A_MaxHorizontalSpeed;
    public float A_MaxJumpSpeed;
    public float A_MaxAcceleration;
    public float A_MaxDecceleration;

    public int m_PathIndex;
    public List<Vector2> m_Path { get; private set; }

    public void Awake()
    {
        m_Path = new List<Vector2>();
        //m_GridNodes = GetCompon.GridNodes;
    }

    public void GeneratePath(PlatformerNavMeshNode start, PlatformerNavMeshNode end)
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
                bool m_NodeFoundOnList = false;

                foreach (PlatformerNavMeshNode Pnode in m_BestNode.node.A_PlatformConnectors)
                {
                    m_GCost = Maths.Magnitude((Vector2)m_BestNode.node.transform.position - (Vector2)Pnode.transform.position);
                    for (int j = 0; j < openList.Count; j++)
                    {
                        if (openList[j].node == Pnode)
                        {
                            m_NodeFoundOnList = true;
                            float m_NewCost = m_BestNode.gCost + m_GCost + Heuristic_Manhattan(openList[j].node, end);
                            if (m_NewCost < openList[j].fCost) openList[j].UpdateNodeInformation(m_BestNode, m_BestNode.gCost + m_GCost, Heuristic_Manhattan(openList[j].node, end));
                        }
                    }

                    if (!m_NodeFoundOnList)
                    {
                        for (int j = 0; j < closedList.Count; j++)
                        {
                            if (closedList[j].node == Pnode)
                            {
                                m_NodeFoundOnList = true;
                                float m_NewCost = m_BestNode.gCost + m_GCost + Heuristic_Manhattan(closedList[j].node, end);
                                if (m_NewCost < closedList[j].fCost) closedList[j].UpdateNodeInformation(m_BestNode, m_BestNode.gCost + m_GCost, Heuristic_Manhattan(closedList[j].node, end));
                            }
                        }
                        m_newOpenInfo = new NodeInformation(Pnode, m_BestNode, m_BestNode.gCost + m_GCost, Heuristic_Manhattan(Pnode, end));
                        openList.Add(m_newOpenInfo);
                    }
                }
                closedList.Add(m_BestNode);
                openList.Remove(m_BestNode);
            }
        }

        /*foreach (NodeInformation Info in pathNodes)
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
        }*/

        m_Path = path;
    }


    protected float Heuristic_Manhattan(PlatformerNavMeshNode start, PlatformerNavMeshNode end)
    {
        Vector2 TargetVelocity = (Vector2)end.transform.position - (Vector2)start.transform.position;
        return Mathf.Abs(TargetVelocity.x) + Mathf.Abs(TargetVelocity.y);
    }

    protected float Heuristic_Euclidean(PlatformerNavMeshNode start, PlatformerNavMeshNode end)
    {
        Vector2 TargetVelocity = (Vector2)end.transform.position - (Vector2)start.transform.position;
        return Maths.Magnitude(TargetVelocity);
    }
}
