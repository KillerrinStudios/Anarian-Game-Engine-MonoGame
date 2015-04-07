using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Anarian.DataStructures;
using Anarian.Interfaces;
using Anarian.Helpers;
using System.Diagnostics;

namespace Anarian.DataStructures.Rendering
{
    /// <summary>
    /// A sphere with a texture mapped onto it.  For best results, the texture should use a projection
    /// that spaces latitude lines equally along the y-axis.  The polar regions are mapped with
    /// a different texture.
    /// </summary>
    public class Sphere : GameObject, IUpdatable, IRenderable
    {
        // the textures used for mid- and high-latitudes
        protected Texture2D m_texture;
        public Texture2D Texture { get { return m_texture; } set { m_texture = value; } }

        protected Effect m_effect;
        public Effect Effect { get { return m_effect; } set { m_effect = value; } }

        public readonly int LONGITUDE_INTERVAL;
        public readonly int LATITUDE_INTERVAL;

        public readonly int LONGITUDE_COUNT;
        public readonly int LATITUDE_COUNT;

        // North and South hemisphere point lists (one complete circle of latitude followed by another)
        private VertexPositionNormalTexture[] m_north;
        private VertexPositionNormalTexture[] m_south;

        // indices into the north and south arrays defining one circle of triangle strips per row
        private short[][] m_northStrips;
        private short[][] m_southStrips;

        // indices into the northFan and southFan arrays defining triangle lists for the poles
        private short[] m_northIndices;
        private short[] m_southIndices;

        //// the radius of this sphere
        private float m_radius;
        //public float Radius { get { return m_radius; } protected set { if (value <= 0.0) throw new ArgumentOutOfRangeException(); m_radius = value; } }

        public BoundingSphere BoundingSphere { get { return new BoundingSphere(m_transform.WorldPosition, m_radius); } }

        /// <summary>
        /// Creates a sphere with the given textures.
        /// </summary>
        /// <param name="tex">A Texture to wrap around the sphere</param>
        /// <param name="equator">The radius of the sphere</param>
        public Sphere(GraphicsDevice graphicsDevice, Texture2D texture, int longitudeInterval = 10, int latitudeInterval = 10, float radius = 1.0f)
            :base()
        {
            m_radius = radius;
            
            if (texture == null) m_texture = Color.White.CreateTextureFromSolidColor(graphicsDevice, 1, 1);
            else m_texture = texture;
            
            // Setup the Values
            LONGITUDE_INTERVAL = longitudeInterval;
            LONGITUDE_COUNT = 360 / LONGITUDE_INTERVAL;
            
            LATITUDE_INTERVAL = latitudeInterval;
            LATITUDE_COUNT = 180 / LATITUDE_INTERVAL;

            m_north = new VertexPositionNormalTexture[LONGITUDE_COUNT * (LATITUDE_COUNT + 1)];
            m_south = new VertexPositionNormalTexture[LONGITUDE_COUNT * (LATITUDE_COUNT + 1)];

            m_northStrips = new short[LATITUDE_COUNT][];
            m_southStrips = new short[LATITUDE_COUNT][];

            m_northIndices = new short[3 * LONGITUDE_COUNT];
            m_southIndices = new short[3 * LONGITUDE_COUNT];

            // Finally, Create the Sphere
            CreateSphereModel();
            SetupEffects(graphicsDevice);
        }

        public static Sphere SphereFromBoundingSphere(GraphicsDevice graphicsDevice, BoundingSphere boundingSphere, Texture2D texture = null)
        {
            Sphere sphere = new Sphere(graphicsDevice, texture, 10, 10, boundingSphere.Radius);
            sphere.Transform.Position = boundingSphere.Center;

            return sphere;
        }

