﻿using Anarian.Enumerators;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.DataStructures
{
    public class Timer : AnarianObject, IGameComponent, IUpdatable
    {
        #region Fields/Properties
        ProgressStatus m_progress;
        public ProgressStatus Progress { get { return m_progress; } protected set { m_progress = value; } }

        PausedState m_paused;
        public PausedState Paused { get { return m_paused; } set { m_paused = value; } }

        TimeSpan m_currentTick;
        TimeSpan m_interval;
        public TimeSpan CurrentTick { get { return m_currentTick; } protected set { m_currentTick = value; } }
        public TimeSpan Interval { get { return m_interval; } set { m_interval = value; } }
        public TimeSpan TimeRemaining { get { return m_interval - m_currentTick; } 
        }
        #endregion

        public Timer(TimeSpan interval)
            :base()
        {
            m_interval = interval;
            Reset();
        }

        public void Reset()
        {
            m_progress = ProgressStatus.NotStarted;
            m_paused = PausedState.Unpaused;
            m_currentTick = TimeSpan.Zero;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IGameComponent.Initialize() { Reset(); }
        #endregion

        #region Helper Methods
        #endregion

        public virtual void Update(GameTime gameTime)
        {
            if (m_progress == ProgressStatus.Completed) return;
            if (m_paused == PausedState.Paused) return;

            m_currentTick += gameTime.ElapsedGameTime;

            if (m_currentTick < m_interval)
            {
                m_progress = ProgressStatus.InProgress;

                if (Tick != null)
                    Tick(this, null);
            }
            else {
                m_progress = ProgressStatus.Completed;
                m_currentTick = m_interval;

                if (Completed != null)
                    Completed(this, null);
            }
        }

        public event EventHandler Tick;
        public event EventHandler Completed;
    }
}
