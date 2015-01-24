using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Anarian.Enumerators;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;

namespace Anarian.GUI.Components
{
    public class Health2D : Component2D,
                          IUpdatable
    {
        #region Fields/Properties
        bool m_alive;
        public bool Alive
        {
            get { return m_alive; }
            set { m_alive = value; }
        }

        bool m_visible;
        internal bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        bool m_regenerateHealth;
        public bool RegenerateHealth
        {
            get { return m_regenerateHealth; }
            set { m_regenerateHealth = value; }
        }

        float m_currentHealth;
        public float CurrentHealth
        {
            get { return m_currentHealth; }
            set { m_currentHealth = value; }
        }

        float m_maxHealth;
        public float MaxHealth
        {
            get { return m_maxHealth; }
            set { m_maxHealth = value; }
        }

        float m_regenerationRate;
        public float RegenerationRate
        {
            get { return m_regenerationRate; }
            set { m_regenerationRate = value; }
        }
        #endregion

        public Health2D (GuiObject guiObject)
            : base(guiObject, ComponentTypes.Health)
        {
            m_alive = true;
            m_visible = true;

            m_maxHealth = 100.0f;
            m_currentHealth = m_maxHealth;

            m_regenerateHealth = false;
            m_regenerationRate = 0.02f;
        }
        public Health2D(GuiObject guiObject, float maxHealth, bool regenerateHealth = false, float regenerationRate = 0.0f)
            : base(guiObject, ComponentTypes.Health)
        {
            m_alive = true;
            m_visible = true;

            m_maxHealth = maxHealth;
            m_currentHealth = m_maxHealth;

            m_regenerateHealth = regenerateHealth;
            m_regenerationRate = regenerationRate;
        }
        public override void Reset()
        {
            base.Reset();

            m_alive = true;
            m_visible = true;
            m_currentHealth = m_maxHealth;
        }

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        #region Helper Methods
        public void IncreaseHealth(float amount, bool allowPastMax = false)
        {
            m_currentHealth += amount;

            if (!allowPastMax &&
                m_currentHealth >= m_maxHealth) {
                m_currentHealth = m_maxHealth;
            }
        }

        public void DecreaseHealth(float amount)
        {
            m_currentHealth -= amount;

            if (m_currentHealth <= 0.0f) {
                m_currentHealth = 0.0f;
                m_alive = false;
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;
            if (!m_alive) return;
            if (!m_regenerateHealth) return;

            IncreaseHealth((float)(m_regenerationRate * gameTime.ElapsedGameTime.TotalMilliseconds));
        }

        public override string ToString()
        {
            return base.ToString() +
                   m_currentHealth + "\\" + m_maxHealth;
        }
    }
}
