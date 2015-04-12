using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Anarian.Enumerators;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;

namespace Anarian.DataStructures.Components
{
    public class Health : Component,
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

        bool m_invincible;
        public bool Invincible { get { return m_invincible; } set { m_invincible = value; } }

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

        /// <summary>
        /// The ammount of time which needs to pass before health can be regenerated
        /// </summary>
        public Timer RegenerationTimer;
        #endregion

        public Health (GameObject gameObject)
            :base(gameObject, ComponentTypes.Health)
        {
            m_alive = true;
            m_visible = true;
            m_invincible = false;

            m_maxHealth = 100.0f;
            m_currentHealth = m_maxHealth;

            m_regenerateHealth = false;
            m_regenerationRate = 0.02f;

            RegenerationTimer = new Timer(TimeSpan.FromSeconds(1.0));
        }
        public Health(GameObject gameObject, float maxHealth, bool regenerateHealth = false, float regenerationRate = 0.0f)
            : base(gameObject, ComponentTypes.Health)
        {
            m_alive = true;
            m_visible = true;

            m_maxHealth = maxHealth;
            m_currentHealth = m_maxHealth;

            m_regenerateHealth = regenerateHealth;
            m_regenerationRate = regenerationRate;

            RegenerationTimer = new Timer(TimeSpan.FromSeconds(1.0));
        }
        public override void Reset()
        {
            base.Reset();

            m_alive = true;
            m_visible = true;
            m_currentHealth = m_maxHealth;

            RegenerationTimer.Reset();
        }

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        #region Helper Methods
        public void IncreaseHealth(float amount, bool allowPastMax = false)
        {
            //Debug.WriteLine("Increase Health");
            m_currentHealth += amount;

            if (!allowPastMax &&
                m_currentHealth >= m_maxHealth) 
            {
                m_currentHealth = m_maxHealth;
            }

            if (m_currentHealth > 0.0f)
                m_alive = true;
        }

        public void DecreaseHealth(float amount)
        {
            if (m_invincible) return;

            m_currentHealth -= amount;

            if (m_currentHealth <= 0.0f) {
                m_currentHealth = 0.0f;
                m_alive = false;
            }

            RegenerationTimer.Reset();
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;

            if (m_invincible)
            {
                m_currentHealth = m_maxHealth;
                return;
            }
            
            if (!m_alive) return;

            if (m_regenerateHealth)
            {
                RegenerationTimer.Update(gameTime);
                if (RegenerationTimer.Progress == ProgressStatus.Completed)
                    IncreaseHealth((float)(m_regenerationRate * gameTime.ElapsedGameTime.TotalMilliseconds));
            }

            return;
        }

        public override string ToString()
        {
            return base.ToString() +
                   m_currentHealth + "\\" + m_maxHealth;
        }
    }
}
