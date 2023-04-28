using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    SpriteRenderer m_SpriteRenderer;
    Grid m_Generator;

    [SerializeField]
    Color m_WalkableColour;
    [SerializeField]
    Color m_NotWalkableColour;

    [SerializeField]
    Color m_ClosedInPathFindingColour;

    [SerializeField]
    Color m_OpenInPathFindingColour;

    [SerializeField]
    Color m_PathInPathFindingColour;

    public bool m_Walkable;
    public GridNode m_Parent;
    public int m_Cost = 1;

    /// <summary>
    /// Neighbouring nodes on the grid starting with up and going clockwise
    /// 0 - up
    /// 1 - up right
    /// 2 - right
    /// 3 - down right
    /// 4 - down
    /// 5 - down left
    /// 6 - left
    /// 7 - up left
    /// null if no neighbours
    /// </summary>
    GridNode[] m_Neighbours;
    public GridNode[] Neighbours { get { return m_Neighbours; } private set { } }

    public void Init(Grid generator, GridNode up, GridNode upRight, GridNode right, GridNode downRight, GridNode down, GridNode downLeft, GridNode left, GridNode upLeft)
    {
        m_Neighbours = new GridNode[8] { up, upRight, right, downRight, down, downLeft, left, upLeft };
        
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Generator = generator;
        UpdateWalkable();
    }

    public void UpdateWalkable()
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];
        Physics2D.BoxCast(transform.position, transform.localScale, transform.rotation.eulerAngles.z, transform.forward, m_Generator.m_ContactFilter, hits);
 
 
        if(hits[0])
        {
            m_Walkable = false;
        }
        else
        {
            m_Walkable = true;
        }

        SetWalkableColour();
    }

    public void SetWalkableColour()
    {
        if (m_Walkable)
        {
            m_SpriteRenderer.color = m_WalkableColour;
        }
        else
        {
            m_SpriteRenderer.color = m_NotWalkableColour;
        }
    }

    public void ShowGrid()
    {
        m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
    }

    public void ShowGrid(bool show)
    {
        m_SpriteRenderer.enabled = show;
    }

    public void SetClosedInPathFinding()
    {
        m_SpriteRenderer.color = m_ClosedInPathFindingColour;
    }

    public void SetOpenInPathFinding()
    {
        m_SpriteRenderer.color = m_OpenInPathFindingColour;
    }

    public void SetPathInPathFinding()
    {
        m_SpriteRenderer.color = m_PathInPathFindingColour;
    }
}
