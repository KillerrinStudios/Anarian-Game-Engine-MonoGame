using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Anarian.GUI.Components;
using Anarian.Interfaces;
using Anarian.DataStructures;
using Anarian.Events;
using Anarian.Enumerators;

namespace Anarian.GUI
{
    public class Menu : AnarianObject,
                        IScene2D, IUpdatable, IRenderable
    {
        Transform2D m_sceneNode;
        public Transform2D SceneNode
        {
            get { return m_sceneNode; }
            protected set { m_sceneNode = value; }
        }

        NavigationSaveState m_navigationSaveState;
        public NavigationSaveState NavigationSaveState
        {
            get { return m_navigationSaveState; }
            set { m_navigationSaveState = value; }
        }

        public event EventHandler OnLoad;

        public Menu()
            :base()
        {
            m_navigationSaveState = NavigationSaveState.KeepSate;

            // When Creating the Base SceneNode, we will set
            // its Scale to Zero so that SceneNodes which get
            // added as children aren't screwed up
            GuiObject node = new GuiObject();
            node.Transform.Scale = Vector2.Zero;
            m_sceneNode = node.Transform;
        }

        public virtual void MenuLoaded()
        {
            if (m_navigationSaveState == NavigationSaveState.RecreateState) {
                GuiObject node = new GuiObject();
                node.Transform.Scale = Vector2.Zero;
                m_sceneNode = node.Transform;
            }

            if (OnLoad != null)
                OnLoad(this, null);
        }

        #region Interface Implimentation
        #region IScene2D
        Transform2D IScene2D.SceneNode
        {
            get { return SceneNode; }
            set { }
        }
        void IScene2D.HandlePointerDown(object sender, PointerPressedEventArgs e) { HandlePointerDown(sender, e); }
        void IScene2D.HandlePointerPressed(object sender, PointerPressedEventArgs e) { HandlePointerPressed(sender, e); }
        void IScene2D.HandlePointerMoved(object sender, PointerMovedEventArgs e) { HandlePointerMoved(sender, e); }
        #endregion

        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics); }
        #endregion

        public virtual void Update(GameTime gameTime)
        {
            m_sceneNode.GuiObject.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            m_sceneNode.GuiObject.Draw(gameTime, spriteBatch, graphics);
        }

        #region HandleEvents
        internal void HandlePointerDown(object sender, PointerPressedEventArgs e)
        {
        }

        internal void HandlePointerPressed(object sender, PointerPressedEventArgs e)
        {
        }

        internal void HandlePointerMoved(object sender, PointerMovedEventArgs e)
        {
        }
        #endregion
    }
}
