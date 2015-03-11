using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Anarian;
using Anarian.DataStructures;
using Anarian.Interfaces;


namespace Anarian.Helpers
{
    public static class PrimitiveHelper3D
    {
        private static BasicEffect basicEffect = null;
        public static void Initialize(GraphicsDevice graphics)
        {
            basicEffect = new BasicEffect(graphics);
        }


        public static void DrawRay(this Ray ray, GraphicsDevice graphics, Color color, ICamera camera, Matrix WorldMatrix)
        {
            try
            {
                if (basicEffect == null) Initialize(graphics);

                // Inside your Game class
                Vector3 startPoint = ray.Position;
                Vector3 endPoint = ray.Position * (ray.Direction * camera.Far);

                // Inside your Game.LoadContent method
                basicEffect.World = WorldMatrix;
                basicEffect.View = camera.View;
                basicEffect.Projection = camera.Projection;

                // Inside your Game.Draw method
                basicEffect.CurrentTechnique.Passes[0].Apply();
                var vertices = new[] { new VertexPositionColor(startPoint, color), new VertexPositionColor(endPoint, color) };
                graphics.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
            }
            catch (Exception) { }
        }

        public static void DrawBoundingBox(this BoundingBox boundingBox, GraphicsDevice graphics, Color color, ICamera camera, Matrix WorldMatrix)
        {
            try
            {
                if (basicEffect == null) Initialize(graphics);

                // Initialize an array of indices for the box. 12 lines require 24 indices
                short[] bBoxIndices = {
                   0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                   4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                   0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
                };

                Vector3[] corners = boundingBox.GetCorners();
                VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

                // Assign the 8 box vertices
                for (int i = 0; i < corners.Length; i++)
                {
                    primitiveList[i] = new VertexPositionColor(corners[i], color);
                }

                /* Set your own effect parameters here */
                BasicEffect boxEffect = new BasicEffect(graphics);
                boxEffect.World = WorldMatrix;
                boxEffect.View = camera.View;
                boxEffect.Projection = camera.Projection;
                boxEffect.TextureEnabled = false;

                // Draw the box with a LineList
                foreach (EffectPass pass in boxEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphics.DrawUserIndexedPrimitives(
                        PrimitiveType.LineList, primitiveList, 0, 8,
                        bBoxIndices, 0, 12);
                }
            }
            catch (Exception) { }
        }

        public static void RenderBoundingSphere(this BoundingSphere sphere, GraphicsDevice graphicsDevice, Matrix world, Matrix view, Matrix projection, Color color)
        {
            try
            {
                BoundingSphereRenderer.Render(sphere, graphicsDevice, world, view, projection, color);
            }
            catch (Exception) { }
        }

    }
}
