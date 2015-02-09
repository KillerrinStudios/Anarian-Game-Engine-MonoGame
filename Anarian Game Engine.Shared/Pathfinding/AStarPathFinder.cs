using Anarian.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Anarian.Pathfinding
{
    public static class AStarPathFinder
    {
        public static event PathFoundEventHandler PathFound;

        public static int CalculateManhattanDistance(Point currentGridPos, Point endGridPos)
        {
            return (int)(Math.Abs(currentGridPos.X - endGridPos.X) + Math.Abs(currentGridPos.Y - endGridPos.Y));
        }

        /// <summary>
        /// Uses A* to pathfind towards the target
        /// </summary>
        /// <param name="pathfindable">The pathfinding data we will use to get to the target</param>
        /// <returns>True if path found, false if not</returns>
        public static bool FindPath(IPathfindable pathfindable, Grid<AStarNode> grid, bool includeDiagnols = true)
        {
            if (!pathfindable.PathfindingData.ReadyToPreformPathfinding()) return false;
            if (pathfindable.PathfindingData.OpenedList.Count == 0) return false;
            if (pathfindable.PathfindingData.FoundTarget) return true;

            var pathfindingData = pathfindable.PathfindingData;

            bool loop = true;
            while (loop)
            {
                for (int i = 0; i < pathfindingData.CheckingNode.Connections.Count; i++) {
                    if (pathfindingData.CheckingNode.Connections[i] != null) {
                        DetermineNodeValues(pathfindingData, pathfindingData.CheckingNode.Connections[i], grid);
                    }
                }

                if (!pathfindingData.FoundTarget) {
                    pathfindingData.AddToClosedList(pathfindingData.CheckingNode);
                    pathfindingData.CheckingNode = pathfindingData.OpenedList.Dequeue().Data;
                    FindAdjacentNodes(pathfindingData, grid, includeDiagnols);
                }
                else { loop = false; break; }
            }

            if (pathfindingData.FoundTarget)
            {
                TraceBackPath(pathfindingData);

                if (PathFound != null)
                    PathFound(null, new PathFoundEventArgs(null, pathfindable));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Traces back the path and saves it to the pathfinding data for use
        /// </summary>
        /// <param name="pathfindingData">The pathfinding data we will save the path to</param>
        static void TraceBackPath(PathfindingData pathfindingData)
        {
            Debug.WriteLine("Path Found");
            AStarNode node = pathfindingData.EndNode;

            do {
                pathfindingData.FoundPath.Insert(0, node);
                node = node.Parent; 
            }
            while (node != null);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathfindingData"></param>
        /// <param name="targetNode"></param>
        /// <param name="grid">A grid from which we can use to grab cells off of</param>
        static void DetermineNodeValues(PathfindingData pathfindingData, AStarNode targetNode, Grid<AStarNode> grid)
        {
            if (targetNode == null) return;
            if (pathfindingData.CheckingNode.GridCellID == targetNode.GridCellID) return;
            if (pathfindingData.EndNode.GridCellID == targetNode.GridCellID)
            {
                pathfindingData.EndNode.Parent = pathfindingData.CheckingNode;
                pathfindingData.FoundTarget = true;
                return;
            }

            // Check the Grid Cell to see if it is passable
            if (!targetNode.Passible)
                return;

            if (!pathfindingData.ClosedListContains(targetNode))
            {
                targetNode.Parent = pathfindingData.CheckingNode;
                targetNode.HeuristicHValue = CalculateManhattanDistance(grid.GetColumnRowFromID(targetNode.GridCellID), grid.GetColumnRowFromID(pathfindingData.EndNode.GridCellID));

                if (pathfindingData.OpenedListContains(targetNode))
                {
                    int newGCost = pathfindingData.CheckingNode.MovementGCost + pathfindingData.BaseMovementCost;

                    if (newGCost < targetNode.MovementGCost)
                    {
                        targetNode.MovementGCost = newGCost;
                        targetNode.CalculateTotalFCost();
                    }
                }
                else
                {
                    targetNode.MovementGCost = pathfindingData.CheckingNode.MovementGCost + pathfindingData.BaseMovementCost;
                    targetNode.CalculateTotalFCost();
                    pathfindingData.AddToOpenedList(targetNode);
                }
            }
        }

        static AStarNode CreateNodeAtGridSpace(PathfindingData pathfindingData, Grid<AStarNode> grid, Point gridPosition, AStarNode parent)
        {
            AStarNode temp = new AStarNode(grid.GetIDFromColumnRow(gridPosition), parent);
            temp.MovementGCost = pathfindingData.BaseMovementCost;
            temp.HeuristicHValue = CalculateManhattanDistance(grid.GetColumnRowFromID(temp.GridCellID), grid.GetColumnRowFromID(pathfindingData.EndNode.GridCellID));
            temp.CalculateTotalFCost();
            return temp;
        }

        /// <summary>
        /// Finds and assigns the adjacent nodes to our inputted AStarNode.
        /// </summary>
        /// <param name="node">Node we will get the adjacent nodes for</param>
        /// <param name="includeDiagnols">If True, it will include diagnols in the search</param>
        public static void FindAdjacentNodes(PathfindingData pathfindingData, Grid<AStarNode> grid, bool includeDiagnols)
        {
            uint parentGridCellID;
            Point? parentPos = new Point(-1, -1);
            if (pathfindingData.CheckingNode.Parent != null)
            {
                parentGridCellID = pathfindingData.CheckingNode.Parent.GridCellID;
                parentPos = grid.GetColumnRowFromID(parentGridCellID);
            }

            uint thisNodeID = pathfindingData.CheckingNode.GridCellID;
            Point? thisNodePos = grid.GetColumnRowFromID(thisNodeID);
            Point northPos = thisNodePos.Value + new Point(0, 1);
            Point southPos = thisNodePos.Value - new Point(0, 1);
            Point eastPos = thisNodePos.Value + new Point(1, 0);
            Point westPos = thisNodePos.Value - new Point(1, 0);

            // Up Down Left Right
            if (grid.InGrid(northPos))
            {
                if (parentPos != northPos &&
                    grid[northPos].Passible)
                {

                    AStarNode north = CreateNodeAtGridSpace(pathfindingData, grid, northPos, pathfindingData.CheckingNode);
                    Debug.WriteLine("Found North: " + north.GridCellID.ToString());
                    pathfindingData.CheckingNode.Connections.Add(north);
                }
            }
            if (grid.InGrid(southPos))
            {
                if (parentPos != southPos &&
                    grid[southPos].Passible)
                {

                    AStarNode south = CreateNodeAtGridSpace(pathfindingData, grid, southPos, pathfindingData.CheckingNode);
                    Debug.WriteLine("Found South: " + south.GridCellID.ToString());
                    pathfindingData.CheckingNode.Connections.Add(south);
                }
            }

            if (grid.InGrid(eastPos))
            {
                if (parentPos != eastPos &&
                    grid[eastPos].Passible)
                {

                    AStarNode east = CreateNodeAtGridSpace(pathfindingData, grid, eastPos, pathfindingData.CheckingNode);
                    Debug.WriteLine("Found East: " + east.GridCellID.ToString());
                    pathfindingData.CheckingNode.Connections.Add(east);
                }
            }
            if (grid.InGrid(westPos))
            {
                if (parentPos != westPos &&
                    grid[westPos].Passible)
                {

                    AStarNode west = CreateNodeAtGridSpace(pathfindingData, grid, westPos, pathfindingData.CheckingNode);
                    Debug.WriteLine("Found West: " + west.GridCellID.ToString());
                    pathfindingData.CheckingNode.Connections.Add(west);
                }
            }

            // Diagnols
            if (includeDiagnols)
            {
                Point northEastPos = thisNodePos.Value + new Point(1, 1);
                Point northWestPos = thisNodePos.Value - new Point(1, 1);
                Point southEastPos = thisNodePos.Value + new Point(1, 1);
                Point southWestPos = thisNodePos.Value - new Point(1, 1);

                if (grid.InGrid(northEastPos))
                {
                    if (parentPos != northEastPos &&
                        grid[northEastPos].Passible)
                    {

                        AStarNode north = CreateNodeAtGridSpace(pathfindingData, grid, northEastPos, pathfindingData.CheckingNode);
                        Debug.WriteLine("Found northEastPos: " + north.GridCellID.ToString());
                        pathfindingData.CheckingNode.Connections.Add(north);
                    }
                }
                if (grid.InGrid(northWestPos))
                {
                    if (parentPos != northWestPos &&
                        grid[northWestPos].Passible)
                    {

                        AStarNode south = CreateNodeAtGridSpace(pathfindingData, grid, northWestPos, pathfindingData.CheckingNode);
                        Debug.WriteLine("Found northWestPos: " + south.GridCellID.ToString());
                        pathfindingData.CheckingNode.Connections.Add(south);
                    }
                }

                if (grid.InGrid(southEastPos))
                {
                    if (parentPos != southEastPos &&
                        grid[southEastPos].Passible)
                    {

                        AStarNode east = CreateNodeAtGridSpace(pathfindingData, grid, southEastPos, pathfindingData.CheckingNode);
                        Debug.WriteLine("Found southEastPos: " + east.GridCellID.ToString());
                        pathfindingData.CheckingNode.Connections.Add(east);
                    }
                }
                if (grid.InGrid(southWestPos))
                {
                    if (parentPos != southWestPos &&
                        grid[southWestPos].Passible)
                    {

                        AStarNode west = CreateNodeAtGridSpace(pathfindingData, grid, southWestPos, pathfindingData.CheckingNode);
                        Debug.WriteLine("Found southWestPos: " + west.GridCellID.ToString());
                        pathfindingData.CheckingNode.Connections.Add(west);
                    }
                }
            }
        }

    }
}
