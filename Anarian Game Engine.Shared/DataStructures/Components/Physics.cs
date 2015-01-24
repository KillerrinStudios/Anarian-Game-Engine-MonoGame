using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.DataStructures;
using Anarian.Enumerators;
using Anarian.Interfaces;
using Anarian.Helpers;

namespace Anarian.DataStructures.Components
{
    public class Physics : Component,
                           IUpdatable
    {
        #region Static Constants
        public static float Gravity = -9.8f;
        #endregion

        #region Fields/Properties
        float m_mass;
        public float Mass
        {
            get { return m_mass; }
            set { m_mass = value; }
        }

        float m_acceleration;
        public float Acceleration
        {
            get { return m_acceleration; }
            set { m_acceleration = value; }
        }
        #endregion

        public Physics(GameObject gameObject)
            :base(gameObject, ComponentTypes.Physics)
        {

        }

        public override void Reset()
        {
            base.Reset();
        }

        #region Interfaces
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public override void Update(GameTime gameTime)
        {

        }
    }
}
