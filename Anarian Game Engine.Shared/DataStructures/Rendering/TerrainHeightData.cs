using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.DataStructures.Rendering
{
    public class TerrainHeightData
    {
        Terrain m_terrain;
        public Terrain Terrain { get { return m_terrain; } }

        #region Fields/Properties
        Texture2D m_heightMap;
        public Texture2D HeightMap
        {
            get { return m_heightMap; }
            protected set { m_heightMap = value; }
        }

        VertexPositionNormalTexture[] m_vertices;
        public VertexPositionNormalTexture[] Vertices { get { return m_vertices; } }
        
        int[] m_indices;
        public int[] Indices { get { return m_indices; } }

        Vector3[,] m_terrainVertsPos;
        public Vector3[,] TerrainVertsPos { get { return m_terrainVertsPos; } }

        float[,] m_heightData;
        public float[,] HeightData { get { return m_heightData; } }

        int m_terrainWidth = 0;
        public int TerrainWidth { get { return m_terrainWidth; } }

        int m_terrainHeight = 0;
        public int TerrainHeight { get { return m_terrainHeight; } }


        float m_highestHeightPoint;
        public float HighestHeight { get { return m_highestHeightPoint * m_terrain.Transform.WorldScale.Y; } }

        float m_lowestHeightPoint;
        public float LowestHeight { get { return m_lowestHeightPoint * m_terrain.Transform.WorldScale.Y; } }


        float m_terrainHeightScale;
        public float TerrainHeightScale { get { return m_terrainHeightScale; } }
        #endregion

        public TerrainHeightData(Terrain terrain, Texture2D heightMap, float terrainHeightScale = 5.0f)
        {
            m_terrain = terrain;
            SetupTerrain(heightMap, terrainHeightScale);
        }

        public void SetupTerrain(Texture2D heightMap, float terrainHeightScale)
        {
            LoadHeightData(heightMap, terrainHeightScale);

            SetUpVertices();
            SetUpIndices();
            CalculateNormals();
        }

        private void LoadHeightData(Texture2D heightMap, float terrainHeightScale)
        {
            m_heightMap = heightMap;
            m_terrainWidth = heightMap.Width;
            m_terrainHeight = heightMap.Height;

            m_terrainHeightScale = terrainHeightScale;

            float tempLowestHeight = float.MaxValue;
            float tempHighestHeight = float.MinValue;

            Color[] heightMapColors = new Color[m_terrainWidth * m_terrainHeight];
            heightMap.GetData(heightMapColors);

            m_heightData = new float[m_terrainWidth, m_terrainHeight];
            for (int x = 0; x < m_terrainWidth; x++) {
                for (int y = 0; y < m_terrainHeight; y++) {
                    m_heightData[x, y] = heightMapColors[x + y * m_terrainWidth].R / m_terrainHeightScale;

                    if (m_heightData[x, y] < tempLowestHeight) { tempLowestHeight = m_heightData[x, y]; }
                    if (m_heightData[x, y] > tempHighestHeight) { tempHighestHeight = m_heightData[x, y]; }
                }
            }

            m_lowestHeightPoint = tempLowestHeight;
            m_highestHeightPoint = tempHighestHeight;
        }

        private void SetUpVertices()
        {
            Vector3 centerAlign = new Vector3(-m_terrainWidth / 2.0f, 0, m_terrainHeight / 2.0f);

            m_vertices = new VertexPositionNormalTexture[m_terrainWidth * m_terrainHeight];
            m_terrainVertsPos = new Vector3[m_terrainWidth, m_terrainHeight];

            for (int x = 0; x < m_terrainWidth; x++) {
                for (int y = 0; y < m_terrainHeight; y++) {
                    int vertIndex = x + y * m_terrainWidth;
                    //Debug.WriteLine("Terrain Setup: {0}", vertIndex);

                    m_terrainVertsPos[x, y] = new Vector3(x, m_heightData[x, y], -y) + centerAlign;
                    m_vertices[vertIndex].Position = m_terrainVertsPos[x, y];

                    m_vertices[vertIndex].TextureCoordinate.X = (float)x / 30.0f;
                    m_vertices[vertIndex].TextureCoordinate.Y = (float)y / 30.0f;
                }
                //Debug.WriteLine("\n");
            }
        }

        private void SetUpIndices()
        {
            m_indices = new int[(m_terrainWidth - 1) * (m_terrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < m_terrainHeight - 1; y++) {
                for (int x = 0; x < m_terrainWidth - 1; x++) {
                    int lowerLeft = x + y * m_terrainWidth;
                    int lowerRight = (x + 1) + y * m_terrainWidth;
                    int topLeft = x + (y + 1) * m_terrainWidth;
                    int topRight = (x + 1) + (y + 1) * m_terrainWidth;

                    m_indices[counter++] = topLeft;
                    m_indices[counter++] = lowerRight;
                    m_indices[counter++] = lowerLeft;

                    m_indices[counter++] = topLeft;
                    m_indices[counter++] = topRight;
                    m_indices[counter++] = lowerRight;
                }
            }
        }

        private void CalculateNormals()
        {
            for (int i = 0; i < m_vertices.Length; i++)
                m_vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < m_indices.Length / 3; i++) {
                int index1 = m_indices[i * 3];
                int index2 = m_indices[i * 3 + 1];
                int index3 = m_indices[i * 3 + 2];

                Vector3 side1 = m_vertices[index1].Position - m_vertices[index3].Position;
                Vector3 side2 = m_vertices[index1].Position - m_vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                m_vertices[index1].Normal += normal;
                m_vertices[index2].Normal += normal;
                m_vertices[index3].Normal += normal;
            }

            for (int i = 0; i < m_vertices.Length; i++)
                m_vertices[i].Normal.Normalize();
        }
    }
}
