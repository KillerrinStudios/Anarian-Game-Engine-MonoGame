using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.DataStructures
{
    public class AnarianObject
    {
        #region Fields/Properties
        internal string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        internal string m_tag;
        public string Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        internal protected object m_lockObject;
        protected object LockObject { get { return m_lockObject; } set { m_lockObject = value; } }
        #endregion

        public AnarianObject(string name = "", string tag = "")
        {
            m_name = name;
            m_tag = tag;
            m_lockObject = new object();
        }
    }
}
