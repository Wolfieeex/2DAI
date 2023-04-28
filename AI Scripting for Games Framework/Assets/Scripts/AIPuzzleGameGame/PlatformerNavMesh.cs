using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformerNavMesh : MonoBehaviour
{
    [SerializeField] private GameObject A_Map;
    [SerializeField] private bool A_ShowGrid = false;
    [SerializeField] private PlatformerNavMeshNode A_NavMeshModePrefab; 
    [SerializeField] private AIPlatformPlayer A_AI;

    public ContactFilter2D m_ContactFilter;
    static PlatformerNavMeshNode[,] A_Nodes;
    private float A_NodeScale = 1.0f;

    public static Rect A_GridSize;
    float left = int.MaxValue;
    float right = int.MinValue;
    float top = int.MinValue;
    float bottom = int.MaxValue;

    public int HorizontalIntervalsChecks = 5;
    public int JumpIntervalsChecks = 5;
    public int ChecksPerSecond = 5;
    public float StandardGravityDecceleration = 9.81f;


    void Awake()
    {
        A_AI = GameObject.FindObjectOfType<AIPlatformPlayer>();

        Physics2D.queriesStartInColliders = true;
        A_Nodes = new PlatformerNavMeshNode[0,0];



        if (A_Map)
        {
            Tilemap[] tileMaps = A_Map.GetComponentsInChildren<Tilemap>();

            for (int i = 0; i < tileMaps.Length; ++i)
            {
                tileMaps[i].CompressBounds();
                Bounds tileMapBounds = tileMaps[i].localBounds;

                float curLeft = tileMapBounds.center.x - tileMapBounds.extents.x;
                float curRight = tileMapBounds.center.x + tileMapBounds.extents.x;
                float curTop = tileMapBounds.center.y + tileMapBounds.extents.y;
                float curBottom = tileMapBounds.center.y - tileMapBounds.extents.y;

                Debug.Log(tileMapBounds.center);

                if (curLeft < left) left = curLeft;
                if (curRight > right) right = curRight;
                if (curTop > top) top = curTop;
                if (curBottom < bottom) bottom = curBottom;
            }

            if (left != int.MaxValue)
            {
                A_NodeScale = tileMaps[0].cellSize.x;

                float horizontalSize = (right - left) / A_NodeScale;
                float verticalSize = (top - bottom) / A_NodeScale;

                A_GridSize = new Rect(left, bottom, right - left, top - bottom);
                A_Nodes = new PlatformerNavMeshNode[Mathf.FloorToInt(verticalSize), Mathf.FloorToInt(horizontalSize)];

                
                

                for (int i = 0; i < Mathf.FloorToInt(verticalSize); ++i)
                {
                    for (int j = 0; j < Mathf.FloorToInt(horizontalSize); ++j)
                    {
                        A_Nodes[i, j] = Instantiate(A_NavMeshModePrefab, new Vector3((left + (j * A_NodeScale)) + (A_NodeScale / 2), (top - (i * A_NodeScale)) - (A_NodeScale / 2), 0.0f), Quaternion.identity, transform);
                        A_Nodes[i, j].gameObject.name = i + " - " + j;
                        A_Nodes[i, j].transform.localScale = new Vector3(A_NodeScale, A_NodeScale, 1);
                    }
                }

                for (int i = 0; i < Mathf.FloorToInt(verticalSize); ++i)
                {
                    for (int j = 0; j < Mathf.FloorToInt(horizontalSize); ++j)
                    {
                        PlatformerNavMeshNode up         = (i - 1) >= 0 ?                                                                            A_Nodes[i - 1, j]        : null;
                        PlatformerNavMeshNode upRight    = (i - 1) >= 0 ? (j + 1) < Mathf.FloorToInt(horizontalSize) ?                               A_Nodes[i - 1, j + 1]  : null : null;
                        PlatformerNavMeshNode right      = (j + 1) < Mathf.FloorToInt(horizontalSize) ?                                              A_Nodes[i, j + 1]        : null;
                        PlatformerNavMeshNode downRight  = (i + 1) < Mathf.FloorToInt(verticalSize) ? (j + 1) < Mathf.FloorToInt(horizontalSize) ?   A_Nodes[i + 1, j + 1]    : null : null;
                        PlatformerNavMeshNode down       = (i + 1) < Mathf.FloorToInt(verticalSize) ?                                                A_Nodes[i + 1, j]        : null;
                        PlatformerNavMeshNode downLeft   = (i + 1) < Mathf.FloorToInt(verticalSize) ? (j - 1) >= 0 ?                                 A_Nodes[i + 1, j - 1] : null : null;
                        PlatformerNavMeshNode left       = (j - 1) >= 0 ?                                                                            A_Nodes[i, j - 1]        : null;
                        PlatformerNavMeshNode upLeft     = (i - 1) >= 0 ? (j - 1) >= 0 ?                                                             A_Nodes[i - 1, j - 1]  : null : null;

                        A_Nodes[i, j].Init(this, up, upRight, right, downRight, down, downLeft, left, upLeft);
                    }
                }

                int curPlatfromID = 0;
                bool[] PlatformOccupancy = new bool[8];
                for (int i = 0; i < Mathf.FloorToInt(verticalSize); ++i)
                {
                    for (int j = 0; j < Mathf.FloorToInt(horizontalSize); ++j)
                    {
                        if (!A_Nodes[i, j].A_IsTerrain)
                        {
                            for (int k = 0; k < 8; k++)
                            {
                                PlatformOccupancy[k] = false;
                                if (A_Nodes[i, j].A_Neighbours[k] == null) PlatformOccupancy[k] = true;
                                else if (A_Nodes[i, j].A_Neighbours[k].A_IsTerrain) PlatformOccupancy[k] = true;
                            }

                            if (PlatformOccupancy[4])
                            {
                                if (A_Nodes[i, j].A_Neighbours[4] == null) {}
                                else if ((!PlatformOccupancy[6] && PlatformOccupancy[2] && PlatformOccupancy[5]) || (!PlatformOccupancy[6] && PlatformOccupancy[5] && !PlatformOccupancy[2] && !PlatformOccupancy[3]))
                                {
                                    A_Nodes[i, j].A_TileType = TileAssigner.RightEdge;
                                    A_Nodes[i, j].A_PlatformID = curPlatfromID;
                                    curPlatfromID++;
                                }
                                else if (!PlatformOccupancy[2] && !PlatformOccupancy[6] && PlatformOccupancy[5] && PlatformOccupancy[3])
                                {
                                    A_Nodes[i, j].A_TileType = TileAssigner.Middle;
                                    A_Nodes[i, j].A_PlatformID = curPlatfromID;
                                }
                                else if ((!PlatformOccupancy[2] && PlatformOccupancy[3] && PlatformOccupancy[6]) || (PlatformOccupancy[3] && !PlatformOccupancy[6] && !PlatformOccupancy[5] && !PlatformOccupancy[2]))
                                {
                                    A_Nodes[i, j].A_TileType = TileAssigner.LeftEdge;
                                    A_Nodes[i, j].A_PlatformID = curPlatfromID;
                                }
                                else if ((PlatformOccupancy[2] && !PlatformOccupancy[6] && !PlatformOccupancy[5]) || (PlatformOccupancy[6] && !PlatformOccupancy[3] && !PlatformOccupancy[2]) ||(PlatformOccupancy[6] && PlatformOccupancy[2]) || (!PlatformOccupancy[2] && !PlatformOccupancy[3] && !PlatformOccupancy[6] && !PlatformOccupancy[5]))
                                {
                                    A_Nodes[i, j].A_TileType = TileAssigner.Single;
                                    A_Nodes[i, j].A_PlatformID = curPlatfromID;
                                    curPlatfromID++;
                                }
                                else A_Nodes[i, j].A_TileType = TileAssigner.Invalid;
                            }
                            else A_Nodes[i, j].A_TileType = TileAssigner.Invalid;
                        }
                        else A_Nodes[i, j].A_TileType = TileAssigner.Invalid;
                        A_Nodes[i, j].VisualiseTileType();
                    }
                }

                for (int i = 0; i < Mathf.FloorToInt(verticalSize); ++i)
                {
                    for (int j = 0; j < Mathf.FloorToInt(horizontalSize); ++j)
                    {
                        if (A_Nodes[i, j].A_TileType == TileAssigner.Middle)
                        {
                            A_Nodes[i, j].A_PlatformConnectors.Add(A_Nodes[i, j].A_Neighbours[2]);
                            A_Nodes[i, j].A_PlatformConnectors.Add(A_Nodes[i, j].A_Neighbours[6]);
                            Debug.DrawLine(A_Nodes[i, j].transform.position, A_Nodes[i, j].A_PlatformConnectors[0].transform.position, Color.white, 100.0f);
                            Debug.DrawLine(A_Nodes[i, j].transform.position, A_Nodes[i, j].A_PlatformConnectors[1].transform.position, Color.white, 100.0f);
                        }
                        else if (A_Nodes[i, j].A_TileType == TileAssigner.LeftEdge)
                        {
                            A_Nodes[i, j].A_PlatformConnectors.Add(A_Nodes[i, j].A_Neighbours[2]);
                            Debug.DrawLine(A_Nodes[i, j].transform.position, A_Nodes[i, j].A_PlatformConnectors[0].transform.position, Color.white, 100.0f);
                            
                            bool checkLower = true;
                            PlatformerNavMeshNode tempLowerNode = A_Nodes[i, j];
                            if (A_Nodes[i, j].A_Neighbours[6] != null)
                            {
                                if (A_Nodes[i, j].A_Neighbours[6].A_IsTerrain) checkLower = false;
                                else tempLowerNode = A_Nodes[i, j].A_Neighbours[6];
                            }  
                            while (checkLower)
                            {
                                if (tempLowerNode.A_Neighbours[4] == null) checkLower = false;
                                else
                                {
                                    if (!tempLowerNode.A_Neighbours[4].A_IsTerrain) tempLowerNode = tempLowerNode.A_Neighbours[4];
                                    else
                                    {
                                        checkLower = false;
                                        A_Nodes[i, j].A_DropConnectors.Add(tempLowerNode);
                                        Debug.DrawLine(A_Nodes[i, j].transform.position, A_Nodes[i, j].A_DropConnectors[0].transform.position, Color.green, 100.0f);
                                    }
                                }
                            }
                        }
                        else if (A_Nodes[i, j].A_TileType == TileAssigner.RightEdge)
                        {
                            A_Nodes[i, j].A_PlatformConnectors.Add(A_Nodes[i, j].A_Neighbours[6]);
                            Debug.DrawLine(A_Nodes[i, j].transform.position, A_Nodes[i, j].A_PlatformConnectors[0].transform.position, Color.white, 100.0f);
                            
                            bool checkLower = true;
                            PlatformerNavMeshNode tempLowerNode = A_Nodes[i, j];
                            if (A_Nodes[i, j].A_Neighbours[2] != null)
                            {
                                if (A_Nodes[i, j].A_Neighbours[2].A_IsTerrain) checkLower = false;
                                else tempLowerNode = A_Nodes[i, j].A_Neighbours[2];
                            }  
                            
                            while (checkLower)
                            {
                                if (tempLowerNode.A_Neighbours[4] == null) checkLower = false;
                                else
                                {
                                    if (!tempLowerNode.A_Neighbours[4].A_IsTerrain) tempLowerNode = tempLowerNode.A_Neighbours[4];
                                    else
                                    {
                                        checkLower = false;
                                        A_Nodes[i, j].A_DropConnectors.Add(tempLowerNode);
                                        Debug.DrawLine(A_Nodes[i, j].transform.position, A_Nodes[i, j].A_DropConnectors[0].transform.position, Color.green, 100.0f);
                                    }
                                }
                            }
                        }
                        else if (A_Nodes[i, j].A_TileType == TileAssigner.Single)
                        {
                            bool checkLower = true;
                            PlatformerNavMeshNode tempLowerNode = A_Nodes[i, j];


                            if (A_Nodes[i, j].A_Neighbours[6] != null)
                            {
                                if (A_Nodes[i, j].A_Neighbours[6].A_IsTerrain) checkLower = false;
                                else tempLowerNode = A_Nodes[i, j].A_Neighbours[6];
                            }  
                            while (checkLower)
                            {
                                if (tempLowerNode.A_Neighbours[4] == null) checkLower = false;
                                else
                                {
                                    if (!tempLowerNode.A_Neighbours[4].A_IsTerrain) tempLowerNode = tempLowerNode.A_Neighbours[4];
                                    else
                                    {
                                        checkLower = false;
                                        A_Nodes[i, j].A_DropConnectors.Add(tempLowerNode);
                                    }
                                }
                            }


                            checkLower = true;
                            if (A_Nodes[i, j].A_Neighbours[2] != null)
                            {
                                if (A_Nodes[i, j].A_Neighbours[2].A_IsTerrain) checkLower = false;
                                else tempLowerNode = A_Nodes[i, j].A_Neighbours[2];
                            }  
                            while (checkLower)
                            {
                                if (tempLowerNode.A_Neighbours[4] == null) checkLower = false;
                                else
                                {
                                    if (!tempLowerNode.A_Neighbours[4].A_IsTerrain) tempLowerNode = tempLowerNode.A_Neighbours[4];
                                    else
                                    {
                                        checkLower = false;
                                        A_Nodes[i, j].A_DropConnectors.Add(tempLowerNode);
                                    }
                                }
                            }
                            foreach (PlatformerNavMeshNode Node in A_Nodes[i, j].A_DropConnectors)
                            {
                                Debug.DrawLine(A_Nodes[i, j].transform.position, Node.transform.position, Color.green, 100.0f);
                            }
                        }
                    }
                }


                float jumpSpeed = A_AI.A_MaxJumpSpeed;
                float horizontalSpeed = A_AI.A_MaxHorizontalSpeed;
                
                for (int i = 0; i < Mathf.FloorToInt(verticalSize); ++i)
                {
                    for (int j = 0; j < Mathf.FloorToInt(horizontalSize); ++j)
                    {
                        if (A_Nodes[i, j].A_TileType != TileAssigner.Invalid)
                        {
                            for (int a = 1; a <= HorizontalIntervalsChecks; a++)
                            {
                                for (int b = 1; b <= JumpIntervalsChecks; b++)
                                {
                                    float IntervaledHorizontalSpeed = Mathf.Lerp(0.0f, horizontalSpeed, (float)a / (float)HorizontalIntervalsChecks);
                                    float IntervaledJumpSpeed = Mathf.Lerp(0.0f, jumpSpeed, (float)b / (float)JumpIntervalsChecks);

                                    List<Vector2> FallingPointsToDraw = new List<Vector2>();
                                    FallingPointsToDraw.Add(A_Nodes[i, j].transform.position);

                                    bool Falling = true;

                                    float Time = 0.0f;
                                    float TimedVerticalDisplacement = 0.0f;
                                    float TimedHorizontalDisplacement = 0.0f;

                                    Vector2 AI_CollisionSize = A_AI.gameObject.GetComponent<BoxCollider2D>().size;
                                    AI_CollisionSize = new Vector2(AI_CollisionSize.x + 0.3f, AI_CollisionSize.y + 0.3f);

                                    while (Falling)
                                    {
                                        Time += 1.0f / (float)ChecksPerSecond;
                                        TimedVerticalDisplacement = (IntervaledJumpSpeed * Time) - (StandardGravityDecceleration * Time * Time / 2);
                                        TimedHorizontalDisplacement = IntervaledHorizontalSpeed * Time;
                                        Vector2 CurrentGraphPoint = new Vector2(A_Nodes[i, j].transform.position.x + TimedHorizontalDisplacement, A_Nodes[i, j].transform.position.y + TimedVerticalDisplacement);

                                        if (CurrentGraphPoint.x > right || CurrentGraphPoint.x < left || CurrentGraphPoint.y < bottom || CurrentGraphPoint.y > top) break;

                                        FallingPointsToDraw.Add(CurrentGraphPoint);

                                        RaycastHit2D[] tilesInRange = Physics2D.BoxCastAll(CurrentGraphPoint, AI_CollisionSize, transform.rotation.eulerAngles.z, transform.forward);
                                        foreach (RaycastHit2D tiles in tilesInRange)
                                        {
                                            if (tiles.transform.gameObject.GetComponent<PlatformerNavMeshNode>() != null)
                                            {
                                                PlatformerNavMeshNode Node = tiles.transform.gameObject.GetComponent<PlatformerNavMeshNode>();
                                                if (Node.A_IsTerrain)
                                                {
                                                    Falling = false;
                                                    break;
                                                }
                                            }
                                        }
                                        if (IntervaledHorizontalSpeed < Time * StandardGravityDecceleration)
                                        {
                                            RaycastHit2D[] validTiles = Physics2D.BoxCastAll(CurrentGraphPoint, new Vector2(0.005f, 0.005f), transform.rotation.eulerAngles.z, transform.forward);
                                            {
                                                foreach (RaycastHit2D validTile in validTiles)
                                                {
                                                    if (validTile.transform.gameObject.GetComponent<PlatformerNavMeshNode>() != null)
                                                    {
                                                        PlatformerNavMeshNode ValidNode = validTile.transform.gameObject.GetComponent<PlatformerNavMeshNode>();
                                                        if (ValidNode.A_TileType != TileAssigner.Invalid)
                                                        {
                                                            if (Maths.Magnitude((Vector2)ValidNode.transform.position - CurrentGraphPoint) < 0.2f)
                                                            {
                                                                Falling = false;
                                                                if (ValidNode.A_PlatformID == A_Nodes[i, j].A_PlatformID) break;

                                                                bool NodeAlreadyConnected = false;
                                                                foreach (PlatformerNavMeshNode SearchedNode in A_Nodes[i, j].A_JumpConnectors)
                                                                {
                                                                    if (SearchedNode == ValidNode)
                                                                    {
                                                                        NodeAlreadyConnected = true;
                                                                        break;
                                                                    }
                                                                }

                                                                if (!NodeAlreadyConnected)
                                                                {
                                                                    A_Nodes[i, j].A_JumpConnectors.Add(ValidNode);
                                                                    JumpCoordinates jumpDetails = new JumpCoordinates(TimedVerticalDisplacement, Time);
                                                                    A_Nodes[i, j].A_JumpDetails.Add(jumpDetails);
                                                                    if (FallingPointsToDraw.Count > 1)
                                                                    {
                                                                        for (int z = 1; z < FallingPointsToDraw.Count; z++)
                                                                        {
                                                                            Debug.DrawLine(new Vector3(FallingPointsToDraw[z - 1].x, FallingPointsToDraw[z - 1].y, A_Nodes[i, j].transform.position.z), new Vector3(FallingPointsToDraw[z].x, FallingPointsToDraw[z].y, A_Nodes[i, j].transform.position.z), Color.yellow, 100.0f);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    FallingPointsToDraw.Clear();
                                    FallingPointsToDraw.Add(A_Nodes[i, j].transform.position);

                                    Falling = true;

                                    Time = 0.0f;
                                    TimedVerticalDisplacement = 0.0f;
                                    TimedHorizontalDisplacement = 0.0f;

                                    while (Falling)
                                    {
                                        Time += 1.0f / (float)ChecksPerSecond;
                                        TimedVerticalDisplacement = (IntervaledJumpSpeed * Time) - (StandardGravityDecceleration * Time * Time / 2);
                                        TimedHorizontalDisplacement = IntervaledHorizontalSpeed * Time;
                                        Vector2 CurrentGraphPoint = new Vector2(A_Nodes[i, j].transform.position.x - TimedHorizontalDisplacement, A_Nodes[i, j].transform.position.y + TimedVerticalDisplacement);

                                        if (CurrentGraphPoint.x > right || CurrentGraphPoint.x < left || CurrentGraphPoint.y < bottom || CurrentGraphPoint.y > top) break;

                                        FallingPointsToDraw.Add(CurrentGraphPoint);

                                        RaycastHit2D[] tilesInRange = Physics2D.BoxCastAll(CurrentGraphPoint, AI_CollisionSize, transform.rotation.eulerAngles.z, transform.forward);
                                        foreach (RaycastHit2D tiles in tilesInRange)
                                        {
                                            if (tiles.transform.gameObject.GetComponent<PlatformerNavMeshNode>() != null)
                                            {
                                                PlatformerNavMeshNode Node = tiles.transform.gameObject.GetComponent<PlatformerNavMeshNode>();
                                                if (Node.A_IsTerrain)
                                                {
                                                    Falling = false;
                                                    break;
                                                }
                                            }
                                        }
                                        if (IntervaledHorizontalSpeed < Time * StandardGravityDecceleration)
                                        {
                                            RaycastHit2D[] validTiles = Physics2D.BoxCastAll(CurrentGraphPoint, new Vector2(0.005f, 0.005f), transform.rotation.eulerAngles.z, transform.forward);
                                            {
                                                foreach (RaycastHit2D validTile in validTiles)
                                                {
                                                    if (validTile.transform.gameObject.GetComponent<PlatformerNavMeshNode>() != null)
                                                    {
                                                        PlatformerNavMeshNode ValidNode = validTile.transform.gameObject.GetComponent<PlatformerNavMeshNode>();
                                                        if (ValidNode.A_TileType != TileAssigner.Invalid)
                                                        {
                                                            if (Maths.Magnitude((Vector2)ValidNode.transform.position - CurrentGraphPoint) < 0.2f)
                                                            {
                                                                Falling = false;
                                                                if (ValidNode.A_PlatformID == A_Nodes[i, j].A_PlatformID) break;

                                                                bool NodeAlreadyConnected = false;
                                                                foreach (PlatformerNavMeshNode SearchedNode in A_Nodes[i, j].A_JumpConnectors)
                                                                {
                                                                    if (SearchedNode == ValidNode)
                                                                    {
                                                                        NodeAlreadyConnected = true;
                                                                        break;
                                                                    }
                                                                }

                                                                if (!NodeAlreadyConnected)
                                                                {
                                                                    A_Nodes[i, j].A_JumpConnectors.Add(ValidNode);
                                                                    JumpCoordinates jumpDetails = new JumpCoordinates(TimedVerticalDisplacement, Time);
                                                                    A_Nodes[i, j].A_JumpDetails.Add(jumpDetails);
                                                                    if (FallingPointsToDraw.Count > 1)
                                                                    {
                                                                        for (int z = 1; z < FallingPointsToDraw.Count; z++)
                                                                        {
                                                                            Debug.DrawLine(new Vector3(FallingPointsToDraw[z - 1].x, FallingPointsToDraw[z - 1].y, A_Nodes[i, j].transform.position.z), new Vector3(FallingPointsToDraw[z].x, FallingPointsToDraw[z].y, A_Nodes[i, j].transform.position.z), Color.red, 100.0f);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Map is value is not set", this);
        }

       // ShowGrid(A_ShowGrid);

        Physics2D.queriesStartInColliders = false;
    }



    /*

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
    }*/
}
