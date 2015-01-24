using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Anarian.Enumerators;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;

namespace Anarian.GUI.Components
{
    public class Mana2D : Component2D,
                        IUpdatable
    {
        #region Fields/Properties
        bool m_regenerateMana;
        public bool RegenerateMana
        {
            get { return m_regenerateMana; }
            set { m_regenerateMana = value; }
        }

        float m_currentMana;
        public float CurrentMana
        {
            get { return m_currentMana; }
            set { m_currentMana = value; }
        }

        float m_maxMana;
        public float MaxMana
        {
            get { return m_maxMana; }
            set { m_maxMana = value; }
        }

        float m_regenerationRate;
        public float RegerationRate
        {
            get { return m_regenerationRate; }
            set { m_regenerationRate = value; }
        }

        #endregion

        public Mana2D(GuiObject guiObject)
            : base(guiObject, ComponentTypes.Mana)
        {
            m_maxMana = 100.0f;
            m_currentMana = m_maxMana;

            m_regenerateMana = false;
            m_regenerationRate = 0.02f;
        }
        public Mana2D(GuiObject guiObject, float maxMana, bool regenerateMana = false, float regenerationRate = 0.0f)
            : base(guiObject, ComponentTypes.Mana)
        {
            m_maxMana = maxMana;
            m_currentMana = m_maxMana;

            m_regenerateMana = regenerateMana;
            m_regenerationRate = regenerationRate;
        }
        public override void Reset()
        {
            base.Reset();

            m_currentMana = m_maxMana;
        }

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        #region Helper Methods
        public void IncreaseMana(float amount, bool allowPastMax = false)
        {
            m_currentMana += amount;

            if (!allowPastMax)
                m_currentMana = m_maxMana;
        }

        public void DecreaseMana(float amount)
        {
            m_currentMana -= amount;

            if (m_currentMana < 0.0f)
                m_currentMana = 0.0f;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;
            if (!m_regenerateMana) return;

            IncreaseMana((float)(m_regenerationRate * gameTime.ElapsedGameTime.TotalMilliseconds));
        }
    }
}
