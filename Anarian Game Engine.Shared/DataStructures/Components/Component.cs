using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Anarian.Enumerators;
using Microsoft.Xna.Framework;
using Anarian.Interfaces;

namespace Anarian.DataStructures.Components
{
    public class Component : AnarianObject, IUpdatable
    {
        #region Fields/Properties
        protected ComponentTypes m_componentType;
        public ComponentTypes ComponentType
        {
            get { return m_componentType; }
            protected set { m_componentType = value; }
        }

        protected bool m_active;
        public bool Active
        {
            get { return m_active; }
            set { m_active = value; }
        }

        protected GameObject m_gameObject;
        public GameObject GameObject
        {
            get { return m_gameObject; }
            internal set { m_gameObject = value; }
        }
        #endregion

        public Component(GameObject gameObject)
            :base()
        {
            m_componentType = ComponentTypes.None;
           
            m_active = true;
            m_gameObject = gameObject;
        }

        public Component(GameObject gameObject, ComponentTypes componentType)
            :base(componentType.ToString())
        {
            m_componentType = componentType;

            m_active = true;
            m_gameObject = gameObject;
        }

        public Component(GameObject gameObject, Component o, ComponentTypes componentType)
            : base(componentType.ToString())
        {
            m_componentType = componentType;

            m_active = o.m_active;
            m_gameObject = gameObject;
        }

        public virtual void Reset()
        {
            m_active = true;
        }

        #region Interfaces
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public virtual void Update(GameTime gameTime) { }

        public override string ToString()
        {
            return m_gameObject.Name + "|" + m_name + ": ";
        }
    }
}
