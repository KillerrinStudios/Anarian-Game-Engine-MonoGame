using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Anarian.Interfaces;
using Anarian.DataStructures.Components;
using Microsoft.Xna.Framework.Graphics;
using Anarian.Enumerators;

namespace Anarian.DataStructures
{
    public class Level : AnarianObject,
                         IScene, IUpdatable, IRenderable
    {
        private NavigationSaveState m_navigationSaveState;
        private Camera m_camera;
        private Transform m_sceneNode;

        public NavigationSaveState NavigationSaveState
        {
            get { return m_navigationSaveState; }
            set { m_navigationSaveState = value; }
        }

        public Camera Camera
        {
            get { return m_camera; }
            set { m_camera = value; }
        }
        public Transform SceneNode
        {
            get { return m_sceneNode; }
            protected set { m_sceneNode = value; }
        }

        public event EventHandler OnLoad;

        public Level(GraphicsDevice graphics)
            :base()
        {
            m_navigationSaveState = NavigationSaveState.KeepSate;

            // Create the Camera using the Graphics Device
            m_camera = new Camera();
            m_camera.AspectRatio = graphics.Viewport.AspectRatio;

            // When Creating the Base SceneNode, we will set
            // its Scale to Zero so that SceneNodes which get
            // added as children aren't screwed up
            GameObject node = new GameObject();
            node.Transform.Scale = Vector3.Zero;
            m_sceneNode = node.Transform;
        }

        public virtual void LevelLoaded()
        {
            if (m_navigationSaveState == NavigationSaveState.RecreateState) {
                // Recreate Camera
                float tempAspectRatio = m_camera.AspectRatio;
                m_camera = new Camera();
                m_camera.AspectRatio = tempAspectRatio;

                // Recreate Node
                GameObject node = new GameObject();
                node.Transform.Scale = Vector3.Zero;
                m_sceneNode = node.Transform;
            }

            if (OnLoad != null)
                OnLoad(this, null);
        }

        #region Interface Implimentation
        Camera IScene.Camera
        {
            get { return Camera; }
            set { Camera = value; }
        }

        Transform IScene.SceneNode
        {
            get { return SceneNode; }
            set { }
        }
        
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }

        /// <summary>
        /// Draws the Level
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        /// <param name="spriteBatch">The SpriteBatch</param>
        /// <param name="graphics">The GraphicsDevice</param>
        /// <param name="camera">Set Camera to null to use Main Camera</param>
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        /// <summary>
        /// Updates the Level
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public virtual void Update(GameTime gameTime)
        {
            m_sceneNode.GameObject.Update(gameTime);
        }

        /// <summary>
        /// Draws the Level
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        /// <param name="spriteBatch">The SpriteBatch</param>
        /// <param name="graphics">The GraphicsDevice</param>
        /// <param name="camera">Set to null to use Camera attached to Level</param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera = null)
        {
            if (camera == null)
                m_sceneNode.GameObject.Draw(gameTime, spriteBatch, graphics, m_camera);
            else
                m_sceneNode.GameObject.Draw(gameTime, spriteBatch, graphics, camera);
        }
    }
}
