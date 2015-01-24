using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Anarian.DataStructures;
using Anarian.Interfaces;
using Anarian.GUI.Components;

namespace Anarian.GUI
{
    public class GuiObject : AnarianObject,
                             IUpdatable, IRenderable
    {
        #region Fields/Properties
        protected bool m_active;
        protected bool m_visible;

        protected List<Component2D> m_components;

        protected Transform2D m_transform;


        public bool Active
        {
            get { return m_active; }
            set { m_active = value; }
        }
        public bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        public Transform2D Transform
        {
            get { return m_transform; }
            protected set { m_transform = value; }
        }

        public List<Component2D> Components
        {
            get { return m_components; }
            protected set { m_components = value; }
        }
        #endregion

        public GuiObject()
            :base()
        {
            // Setup Defaults
            m_active = true;
            m_visible = true;

            // Setup the Transform
            m_transform = new Transform2D(this);

            // Setup the other Components
            m_components = new List<Component2D>();
        }


        #region Component Management
        /// <summary>
        /// Adds a Default Component of Type
        /// </summary>
        /// <param name="type">The Type of Component we are going to add</param>
        /// <returns>The newly created Component, Null if can't be created</returns>
        public Component2D AddComponent(Type type)
        {
            Component2D component;
            if (type == typeof(Health2D)) { component = new Health2D(this); }
            else if (type == typeof(Mana2D)) { component = new Mana2D(this); }
            else if (type == typeof(Transform2D)) { component = new Transform2D(this); }
            else { component = null; }

            if (component != null) {
                m_components.Add(component);
            }
            return component;
        }

        /// <summary>
        /// Adds a Component to the Component List
        /// </summary>
        /// <param name="component">The Component we want to add</param>
        public void AddComponent(Component2D component)
        {
            // Associate the component with this
            component.GuiObject = this;
            m_components.Add(component);
        }

        /// <summary>
        /// Gets the first available Component of specified Type
        /// </summary>
        /// <param name="type">The Type we are looking for</param>
        /// <returns>The First Available Component of Type, Null if no component is found</returns>
        public Component2D GetComponent(Type type)
        {
            for (int i = 0; i < m_components.Count; i++) {
                Type compType = m_components[i].GetType();
                if (compType == type)
                    return m_components[i];
            }
            return null;
        }

        /// <summary>
        /// Gets a List of Component of specified Type
        /// </summary>
        /// <param name="type">The Type we are looking for</param>
        /// <returns>A List of Components matching the type</returns>
        public List<Component2D> GetComponents(Type type)
        {
            List<Component2D> componentList = new List<Component2D>();
            for (int i = 0; i < m_components.Count; i++) {
                Type compType = m_components[i].GetType();
                if (compType == type) {
                    componentList.Add(m_components[i]);
                }
            }
            return componentList;
        }

        /// <summary>
        /// Removes the specified Component
        /// </summary>
        /// <param name="component">The Component we wish to remove</param>
        public void RemoveComponent(Component2D component)
        {
            for (int i = 0; i < m_components.Count; i++) {
                if (component == m_components[i]) {
                    m_components.RemoveAt(i);
                    return;
                }
            }
        }
        #endregion

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics); }
        #endregion

        public virtual void Update(GameTime gameTime)
        {
            if (!m_active) return;

            // Update the Children
            foreach (var child in m_transform.GetChildren()) {
                child.GuiObject.Update(gameTime);
            }

            // Update every other Component
            foreach (var component in m_components) {
                component.Update(gameTime);
            }

            // Finally, Update the Transform
            m_transform.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (!m_active) return;
            if (!m_visible) return;
            
            // Update the Children
            foreach (var child in m_transform.GetChildren()) {
                child.GuiObject.Draw(gameTime, spriteBatch, graphics);
            }

            // Draw Each Component
            foreach (var component in m_components) {
                if (component is IRenderable) {
                    ((IRenderable)component).Draw(gameTime, spriteBatch, graphics, null);
                }
            }
        }

    }
}
