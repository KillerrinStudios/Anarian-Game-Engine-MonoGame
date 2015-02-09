using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Pathfinding
{
    public class AStarNode
    {
        public static int DefaultMovementGCost = 10;

        #region Fields/Properties
        public AStarNode Parent { get; set; }
        public List<AStarNode> Connections { get; set; }

        public uint GridCellID { get; set; }
        public bool Passible { get; set; }

        public int HeuristicHValue { get; set; }
        public int MovementGCost { get; set; }
        public int TotalFCost { get; set; }
        #endregion

        //public AStarNode()
        //{
        //    Parent = null;
        //    Connections = new List<AStarNode>();           
        //    Passible = true;
        //    GridCellID = uint.MaxValue;
        //}

        public AStarNode(uint gridCellID)
        {
            Parent = null;
            Connections = new List<AStarNode>();
            Passible = true;
            GridCellID = gridCellID;

        }
        public AStarNode(uint gridCellID, AStarNode parent)
        {
            Parent = parent;
            Connections = new List<AStarNode>();
            Passible = true;
            GridCellID = gridCellID;
        }


        public void CalculateTotalFCost() 
        {
            TotalFCost = MovementGCost + HeuristicHValue;
        }
    }
}
