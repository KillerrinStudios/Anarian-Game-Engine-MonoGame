using Anarian.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Emission
{
    public class ContinuousParticleEmitter : IEmissionSettings
    {
        public bool Active;
        public Timer EmitterTimer;

        public ContinuousParticleEmitter(TimeSpan targetEmitTime)
        {
            Active = true;
            EmitterTimer = new Timer(targetEmitTime);
        }

        bool IEmissionSettings.Active { get { return Active; } set { Active = value; } }
        bool IEmissionSettings.CanEmmit(GameTime gameTime, ParticleEmitter2D emitter) { return CanEmmit(gameTime, emitter); }
        public bool CanEmmit(GameTime gameTime, ParticleEmitter2D emitter)
        {
            if (!Active) return false;

            EmitterTimer.Update(gameTime);
            if (EmitterTimer.Progress == Enumerators.ProgressStatus.Completed)
            {
                EmitterTimer.Reset();
                return true;
            }

            return false;
        }
    }
}
