using Anarian.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Emission
{
    public class ContinuousParticleEmitter : IEmissionSettings
    {
        public Timer EmitterTimer;

        public ContinuousParticleEmitter(TimeSpan targetEmitTime)
        {
            EmitterTimer = new Timer(targetEmitTime);
        }

        bool IEmissionSettings.CanEmmit(GameTime gameTime, ParticleEmitter2D emitter) { return CanEmmit(gameTime, emitter); }
        public bool CanEmmit(GameTime gameTime, ParticleEmitter2D emitter)
        {
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