        #region SphereSetup
        /// <summary>
        /// Creates the arrays of vertices and indices of vertices.
        /// </summary>
        private void CreateSphereModel()
        {
            // North and South hemisphere point lists (one complete circle of latitude followed by another)
            m_north = new VertexPositionNormalTexture[(LONGITUDE_COUNT + 1) * (LATITUDE_COUNT + 1)];
            m_south = new VertexPositionNormalTexture[(LONGITUDE_COUNT + 1) * (LATITUDE_COUNT + 1)];

            // indices into the north and south arrays defining one circle of triangle strips per row
            m_northStrips = new short[LATITUDE_COUNT][];
            m_southStrips = new short[LATITUDE_COUNT][];

            // create the list of vertices for everything except the polar caps
            for (int latitude = 0, row = 0; latitude <= 90; latitude += LATITUDE_INTERVAL, row++)
            {
                for (int longitude = 0, col = 0; longitude <= 360; longitude += LONGITUDE_INTERVAL, col++)
                {
                    float longitudeInRadians = MathHelper.ToRadians(longitude);
                    float latitudeInRadians = MathHelper.ToRadians(latitude);

                    float y = (float)Math.Sin(latitudeInRadians);
                    float r = (float)Math.Cos(latitudeInRadians);
                    float x = (float)Math.Cos(longitudeInRadians) * r;
                    float z = (float)Math.Sin(longitudeInRadians) * r;

                    int index = row * (LONGITUDE_COUNT + 1) + col;

                    m_north[index].Position = new Vector3(x, y, z);
                    m_north[index].Normal = m_north[index].Position;

                    m_south[index].Position = new Vector3(x, -y, z);
                    m_south[index].Normal = m_south[index].Position;

                    float textureX = (360 - longitude) / 360.0f;
                    float northTextureY = 0.5f - (0.5f * latitude / 90);
                    float southTextureY = 0.5f + ((1.0f - 0.5f) * latitude / 90);
                    m_north[index].TextureCoordinate = new Vector2(textureX, northTextureY);
                    m_south[index].TextureCoordinate = new Vector2(textureX, southTextureY);
                }
            }

            // create the lists of indices into the hemisphere vertex lists for each strip
            for (int row = 0; row < LATITUDE_COUNT; row++)
            {
                m_northStrips[row] = new short[(LONGITUDE_COUNT + 1) * 2];
                m_southStrips[row] = new short[(LONGITUDE_COUNT + 1) * 2];
                for (int col = 0; col <= LONGITUDE_COUNT; col++)
                {
                    m_northStrips[row][col * 2] = (short)((row + 1) * (LONGITUDE_COUNT + 1) + col);
                    m_northStrips[row][col * 2 + 1] = (short)(row * (LONGITUDE_COUNT + 1) + col);

                    m_southStrips[row][col * 2] = (short)(row * (LONGITUDE_COUNT + 1) + col);
                    m_southStrips[row][col * 2 + 1] = (short)((row + 1) * (LONGITUDE_COUNT + 1) + col);
                }
            }

            // Now that the Sphere is made, lets make its bounds
            CreateBounds();
        }

        private void SetupEffects(GraphicsDevice graphics)
        {
            m_effect = new BasicEffect(graphics);

            if (m_texture != null)
            {
                ((BasicEffect)m_effect).TextureEnabled = true;
                ((BasicEffect)m_effect).Texture = m_texture;
            }

            SaveDefaultEffects();
        }

        public override void CreateBounds()
        {
            base.CreateBounds();
            m_boundingSpheres.Add(BoundingSphere);
        }
        public override void SaveDefaultEffects()
        {
            base.SaveDefaultEffects();
            m_defaultEffects.Add(m_effect);
        }
        public override void RestoreDefaultEffects()
        {
            base.RestoreDefaultEffects();
            m_effect = m_defaultEffects[0];
        }
        #endregion

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
            if (m_effect is BasicEffect)
            {
                ((BasicEffect)m_effect).World = m_transform.WorldMatrix;
                ((BasicEffect)m_effect).View = camera.View;
                ((BasicEffect)m_effect).Projection = camera.Projection;
            }

            // Setup User Defined Effects
            SetupEffects(m_effect, graphics, camera, gameTime);

            try
            {
                foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    for (int row = 0; row < LATITUDE_COUNT; row++)
                    {
                        graphics.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, m_north, 0, (LONGITUDE_COUNT + 1) * (LATITUDE_COUNT + 1), m_northStrips[row], 0, LONGITUDE_COUNT * 2);
                        graphics.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, m_south, 0, (LONGITUDE_COUNT + 1) * (LATITUDE_COUNT + 1), m_southStrips[row], 0, LONGITUDE_COUNT * 2);
                    }
                }

                if (m_renderBounds)
                    BoundingSphere.RenderBoundingSphere(graphics, Matrix.Identity, camera.View, camera.Projection, Color.White);
            }
            catch (Exception) { }//return false; }
            return true;
        }

        protected virtual void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            if (effect is BasicEffect)
            {
                ((BasicEffect)effect).EnableDefaultLighting();
            }
        }
        #endregion
    }
}
