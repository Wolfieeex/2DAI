using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour
{
    [SerializeField] 
    private GameObject m_Map;

    [SerializeField]
    private bool m_ShowGrid = false;

    [SerializeField] 
    private GridNode m_GridNodePrefab; 

    public ContactFilter2D m_ContactFilter;

    static GridNode[] m_GridNodes;

    private float m_GridNodeScale = 1.0f;

    public static GridNode[] GridNodes 
    { 
        get 
        {
            if (m_GridNodes == null) 
                m_GridNodes = new GridNode[0];
            return m_GridNodes; 
        }
        private set { }
    }

    public static Rect m_GridSize { get; private set; }

    int left = int.MaxValue;
    int right = int.MinValue;
    int top = int.MinValue;
    int bottom = int.MaxValue;

    void Awake()
    {
        Physics2D.queriesStartInColliders = true;
        m_GridNodes = new GridNode[0];

        if (m_Map)
        {
            Tilemap[] tileMaps = m_Map.GetComponentsInChildren<Tilemap>();

            for (int i = 0; i < tileMaps.Length; ++i)
            {
                //tileMaps[i].
                tileMaps[i].CompressBounds();
                Bounds tileMapBounds = tileMaps[i].localBounds;

                float curLeft = tileMaps[i].transform.position.x - tileMapBounds.extents.x;
                float curRight = tileMaps[i].transform.position.x + tileMapBounds.extents.x;
                float curTop = tileMaps[i].transform.position.y + tileMapBounds.extents.y;
                float curBottom = tileMaps[i].transform.position.y - tileMapBounds.extents.y;

                if (curLeft < left) left = Mathf.FloorToInt(curLeft);
                if (curRight > right) right = Mathf.CeilToInt(curRight);
                if (curTop > top) top = Mathf.CeilToInt(curTop);
                if (curBottom < bottom) bottom = Mathf.FloorToInt(curBottom);

                Debug.Log(left + ", " + right + ", " + top + ", " + bottom);
            }

            if (left != int.MaxValue)
            {
                m_GridNodeScale = tileMaps[0].cellSize.x;

                float horizontalSize = (right - left) / m_GridNodeScale;
                float verticalSize = (top - bottom) / m_GridNodeScale;
                float gridSize = Mathf.CeilToInt(horizontalSize) * Mathf.CeilToInt(verticalSize);

                m_GridSize = new Rect(left, bottom, right - left, top - bottom);
                m_GridNodes = new GridNode[Mathf.CeilToInt(gridSize)];

                for (int i = 0; i < verticalSize; ++i)
                {
                    for (int j = 0; j < horizontalSize; ++j)
                    {
                        int index = i * Mathf.CeilToInt(horizontalSize) + j;
                        m_GridNodes[index] = Instantiate(m_GridNodePrefab, new Vector3(left + (j * m_GridNodeScale), top - (i * m_GridNodeScale), 0.0f), Quaternion.identity, transform);
                        m_GridNodes[index].gameObject.name = i + " - " + j;
                        m_GridNodes[index].transform.localScale = new Vector3(m_GridNodeScale, m_GridNodeScale, 1);
                    }
                }

                for (int i = 0; i < verticalSize; ++i)
                {
                    for (int j = 0; j < horizontalSize; ++j)
                    {
                        GridNode up         = (i - 1) >= 0 ?                                        m_GridNodes[(i - 1) * Mathf.CeilToInt(horizontalSize) + j]        : null;
                        GridNode upRight    = (i - 1) >= 0 ? (j + 1) < horizontalSize ?             m_GridNodes[(i - 1) * Mathf.CeilToInt(horizontalSize) + (j + 1)]  : null : null;
                        GridNode right      = (j + 1) < horizontalSize ?                            m_GridNodes[i * Mathf.CeilToInt(horizontalSize) + (j + 1)]        : null;
                        GridNode downRight  = (i + 1) < verticalSize ? (j + 1) < horizontalSize ?   m_GridNodes[(i + 1) * Mathf.CeilToInt(horizontalSize) + (j + 1)]    : null : null;
                        GridNode down       = (i + 1) < verticalSize ?                              m_GridNodes[(i + 1) * Mathf.CeilToInt(horizontalSize) + j]        : null;
                        GridNode downLeft   = (i + 1) < verticalSize ? (j - 1) >= 0 ?               m_GridNodes[(i + 1) * Mathf.CeilToInt(horizontalSize) + (j - 1)] : null : null;
                        GridNode left       = (j - 1) >= 0 ?                                        m_GridNodes[i * Mathf.CeilToInt(horizontalSize) + (j - 1)]        : null;
                        GridNode upLeft     = (i - 1) >= 0 ? (j - 1) >= 0 ?                         m_GridNodes[(i - 1) * Mathf.CeilToInt(horizontalSize) + (j - 1)]  : null : null;

                        m_GridNodes[i * Mathf.CeilToInt(horizontalSize) + j].Init(this, up, upRight, right, downRight, down, downLeft, left, upLeft);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Map is value is not set", this);
        }

        ShowGrid(m_ShowGrid);

        Physics2D.queriesStartInColliders = false;
    }

    public static GridNode GetNodeClosestToLocation(Vector2 point)
    {
        if (m_GridNodes != null)
        {
            float shortestDistance = float.MaxValue;
            int index = 0;

            for (int i = 0; i < m_GridNodes.Length; ++i)
            {
                float distance = Maths.Magnitude((Vector2)m_GridNodes[i].transform.position - point);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    index = i;
                }
            }

            return m_GridNodes[index];
        }

        return null;
    }

    public static GridNode GetNodeClosestWalkableToLocation(Vector2 point)
    {
        if (m_GridNodes != null)
        {
            float shortestDistance = float.MaxValue;
            int index = 0;

            for (int i = 0; i < m_GridNodes.Length; ++i)
            {
                if (m_GridNodes[i].m_Walkable)
                {
                    float distance = Maths.Magnitude((Vector2)m_GridNodes[i].transform.position - point);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        index = i;
                    }
                }
            }

            return m_GridNodes[index];
        }

        return null;
    }

    public static GridNode GetRandomWalkableTile()
    {
        float x = Random.Range(m_GridSize.xMin, m_GridSize.xMax);
        float y = Random.Range(m_GridSize.yMin, m_GridSize.yMax);

        return GetNodeClosestWalkableToLocation(new Vector2(x, y));
    }

    [ContextMenu("Toggle Grid")]
    private void ShowGrid()
    {
        for (int i = 0; i < m_GridNodes.Length; ++i)
        {
            m_GridNodes[i].ShowGrid();
        }
    }

    private void ShowGrid(bool show)
    {
        for (int i = 0; i < m_GridNodes.Length; ++i)
        {
            m_GridNodes[i].ShowGrid(show);
        }
    }

    [ContextMenu("Reset Grid Node Colours")]
    public static void ResetGridNodeColours()
    {
        for (int i = 0; i < m_GridNodes.Length; ++i)
        {
            m_GridNodes[i].SetWalkableColour();
        }
    }
}
