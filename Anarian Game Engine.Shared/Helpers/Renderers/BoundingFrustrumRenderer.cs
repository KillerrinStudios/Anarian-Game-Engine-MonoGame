using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Helpers.Renderers
{
    /// <summary>
    /// Can render a given BoundingBox
    /// </summary>
    class BoundingFrustumRenderer
    {
        #region Fields

        // Vertices & Indices
        protected VertexPositionColor[] verts = new VertexPositionColor[8];
        protected Vector3[] corners = new Vector3[8];
        protected IndexBuffer indexBuffer;
        protected DynamicVertexBuffer vertexBuffer;

        protected GraphicsDevice device;
        protected BasicEffect effect;

        protected RasterizerState rasState, origRasState;
        protected BlendState origBlendState;

        protected BoundingFrustum frustum;

        #endregion

        #region Initialization

        public BoundingFrustumRenderer(BoundingFrustum frustum, GraphicsDevice graphicsdevice)
        {
            this.frustum = frustum;
            this.device = graphicsdevice;

            effect = new BasicEffect(graphicsdevice);
            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;

            rasState = new RasterizerState();
            rasState.CullMode = CullMode.None;

            #region Vertices Initialization

            for (int i = 0; i <= verts.Length - 1; i++)
            {
                verts[i] = new VertexPositionColor(Vector3.Zero, new Color(0, 200, 0, 50));
            }

            UpdateCorners();

            #endregion

            #region Indices Initialization

            ushort[] indices;

            indices = new ushort[] {
                // Near
                0,1,2,
                0,2,3,

                // Far
                6,5,4,
                7,6,4,

                // Up
                4,1,0,
                4,5,1,

                // Down
                7,3,2,
                2,6,7,

                // Left
                0,3,4,
                7,4,3,

                // Right
                5,2,1,
                5,6,2

            };

            vertexBuffer = new DynamicVertexBuffer(device, VertexPositionColor.VertexDeclaration, verts.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(verts);
            //vertexBuffer.ContentLost += new EventHandler<EventArgs>(vertexBuffer_ContentLost);

            indexBuffer = new IndexBuffer(device, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            #endregion
        }

        protected void vertexBuffer_ContentLost(object sender, EventArgs e)
        {
            vertexBuffer.SetData(verts);
        }

        #endregion

        #region Update & Draw

        public void Update()
        {
            UpdateCorners();
            vertexBuffer.SetData(verts);
        }

        protected void UpdateCorners()
        {
            frustum.GetCorners(corners);
            for (int i = 0; i <= 7; i++)
            {
                verts[i].Position = corners[i];
            }
        }

        public void Draw(ICamera camera)
        {
            effect.Projection = camera.Projection;
            effect.View = camera.View;
            effect.World = Matrix.Identity;

            // Save states before changing them
            origRasState = device.RasterizerState;
            origBlendState = device.BlendState;
            device.RasterizerState = rasState;

            // Frustum filled
            device.BlendState = BlendState.AlphaBlend;
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verts.Length, 0, 12);
            }

            // Restore states
            device.RasterizerState = origRasState;
            device.BlendState = origBlendState;
            device.SetVertexBuffer(null);
        }

        #endregion

        #region Properties

        public BoundingFrustum Frustum
        {
            get
            {
                return frustum;
            }
            set
            {
                frustum = value;
            }
        }

        #endregion
    }
}
