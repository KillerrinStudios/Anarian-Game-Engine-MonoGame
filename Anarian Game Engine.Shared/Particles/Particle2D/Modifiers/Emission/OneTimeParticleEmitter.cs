using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Emission
{
    public class OneTimeParticleEmitter : IEmissionSettings
    {
        public bool Active;
        public bool HasEmitted;

        public OneTimeParticleEmitter()
        {
            Active = true;
            HasEmitted = false;
        }


        bool IEmissionSettings.Active { get { return Active; } set { Active = value; } }
        bool IEmissionSettings.CanEmmit(GameTime gameTime, ParticleEmitter2D emitter) { return CanEmmit(gameTime, emitter); }
        public bool CanEmmit(GameTime gameTime, ParticleEmitter2D emitter)
        {
            if (!Active) return false;
            if (HasEmitted) return false;

            HasEmitted = true;
            return true;
        }
    }
}
