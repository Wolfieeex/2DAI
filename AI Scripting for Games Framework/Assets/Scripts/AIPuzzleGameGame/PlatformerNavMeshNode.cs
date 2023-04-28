using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileAssigner
{
    Invalid,
    Middle,
    LeftEdge,
    RightEdge,
    Single
}

public struct JumpCoordinates
{
    public float jumpSpeed;
    public float time;

    public JumpCoordinates(float jump, float period)
    {
        this.jumpSpeed = jump;
        this.time = period;
    }
}

public class PlatformerNavMeshNode : MonoBehaviour
{
    [SerializeField] Color BlankTileColour;
    [SerializeField] Color TerrainTileColour;
    [SerializeField] Color MiddleTileColour;
    [SerializeField] Color LeftEdgeTileColour;
    [SerializeField] Color RightEdgeTileColour;
    [SerializeField] Color SingleTileColour;

    [SerializeField] Sprite BlankTileSpriteRenderer;
    [SerializeField] Sprite TerrainTileSpriteRenderer;
    [SerializeField] Sprite MiddleTileSpriteRenderer;
    [SerializeField] Sprite LeftEdgeTileSpriteRenderer;
    [SerializeField] Sprite RightEdgeTileSpriteRenderer;
    [SerializeField] Sprite SingleTileSpriteRenderer;

    PlatformerNavMesh A_Generator;
    SpriteRenderer A_SpriteRenderer;

    public bool A_IsTerrain;
    public int A_PlatformID;
    public TileAssigner A_TileType;

    public List<PlatformerNavMeshNode> A_PlatformConnectors = new List<PlatformerNavMeshNode>();
    public List<PlatformerNavMeshNode> A_DropConnectors = new List<PlatformerNavMeshNode>();
    public List<PlatformerNavMeshNode> A_JumpConnectors = new List<PlatformerNavMeshNode>();
    public List<JumpCoordinates> A_JumpDetails = new List<JumpCoordinates>();

    
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
    public PlatformerNavMeshNode[] A_Neighbours;

    public void Awake()
    {
        A_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(PlatformerNavMesh generator, PlatformerNavMeshNode up, PlatformerNavMeshNode upRight, PlatformerNavMeshNode right, PlatformerNavMeshNode downRight, PlatformerNavMeshNode down, PlatformerNavMeshNode downLeft, PlatformerNavMeshNode left, PlatformerNavMeshNode upLeft)
    {
        A_Neighbours = new PlatformerNavMeshNode[8] { up, upRight, right, downRight, down, downLeft, left, upLeft };
        A_Generator = generator;
        CheckForTerrain();
    }

    public void CheckForTerrain()
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];
        Vector2 RaycatsBoxScale =  new Vector2(transform.localScale.x - 0.05f, transform.localScale.y - 0.05f);
        Physics2D.BoxCast(transform.position, RaycatsBoxScale, transform.rotation.eulerAngles.z, transform.forward, A_Generator.m_ContactFilter, hits);
 
        if(hits[0]) A_IsTerrain = true;
        else A_IsTerrain = false;

        
    }

    public void VisualiseTileType()
    {
        if (A_IsTerrain) 
        {
            A_SpriteRenderer.color = TerrainTileColour;
            A_SpriteRenderer.sprite = TerrainTileSpriteRenderer;
        }
        else
        {
            A_SpriteRenderer.color = BlankTileColour;
            A_SpriteRenderer.sprite = BlankTileSpriteRenderer;
        }


        switch (A_TileType)
        {
            case TileAssigner.Middle:
                A_SpriteRenderer.color = MiddleTileColour;
                A_SpriteRenderer.sprite = MiddleTileSpriteRenderer;
                break;
            case TileAssigner.LeftEdge:
                A_SpriteRenderer.color = LeftEdgeTileColour;
                A_SpriteRenderer.sprite = LeftEdgeTileSpriteRenderer;
                break;
            case TileAssigner.RightEdge:
                A_SpriteRenderer.color = RightEdgeTileColour;
                A_SpriteRenderer.sprite = RightEdgeTileSpriteRenderer;
                break;
            case TileAssigner.Single:
                A_SpriteRenderer.color = SingleTileColour;
                A_SpriteRenderer.sprite = SingleTileSpriteRenderer;
                break;        
        }
    }
}
