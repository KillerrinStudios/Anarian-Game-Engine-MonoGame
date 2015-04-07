using Anarian.Enumerators;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.DataStructures
{
    public class ContinuousClock : AnarianObject, IGameComponent, IUpdatable
    {
        #region Fields/Properties
        PausedState m_paused;
        public PausedState Paused { get { return m_paused; } set { m_paused = value; } }

        TimeSpan m_currentTime;
        public TimeSpan CurrentTime { get { return m_currentTime; } protected set { m_currentTime = value; } }

        private TimeSpan m_nextReport;
        public TimeSpan NextReport { get { return m_nextReport; } protected set { m_nextReport = value; } }
        public double ReportEveryXSeconds;
        #endregion

        public ContinuousClock(double reportEveryXSeconds)
            :base()
        {
            ReportEveryXSeconds = reportEveryXSeconds;

            Reset();
        }

        public void Reset()
        {
            m_paused = PausedState.Unpaused;
            m_currentTime = TimeSpan.Zero;
            m_nextReport = m_currentTime + TimeSpan.FromSeconds(ReportEveryXSeconds);
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IGameComponent.Initialize() { Reset(); }
        #endregion


        public virtual void Update(GameTime gameTime)
        {
            if (m_paused == PausedState.Paused) return;

            m_currentTime += gameTime.ElapsedGameTime;

            if (m_currentTime >= m_nextReport)
            {
                m_nextReport = m_currentTime + TimeSpan.FromSeconds(ReportEveryXSeconds);
                
                if (ReportTimePassed != null)
                    ReportTimePassed(this, null);
            }
        }

        public event EventHandler ReportTimePassed;
    }
}
