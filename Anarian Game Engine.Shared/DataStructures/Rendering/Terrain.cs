using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Utilities;

using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using Anarian.Helpers;
using Anarian.Collections;
using Anarian.Pathfinding;

namespace Anarian.DataStructures.Rendering
{
    public class Terrain : GameObject, IUpdatable, IRenderable
    {
        #region Fields/Properties
        Texture2D m_texture;

        public Texture2D Texture
        {
            get { return m_texture; }
            set { m_texture = value; }
        }

        BasicEffect m_effect;
        public BasicEffect Effect { get { return m_effect; } }


        TerrainHeightData m_heightData;
        public TerrainHeightData HeightData { get { return m_heightData; } }

        Grid<AStarNode> m_terrainGrid;
        public Grid<AStarNode> TerrainGrid { get { return m_terrainGrid; } }
        #endregion

        public Terrain(GraphicsDevice graphics, Texture2D heightMap, Texture2D texture = null)
            :base()
        {
            // Store the Texture
            m_texture = texture;

            // Finally, Setup the terrain
            SetupTerrain(graphics, heightMap);
        }

        public Terrain(GraphicsDevice graphics, int width, int height, Color heightmapGenerationColor, Texture2D texture = null)
            : base()
        {
            // Store the Texture
            m_texture = texture;

            // Create the Height Map
            Texture2D heightMap = heightmapGenerationColor.CreateTextureFromSolidColor(graphics, width, height);

            // Finally, Setup the terrain
            SetupTerrain(graphics, heightMap);
        }

        public static Terrain CreateFlatTerrain(GraphicsDevice graphics, int width, int height, Texture2D texture = null)
        {
            Terrain terrain = new Terrain(graphics, width, height, Color.White, texture);
            return terrain;
        }

        #region Terrain Setup
        private void SetupTerrain(GraphicsDevice graphics, Texture2D heightMap)
        {
            m_heightData = new TerrainHeightData(this, heightMap, 5.0f);

            SetupEffects(graphics);
            GenerateBoundingBox();
        }

        private void SetupEffects(GraphicsDevice graphics)
        {
            m_effect = new BasicEffect(graphics);

            if (m_texture != null) {
                m_effect.TextureEnabled = true;
                m_effect.Texture = m_texture;
            }
        }

        private void GenerateBoundingBox()
        {
            // Get list of points
            Matrix world = m_transform.WorldMatrix;
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < m_heightData.Vertices.Length; i++) {
                points.Add(Vector3.Transform(m_heightData.Vertices[i].Position, world));
            }

            m_boundingBoxes.Add(BoundingBox.CreateFromPoints(points));
        }
        #endregion

        #region Grid Setup
        public void SetupGrid(bool hookUpDiagnols, params AStarTerrainRule[] parameters)
        {
            m_terrainGrid = new Grid<AStarNode>(m_heightData.TerrainWidth, m_heightData.TerrainHeight);

            // Precreate the Data
            foreach(var i in m_terrainGrid) {
                i.Data = new AStarNode(i.ID);
            }
               
            // Apply the Grid Rules
            ApplyGridRules(parameters);

            // Find every node and hook up parents
            AddGridConnections(hookUpDiagnols);
        }

        private void ApplyGridRules(params AStarTerrainRule[] parameters)
        {
            foreach (var param in parameters)
            {
                switch (param.AppliedTo)
                {
                    case RuleTypeAppliedTo.Passable:            ApplyPassableRule(param);           break;
                    case RuleTypeAppliedTo.Impassable:          ApplyImpassableRule(param);         break;
                    case RuleTypeAppliedTo.BaseMovementCost:    ApplyBaseMovementCostRule(param);   break;
                    default: break;
                }
            }
        }

