using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Anarian;
using Anarian.DataStructures;

namespace Anarian.Helpers
{
    public static class HelperUtils
    {
        public static Vector2 GetViewportCenter(this Viewport viewport)
        {
            return new Vector2(viewport.X + viewport.Width / 2, viewport.Y + viewport.Height / 2);
        }

        public static Rectangle GetViewportRectangle(this Viewport viewport)
        {
            return new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        public static Texture2D CreateTextureFromSolidColor(this Color color, GraphicsDevice graphicsDevice, int width, int height)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);

            Color[] colors = new Color[width * height];
            for (int i = 0; i < width * height; i++) { colors[i] = color; }
            texture.SetData(colors);

            return texture;
        }

        public static float DeltaTime (this GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public static BoundingBox GenerateBoundingBox(this ModelMesh modelMesh, Matrix meshTransform)
        {
            // Create initial variables to hold min and max xyz values for the mesh
            Vector3 meshMax = new Vector3(float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue);

            foreach (ModelMeshPart part in modelMesh.MeshParts) {
                // The stride is how big, in bytes, one vertex is in the vertex buffer
                // We have to use this as we do not know the make up of the vertex
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, stride);

                // Find minimum and maximum xyz values for this mesh part
                Vector3 vertPosition = new Vector3();

                for (int i = 0; i < vertexData.Length; i++) {
                    vertPosition = vertexData[i].Position;

                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }

            }

            // transform by mesh bone matrix
            meshMin = Vector3.Transform(meshMin, meshTransform);
            meshMax = Vector3.Transform(meshMax, meshTransform);

            // Create the bounding box
            return new BoundingBox(meshMin, meshMax);;
        }
    }
}
