using Anarian.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Pathfinding
{
    public class PathfindingData
    {
        public bool FoundTarget;
        public int BaseMovementCost;

        public PriorityQueue<PriorityQueueNode<AStarNode>> OpenedList;
        public List<AStarNode> ClosedList;

        public List<AStarNode> FoundPath;

        public AStarNode CheckingNode;
        public AStarNode StartNode;
        public AStarNode EndNode;

        #region Initializations
        public PathfindingData()
        {
            Reset();
        }

        public void Reset()
        {
            FoundTarget = false;
            BaseMovementCost = 10;

            OpenedList = new PriorityQueue<PriorityQueueNode<AStarNode>>();
            ClosedList = new List<AStarNode>();
            FoundPath = new List<AStarNode>();

            CheckingNode = null;
            StartNode = null;
            EndNode = null;
        }

        public void Setup(AStarNode startNode, AStarNode endNode)
        {
            Reset();

            StartNode = startNode;
            CheckingNode = StartNode;
            EndNode = endNode;

            AddToOpenedList(StartNode);
        }
        #endregion

        public bool ReadyToPreformPathfinding()
        {
            return (StartNode       != null) &&
                   (EndNode         != null) &&
                   (CheckingNode    != null);
        }

        #region Helper Methods
        public void AddToOpenedList(AStarNode node)
        {
            OpenedList.Enqueue(new PriorityQueueNode<AStarNode>(node, node.HeuristicHValue));
        }
        public void AddToClosedList(AStarNode currentNode)
        {
            if (!ClosedListContains(currentNode))
                ClosedList.Add(currentNode);
        }

        public bool OpenedListContains(AStarNode n)
        {
            for (int i = 0; i < OpenedList.Count; i++)
            {
                if (OpenedList[i].Data.GridCellID == n.GridCellID)
                    return true;
            }
            return false;
        }
        public bool ClosedListContains(AStarNode n)
        {
            for (int i = 0; i < ClosedList.Count; i++)
            {
                if (ClosedList[i].GridCellID == n.GridCellID)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
