using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Modifiers
{
    public class OpacityLifespanParticleModifier : IParticleModifier
    {
        public OpacityLifespanParticleModifier()
        {
        }

        void IParticleModifier.ApplyModifier(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle) { ApplyModifier(gameTime, emitter, particle); }
        public void ApplyModifier(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle)
        {
            float normalizedLifetime = emitter.ParticleLifespan.GetNormalizedLifespan(gameTime, emitter, particle);
            float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
            Color color = particle.InitialColor * alpha;

            particle.Colour = color;
        }
    }
}
