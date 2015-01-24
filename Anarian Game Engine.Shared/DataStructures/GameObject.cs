using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using Anarian.Helpers;

namespace Anarian.DataStructures
{
    public class GameObject : AnarianObject, IUpdatable, IRenderable
    {
        #region Fields/Properties
        protected bool    m_active;
        protected bool    m_visible;
        protected bool    m_cullDraw;
        protected bool    m_renderBounds;

        protected List<BoundingBox> m_boundingBoxes;
        protected List<Component> m_components;

        protected Transform m_transform;


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
        public bool CullDraw
        {
            get { return m_cullDraw; }
            set { m_cullDraw = value; }
        }
        public bool RenderBounds
        {
            get { return m_renderBounds; }
            set { m_renderBounds = value; }
        }


        public Transform Transform
        {
            get { return m_transform; }
            protected set { m_transform = value; }
        }

        public List<Component> Components
        {
            get { return m_components; }
            protected set { m_components = value; }
        }
        #endregion

        public GameObject()
            :base()
        {
            // Setup Defaults
            m_active        = true;
            m_visible       = true;
            m_cullDraw      = true;
            m_renderBounds  = false;

            // Setup the Transform
            m_transform = new Transform(this);

            // Setup Bounding Boxes
            m_boundingBoxes = new List<BoundingBox>();

            // Setup the other Components
            m_components = new List<Component>();
        }

        #region Component Management
        /// <summary>
        /// Adds a Default Component of Type
        /// </summary>
        /// <param name="type">The Type of Component we are going to add</param>
        /// <returns>The newly created Component, Null if can't be created</returns>
        public Component AddComponent(Type type)
        {
            Component component;
            if (type == typeof(Health)) { component = new Health(this); }
            else if (type == typeof(Mana)) { component = new Mana(this); }
            else if (type == typeof(Transform)) { component = new Transform(this); }
            else if (type == typeof(Physics)) { component = new Physics(this); }
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
        public void AddComponent(Component component) 
        {
            // Associate the component with this
            component.GameObject = this; 
            m_components.Add(component); 
        }

        /// <summary>
        /// Gets the first available Component of specified Type
        /// </summary>
        /// <param name="type">The Type we are looking for</param>
        /// <returns>The First Available Component of Type, Null if no component is found</returns>
        public Component GetComponent(Type type)
        {
            for (int i = 0; i < m_components.Count; i++ ) {
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
        public List<Component> GetComponents(Type type)
        {
            List<Component> componentList = new List<Component>();
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
        public void RemoveComponent(Component component)
        {
            for (int i = 0; i < m_components.Count; i++) {
                if (component == m_components[i]) {
                    m_components.RemoveAt(i);
                    return;
                }
            }
        }
        #endregion

        public virtual bool CheckRayIntersection(Ray ray)
        {
            foreach (var bound in m_boundingBoxes) {
                float? result = ray.Intersects(bound);
                if (result.HasValue) return true;
            }
            return false;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        #region Update/Draw
        public virtual void Update(GameTime gameTime)
        {
            if (!m_active) return;

            // Update the Children
            foreach (var child in m_transform.GetChildren()) {
                child.GameObject.Update(gameTime);
            }

            // Update every other Component
            foreach (var component in m_components) {
                component.Update(gameTime);
            }

            // Finally, Update the Transform
            m_transform.Update(gameTime);
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
        {
            if (!m_active) return;
            if (!m_visible) return;

            // Render the Children
            foreach (var child in m_transform.GetChildren()) {
                if (child != null) child.GameObject.Draw(gameTime, null, graphics, camera);
            }

            // Draw Each Component
            foreach (var component in m_components) {
                if (component is IRenderable) {
                    ((IRenderable)component).Draw(gameTime, null, graphics, camera);
                }
            }

            // Begin Setting up the GameObject for Rendering in the inherited classes
            //Debug.WriteLine("Rendering Model Pos:{0}, Sca:{1}, Rot:{2}", WorldPosition, WorldScale, WorldRotation);

            // Since we are also using 2D, Reset the
            // Graphics Device to Render 3D Models properly
            GraphicsDevice graphicsDevice = graphics;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
        #endregion
    }
}