        #region Rules
        private void ApplyPassableRule(AStarTerrainRule rule)
        {
            for (int x = 0; x < m_heightData.TerrainWidth; x++)
            {
                for (int y = 0; y < m_heightData.TerrainHeight; y++)
                {
                    if (rule.Measurement == RuleMeasureType.Absolute)
                    {
                        switch (rule.Comparison)
                        {
                            case Anarian.Enumerators.Comparison.GreaterThan:
                                if (rule.Height > m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = true; }
                                //else { m_terrainGrid[x, y].Passible = false; }
                                break;
                            case Anarian.Enumerators.Comparison.EqualTo:
                                if (rule.Height == m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = true; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThan:
                                if (rule.Height < m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = true; }
                                break;

                            case Anarian.Enumerators.Comparison.GreaterThanEqualTo:
                                if (rule.Height >= m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = true; }
                                break;
                            case Anarian.Enumerators.Comparison.NotEqualTo:
                                if (rule.Height != m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = true; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThanEqualTo:
                                if (rule.Height <= m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = true; }
                                break;

                            default: break;
                        }
                    }
                    if (rule.Measurement == RuleMeasureType.Delta)
                    {
                        int x1 = x + 1;
                        int y1 = y + 1;
                        if (x1 > m_heightData.TerrainWidth) break;  //x1 = m_heightData.TerrainWidth;
                        if (y1 > m_heightData.TerrainWidth) break;  //y1 = m_heightData.TerrainHeight;
                        float deltaHeight = m_heightData.TerrainVertsPos[x1, y1].Y - m_heightData.TerrainVertsPos[x, y].Y;

                        switch (rule.Comparison)
                        {
                            case Anarian.Enumerators.Comparison.GreaterThan:
                                if (rule.Height > deltaHeight) { m_terrainGrid[x, y].Passible = true; }
                                break;
                            case Anarian.Enumerators.Comparison.EqualTo:
                                if (rule.Height == deltaHeight) { m_terrainGrid[x, y].Passible = true; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThan:
                                if (rule.Height < deltaHeight) { m_terrainGrid[x, y].Passible = true; }
                                break;

                            case Anarian.Enumerators.Comparison.GreaterThanEqualTo:
                                if (rule.Height >= deltaHeight) { m_terrainGrid[x, y].Passible = true; }
                                break;
                            case Anarian.Enumerators.Comparison.NotEqualTo:
                                if (rule.Height != deltaHeight) { m_terrainGrid[x, y].Passible = true; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThanEqualTo:
                                if (rule.Height <= deltaHeight) { m_terrainGrid[x, y].Passible = true; }
                                break;

                            default: break;
                        }
                    }
                }
            }
        }
        private void ApplyImpassableRule(AStarTerrainRule rule)
        {
            for (int x = 0; x < m_heightData.TerrainWidth; x++)
            {
                for (int y = 0; y < m_heightData.TerrainHeight; y++)
                {
                    if (rule.Measurement == RuleMeasureType.Absolute)
                    {
                        switch (rule.Comparison)
                        {
                            case Anarian.Enumerators.Comparison.GreaterThan:
                                if (rule.Height > m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = false; }
                                //else { m_terrainGrid[x, y].Passible = true; }
                                break;
                            case Anarian.Enumerators.Comparison.EqualTo:
                                if (rule.Height == m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = false; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThan:
                                if (rule.Height < m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = false; }
                                break;

                            case Anarian.Enumerators.Comparison.GreaterThanEqualTo:
                                if (rule.Height >= m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = false; }
                                break;
                            case Anarian.Enumerators.Comparison.NotEqualTo:
                                if (rule.Height != m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = false; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThanEqualTo:
                                if (rule.Height <= m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].Passible = false; }
                                break;

                            default: break;
                        }
                    }
                    if (rule.Measurement == RuleMeasureType.Delta)
                    {
                        int x1 = x + 1;
                        int y1 = y + 1;
                        if (x1 > m_heightData.TerrainWidth) break;  //x1 = m_heightData.TerrainWidth;
                        if (y1 > m_heightData.TerrainWidth) break;  //y1 = m_heightData.TerrainHeight;
                        float deltaHeight = m_heightData.TerrainVertsPos[x1, y1].Y - m_heightData.TerrainVertsPos[x, y].Y;

                        switch (rule.Comparison)
                        {
                            case Anarian.Enumerators.Comparison.GreaterThan:
                                if (rule.Height > deltaHeight) { m_terrainGrid[x, y].Passible = false; }
                                break;
                            case Anarian.Enumerators.Comparison.EqualTo:
                                if (rule.Height == deltaHeight) { m_terrainGrid[x, y].Passible = false; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThan:
                                if (rule.Height < deltaHeight) { m_terrainGrid[x, y].Passible = false; }
                                break;

                            case Anarian.Enumerators.Comparison.GreaterThanEqualTo:
                                if (rule.Height >= deltaHeight) { m_terrainGrid[x, y].Passible = false; }
                                break;
                            case Anarian.Enumerators.Comparison.NotEqualTo:
                                if (rule.Height != deltaHeight) { m_terrainGrid[x, y].Passible = false; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThanEqualTo:
                                if (rule.Height <= deltaHeight) { m_terrainGrid[x, y].Passible = false; }
                                break;

                            default: break;
                        }
                    }
                }
            }
        }
        private void ApplyBaseMovementCostRule(AStarTerrainRule rule)
        {
            for (int x = 0; x < m_heightData.TerrainWidth; x++)
            {
                for (int y = 0; y < m_heightData.TerrainHeight; y++)
                {
                    if (rule.Measurement == RuleMeasureType.Absolute)
                    {
                        switch (rule.Comparison)
                        {
                            case Anarian.Enumerators.Comparison.GreaterThan:
                                if (rule.Height > m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;
                            case Anarian.Enumerators.Comparison.EqualTo:
                                if (rule.Height == m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThan:
                                if (rule.Height < m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;

                            case Anarian.Enumerators.Comparison.GreaterThanEqualTo:
                                if (rule.Height >= m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;
                            case Anarian.Enumerators.Comparison.NotEqualTo:
                                if (rule.Height != m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThanEqualTo:
                                if (rule.Height <= m_heightData.TerrainVertsPos[x, y].Y) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;

                            default: break;
                        }
                    }
                    if (rule.Measurement == RuleMeasureType.Delta)
                    {
                        int x1 = x + 1;
                        int y1 = y + 1;
                        if (x1 > m_heightData.TerrainWidth) break;  //x1 = m_heightData.TerrainWidth;
                        if (y1 > m_heightData.TerrainWidth) break;  //y1 = m_heightData.TerrainHeight;
                        float deltaHeight = m_heightData.TerrainVertsPos[x1, y1].Y - m_heightData.TerrainVertsPos[x, y].Y;

                        switch (rule.Comparison)
                        {
                            case Anarian.Enumerators.Comparison.GreaterThan:
                                if (rule.Height > deltaHeight) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;
                            case Anarian.Enumerators.Comparison.EqualTo:
                                if (rule.Height == deltaHeight) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThan:
                                if (rule.Height < deltaHeight) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;

                            case Anarian.Enumerators.Comparison.GreaterThanEqualTo:
                                if (rule.Height >= deltaHeight) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;
                            case Anarian.Enumerators.Comparison.NotEqualTo:
                                if (rule.Height != deltaHeight) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;
                            case Anarian.Enumerators.Comparison.LessThanEqualTo:
                                if (rule.Height <= deltaHeight) { m_terrainGrid[x, y].MovementGCost += rule.Amount; }
                                break;

                            default: break;
                        }
                    }
                }
            }
        }
        #endregion
        
        private void AddGridConnections(bool hookupDiagnols)
        {
            for (int x = 0; x < m_terrainGrid.Columns; x++)
            {
                for (int y = 0; y < m_terrainGrid.Rows; y++)
                {
                    Point thisNodePos = new Point(x, y);

                    // Up Down Left Right
                    Point northPos = thisNodePos + new Point(0, 1);
                    Point southPos = thisNodePos - new Point(0, 1);
                    Point eastPos = thisNodePos + new Point(1, 0);
                    Point westPos = thisNodePos - new Point(1, 0);

                    if (m_terrainGrid.InGrid(northPos))
                    {
                        var north = m_terrainGrid[northPos];
                        if (north.Passible) {
                            Debug.WriteLine("Found North: " + north.GridCellID.ToString());
                            m_terrainGrid[x, y].Connections.Add(north);
                        }
                    }
                    if (m_terrainGrid.InGrid(southPos))
                    {
                        var south = m_terrainGrid[southPos];
                        if (south.Passible) {
                            Debug.WriteLine("Found South: " + south.GridCellID.ToString());
                            m_terrainGrid[x, y].Connections.Add(south);
                        }
                    }

                    if (m_terrainGrid.InGrid(eastPos))
                    {
                        var east = m_terrainGrid[eastPos];
                        if (east.Passible) {
                            Debug.WriteLine("Found East: " + east.GridCellID.ToString());
                            m_terrainGrid[x, y].Connections.Add(east);
                        }
                    }
                    if (m_terrainGrid.InGrid(westPos))
                    {
                        var west = m_terrainGrid[westPos];
                        if (west.Passible) {
                            Debug.WriteLine("Found West: " + west.GridCellID.ToString());
                            m_terrainGrid[x, y].Connections.Add(west);
                        }
                    }

                    // Diagnols
                    if (hookupDiagnols)
                    {
                        Point northEastPos = thisNodePos + new Point(1, 1);
                        Point northWestPos = thisNodePos - new Point(1, 1);
                        Point southEastPos = thisNodePos + new Point(1, 1);
                        Point southWestPos = thisNodePos - new Point(1, 1);

                        if (m_terrainGrid.InGrid(northEastPos))
                        {
                            var northeast = m_terrainGrid[northEastPos];
                            if (northeast.Passible) {
                                Debug.WriteLine("Found northEastPos: " + northeast.GridCellID.ToString());
                                m_terrainGrid[x, y].Connections.Add(northeast);
                            }
                        }
                        if (m_terrainGrid.InGrid(northWestPos))
                        {
                            var northwest = m_terrainGrid[northWestPos];
                            if (northwest.Passible) {
                                Debug.WriteLine("Found northWestPos: " + northwest.GridCellID.ToString());
                                m_terrainGrid[x, y].Connections.Add(northwest);
                            }
                        }

                        if (m_terrainGrid.InGrid(southEastPos))
                        {
                            var southeast = m_terrainGrid[southEastPos];
                            if (southeast.Passible) {
                                Debug.WriteLine("Found southEastPos: " + southeast.GridCellID.ToString());
                                m_terrainGrid[x, y].Connections.Add(southeast);
                            }
                        }
                        if (m_terrainGrid.InGrid(southWestPos))
                        {
                            var southwest = m_terrainGrid[southWestPos];
                            if (southwest.Passible) {
                                Debug.WriteLine("Found southWestPos: " + southwest.GridCellID.ToString());
                                m_terrainGrid[x, y].Connections.Add(southwest);
                            }
                        }
                    }
                }
            }
        }

        public void DebugPrintGrid()
        {
            if (m_terrainGrid == null) return;

            for (int x = 0; x < m_terrainGrid.Columns; x++)
            {
                string line = "";
                for (int y = 0; y < m_terrainGrid.Rows; y++)
                {
                    line += Convert.ToInt32(m_terrainGrid[x, y].Passible).ToString();
                }
                Debug.WriteLine(line);
            }
        }
        #endregion

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        /// <summary>
        /// Checks if the Ray Intersects with the GameObjects BoundingBox
        /// </summary>
        /// <param name="ray">A Ray which we will test intersection with</param>
        /// <returns>A Boolean which represents if an intersection occured</returns>
        public override bool CheckRayIntersection(Ray ray)
        {
            return base.CheckRayIntersection(ray);
        }

        /// <summary>
        /// Checks along the X and Z axis' to determine if the point is hovering above or below the Terrain
        /// </summary>
        /// <param name="point">A position to check against</param>
        /// <returns></returns>
        public bool IsOnHeightmap(Vector3 point)
        {
            return IsOnHeightmap(point.X, point.Z);
        }
       
        /// <summary>
        /// Checks along the X and Z axis' to determine if the point is hovering above or below the Terrain
        /// </summary>
        /// <param name="pointX">A position along the X Axis</param>
        /// <param name="pointZ">A position along the Z Axis</param>
        /// <returns></returns>
        public bool IsOnHeightmap(float pointX, float pointZ)
        {
            foreach (var bound in m_boundingBoxes) {
                if (pointX > bound.Min.X &&
                    pointX < bound.Max.X &&
                    //point.Y > bound.Min.Y ||
                    //point.Y < bound.Max.Y ||
                    pointZ > bound.Min.Z &&
                    pointZ < bound.Max.Z) {
                    return true;
                }
            }
            return false;
        }

        #region Picking
        /// <summary>
        /// Get a point on the Terrain at the position of intersection
        /// </summary>
        /// <param name="ray">A Ray which will be traversed</param>
        /// <returns>Vector3 containing Position in World Space. Returns null if no intersection occurs</returns>
        public Vector3? Intersects(Ray ray)
        {
            Ray? currentRay = LinearSearch(ray);
            if (currentRay == null) return null;

            return BinarySearch(currentRay.Value);
        }

        private Ray? LinearSearch(Ray ray)
        {
            ray.Direction /= 50.0f;
            Vector3 nextPoint = ray.Position + ray.Direction;

            float heightAtNextPoint = GetHeightAtPoint(nextPoint.X, nextPoint.Z);
            if (heightAtNextPoint == float.MaxValue) return null;

            while (heightAtNextPoint < nextPoint.Y) {
                ray.Position = nextPoint;
                nextPoint = ray.Position + ray.Direction;

                heightAtNextPoint = GetHeightAtPoint(nextPoint.X, nextPoint.Z);
                if (heightAtNextPoint == float.MaxValue) return null;
            }
            return ray;
        }
        private Vector3? BinarySearch(Ray ray)
        {
            float accuracy = 0.01f;
            
            float heightAtStartingPoint = GetHeightAtPoint(ray.Position.X, ray.Position.Z);
            if (heightAtStartingPoint == float.MaxValue) return null;

            float currentError = ray.Position.Y - heightAtStartingPoint;
            int counter = 0;
            while (currentError > accuracy) {
                ray.Direction /= 2.0f;
                Vector3 nextPoint = ray.Position + ray.Direction;

                float heightAtNextPoint = GetHeightAtPoint(nextPoint.X, nextPoint.Z);
                if (heightAtNextPoint == float.MaxValue) return null;

                if (nextPoint.Y > heightAtNextPoint) {
                    ray.Position = nextPoint;
                    currentError = ray.Position.Y - heightAtNextPoint;
                }
                if (counter++ == 1000) break;
            }
            return ray.Position;
        }

        /// <summary>
        /// Gets the Height at a point on the Terrain
        /// </summary>
        /// <param name="point">The Vector3 Position of the point with data oriented along X/Z Axis</param>
        /// <returns>The height for the given point on the map</returns>
        public float GetHeightAtPoint(Vector3 point)
        {
            return GetHeightAtPoint(point.X, point.Z);
        }

        /// <summary>
        /// Gets the Height at a point on the Terrain
        /// </summary>
        /// <param name="pointX">The X Position of the point along the plane</param>
        /// <param name="pointZ">The Z Position of the point along the plane</param>
        /// <returns>The height for the given point on the map</returns>
        public float GetHeightAtPoint(float pointX, float pointZ)
        {
            if (!IsOnHeightmap(pointX, pointZ)) return float.MaxValue;

            // Pre calculate the World Matrix
            Matrix world = m_transform.WorldMatrix;

            // Hold the Grid Counters
            int posX = -1;
            int posZ = -1;

            // Search Along the X
            for (int x = 0; x < m_heightData.TerrainWidth; x++) {
                Vector3 vertAtWorld = Vector3.Transform(m_heightData.TerrainVertsPos[x, 0], world);

                if (pointX <= vertAtWorld.X) {
                    //Debug.WriteLine("PointX Pos: {0} | VertAtWorld: {1} | VertGridSpace: {2}", pointX, vertAtWorld.X, x);
                    posX = x;
                    break;
                }
            }

            // Search along the Z
            for (int z = 0; z < m_heightData.TerrainHeight; z++) {
                Vector3 vertAtWorld = Vector3.Transform(m_heightData.TerrainVertsPos[0, z], world);

                if (pointZ >= vertAtWorld.Z) {
                    //Debug.WriteLine("PointZ Pos: {0} | VertAtWorld: {1} | VertGridSpace: {2}", pointZ, vertAtWorld.Z, z);
                    posZ = z;
                    break;
                }
            }

            // If any of the values are still bad, return empty
            if (posX == -1 || posZ == -1) 
            {
                Debug.WriteLine("A Value on the Terrain Grid could not be found: {0}, {1}", posX, posZ);
                return float.MaxValue;
            }

            // Get the vertex position
            Vector3 vert = Vector3.Transform(m_heightData.TerrainVertsPos[posX, posZ], world);

            // See if we can lerp the values
            if (posX > 0 && posZ > 0) {
                // Lerp the Data from the four vertices to get the average height
                Vector3 vert2 = Vector3.Transform(m_heightData.TerrainVertsPos[posX - 1, posZ], world);
                Vector3 vert3 = Vector3.Transform(m_heightData.TerrainVertsPos[posX, posZ - 1], world);
                Vector3 vert4 = Vector3.Transform(m_heightData.TerrainVertsPos[posX - 1, posZ - 1], world);

                float averageHeight = (vert.Y + vert2.Y + vert3.Y + vert4.Y) / 4.0f;

                // Lerp the Average Height to get the height according to our current position
                return averageHeight;
            }

            // If we can't lerp the values, we continue on and just use the current vertex to get the height 
            // Now we get the height data
            float height = vert.Y;
            
            // ToDo: Lerp the value between the two and return it
            //Debug.WriteLine("Terrain Height: {0} \n", height);
            return height;
        }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;

            // We first Update the Children
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (!m_active) return;
            if (!m_visible) return;

            // We Draw the base here so that the Children get taken care of
            base.Draw(gameTime, spriteBatch, graphics, camera);

            // Prep the Graphics Device
            graphics.RasterizerState.CullMode = CullMode.None;

            // Begin Drawing the World
            // Since the world will be generated outwards from its side, we are offsetting the orgin of the world to its center
            m_effect.World = m_transform.WorldMatrix;
            m_effect.View = camera.View;
            m_effect.Projection = camera.Projection;
            
            // Setup User Defined Effects
            SetupEffects(m_effect, graphics, camera, gameTime);

            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes) {
                pass.Apply();

                graphics.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    m_heightData.Vertices, 0, m_heightData.Vertices.Length,
                    m_heightData.Indices, 0, m_heightData.Indices.Length / 3,
                    VertexPositionNormalTexture.VertexDeclaration);
            }

            //m_boundingBox.DrawBoundingBox(graphics, Color.Red, camera, Matrix.Identity);
        }

        protected virtual void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            m_effect.EnableDefaultLighting();
        }
        #endregion
    }
}
