using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFindingBFS : PathFinding
{
    public PathFindingBFS(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners) { }


    public override void GeneratePath(GridNode start, GridNode end)
    {

    }
}
