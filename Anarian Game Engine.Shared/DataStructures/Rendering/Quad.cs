using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.DataStructures.Rendering
{
    public class Quad : GameObject, IUpdatable, IRenderable
    {
        protected Texture2D m_texture;
        public Texture2D Texture { get { return m_texture; } set { m_texture = value; } }

        protected BasicEffect m_effect;
        public BasicEffect Effect { get { return m_effect; } set { m_effect = value; } }


        private VertexPositionNormalTexture[] m_vertices;
        private short[] m_indices;

        public bool TwoSided { get; private set; }

        public Quad(GraphicsDevice graphicsDevice, Texture2D texture, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, bool twosided = false)
            : base()
        {
            TwoSided = twosided;

            m_vertices = new VertexPositionNormalTexture[4];
            m_vertices[0].Position = p1;
            m_vertices[0].TextureCoordinate = new Vector2(1.0f, 1.0f);

            m_vertices[1].Position = p2;
            m_vertices[1].TextureCoordinate = new Vector2(0.0f, 1.0f);

            m_vertices[2].Position = p3;
            m_vertices[2].TextureCoordinate = new Vector2(0.0f, 0.0f);

            m_vertices[3].Position = p4;
            m_vertices[3].TextureCoordinate = new Vector2(1.0f, 0.0f);

            if (TwoSided)   m_indices = new short[12];
            else            m_indices = new short[6];

            m_indices[0] = 0;   m_indices[1] = 1;   m_indices[2] = 2;
            m_indices[3] = 0;   m_indices[4] = 2;   m_indices[5] = 3;

            if (TwoSided)
            {
                m_indices[6] = 0;       m_indices[7] = 2;       m_indices[8] = 1;
                m_indices[9] = 0;       m_indices[10] = 3;      m_indices[11] = 2;
            }

            CalculateNormals();
            SetupEffects(graphicsDevice);
            CreateBounds();
        }

        private void CalculateNormals()
        {
            for (int i = 0; i < m_vertices.Length; i++)
                m_vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < m_indices.Length / 3; i++)
            {
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

        public override void CreateBounds()
        {
            base.CreateBounds();
        
            List<Vector3> m_points = new List<Vector3>();
            foreach (var p in m_vertices)
	        {
	            m_points.Add(p.Position);	 
	        }

            var bs = BoundingSphere.CreateFromPoints(m_points);
            m_boundingSpheres.Add(bs);
        }

        private void SetupEffects(GraphicsDevice graphics)
        {
            m_effect = new BasicEffect(graphics);

            if (m_texture != null) {
                m_effect.TextureEnabled = true;
                m_effect.Texture = m_texture;
            }
        }

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;

            // We first Update the Children
            base.Update(gameTime);
        }

        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            // We Draw the base here so that the Children get taken care of
            // We grab the result so we can know if it was visible or not
            var result = base.Draw(gameTime, spriteBatch, graphics, camera);
            if (!result) return false;

            //// Check Against Frustrum to cull out objects
            //if (m_cullDraw)
            //{
            //    bool collided = false;
            //    for (int i = 0; i < m_boundingSpheres.Count; i++)
            //    {
            //        if (camera.Frustum.Intersects(m_boundingSpheres[i])) { collided = true; break; }
            //    }
            //
            //    if (!collided) return false;
            //}

            // Prep the Graphics Device
            graphics.RasterizerState.CullMode = CullMode.None;

            // Begin Drawing the World
            // Since the world will be generated outwards from its side, we are offsetting the orgin of the world to its center
            m_effect.World = m_transform.WorldMatrix;
            m_effect.View = camera.View;
            m_effect.Projection = camera.Projection;

            // Setup the Custom Effects
            SetupEffects(m_effect, graphics, camera, gameTime);

            try
            {
                foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphics.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        m_vertices, 0, m_vertices.Length,
                        m_indices, 0, m_indices.Length / 3,
                        VertexPositionNormalTexture.VertexDeclaration);
                }

                //m_boundingBox.DrawBoundingBox(graphics, Color.Red, camera, Matrix.Identity);
            }
            catch (Exception) { return false; }
            return true;
        }

        protected virtual void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            ((BasicEffect)effect).EnableDefaultLighting();
        }
        #endregion
    }
}
