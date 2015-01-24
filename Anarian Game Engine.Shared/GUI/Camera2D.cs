using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Anarian.GUI
{
    public class Camera2D : IUpdatable
    {
        #region Fields/Properties
        Vector2 m_position;
        public Vector2 Position
        {
            get { return m_position; }
            set { 
                m_position = value;
                CreateTransformMatrix(new Vector3(-m_position.X, -m_position.Y, 0),
                                      m_rotation,
                                      new Vector3(m_zoom, m_zoom, 1),
                                      new Vector3(m_origin.X, m_origin.Y, 0)
                                      );
            }
        }

        Vector2 m_origin;
        public Vector2 Origin
        {
            get { return m_origin; }
            set { 
                m_origin = value;
                CreateTransformMatrix(new Vector3(-m_position.X, -m_position.Y, 0),
                                      m_rotation,
                                      new Vector3(m_zoom, m_zoom, 1),
                                      new Vector3(m_origin.X, m_origin.Y, 0)
                                      );
            }
        }

        float m_zoom;
        public float Zoom
        {
            get { return m_zoom; }
            set {
                m_zoom = value; 
                if (m_zoom < 0.1f) m_zoom = 0.1f;  // Negative zoom will flip image

                CreateTransformMatrix(new Vector3(-m_position.X, -m_position.Y, 0),
                                      m_rotation,
                                      new Vector3(m_zoom, m_zoom, 1),
                                      new Vector3(m_origin.X, m_origin.Y, 0)
                                      );
            } 
        }

        float m_rotation;
        public float Rotation
        {
            get { return m_rotation; }
            set { 
                m_rotation = value;
                CreateTransformMatrix(new Vector3(-m_position.X, -m_position.Y, 0),
                                      m_rotation,
                                      new Vector3(m_zoom, m_zoom, 1),
                                      new Vector3(m_origin.X, m_origin.Y, 0)
                                      );
            }
        }

        Matrix m_transform;
        public Matrix Transform
        {
            get { return m_transform; }
            set { m_transform = value; }
        }
        #endregion

        public Camera2D(GraphicsDevice graphicsDevice)
        {
            m_position = Vector2.Zero;
            m_rotation = 0.0f;
            m_zoom = 1.0f;
            m_origin = new Vector2(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f);

            CreateTransformMatrix(new Vector3(-m_position.X, -m_position.Y, 0),
                                  m_rotation,
                                  new Vector3(m_zoom, m_zoom, 1),
                                  new Vector3(m_origin.X, m_origin.Y, 0));
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        protected void CreateTransformMatrix(Vector3 position, float rotation, Vector3 scale, Vector3 origin)
        {
            m_transform = Matrix.CreateTranslation(position) *
                          Matrix.CreateRotationZ(rotation) *
                          Matrix.CreateScale(scale) *
                          Matrix.CreateTranslation(origin);
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            Position += amount;
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
