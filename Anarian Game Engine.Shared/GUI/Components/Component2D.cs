using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Anarian.Enumerators;
using Microsoft.Xna.Framework;
using Anarian.Interfaces;
using Anarian.DataStructures;

namespace Anarian.GUI.Components
{
    public class Component2D : AnarianObject, IUpdatable
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

        protected GuiObject m_guiObject;
        public GuiObject GuiObject
        {
            get { return m_guiObject; }
            internal set { m_guiObject = value; }
        }
        #endregion

        public Component2D(GuiObject guiObject)
            :base()
        {
            m_componentType = ComponentTypes.None;
           
            m_active = true;
            m_guiObject = guiObject;
        }
        public Component2D(GuiObject guiObject, ComponentTypes componentType)
            :base(componentType.ToString())
        {
            m_componentType = componentType;

            m_active = true;
            m_guiObject = guiObject;
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
            return m_guiObject.Name + "|" + m_name + ": ";
        }
    }
}
