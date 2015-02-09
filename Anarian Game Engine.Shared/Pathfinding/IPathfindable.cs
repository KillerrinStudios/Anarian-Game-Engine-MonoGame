using Anarian.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Pathfinding
{
    public interface IPathfindable
    {
        PathfindingData PathfindingData { get; set; }
    }
}
