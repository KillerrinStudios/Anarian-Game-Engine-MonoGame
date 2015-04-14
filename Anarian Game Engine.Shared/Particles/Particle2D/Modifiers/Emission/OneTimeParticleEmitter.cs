using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Emission
{
    public class OneTimeParticleEmitter : IEmissionSettings
    {
        public bool HasEmitted;

        public OneTimeParticleEmitter()
        {
            HasEmitted = false;
        }

        bool IEmissionSettings.CanEmmit(GameTime gameTime, ParticleEmitter2D emitter) { return CanEmmit(gameTime, emitter); }
        public bool CanEmmit(GameTime gameTime, ParticleEmitter2D emitter)
        {
            if (HasEmitted)
                return false;

            HasEmitted = true;
            return true;
        }
    }
}
