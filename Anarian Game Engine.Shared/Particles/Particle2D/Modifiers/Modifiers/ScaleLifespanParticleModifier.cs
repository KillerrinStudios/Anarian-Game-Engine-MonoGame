using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Modifiers
{
    public class ScaleLifespanParticleModifier : IParticleModifier
    {
        public float StartScale;
        public float EndScale;

        public ScaleLifespanParticleModifier(float startScale = 0.75f, float endScale = 0.25f)
        {
            StartScale = startScale;
            EndScale = endScale;
        }
        public static ScaleLifespanParticleModifier ZeroScale() { return new ScaleLifespanParticleModifier(1.0f, 0.0f); }

        void IParticleModifier.ApplyModifier(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle) { ApplyModifier(gameTime, emitter, particle); }
        public void ApplyModifier(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle)
        {
            float normalizedLifetime = emitter.ParticleLifespan.GetNormalizedLifespan(gameTime, emitter, particle);

            // make particles grow as they age. they'll start at 75% of their size,
            // and increase to 100% once they're finished.
            float scale = particle.InitialScale * (StartScale + (EndScale * normalizedLifetime));

            particle.Scale = scale;
        }
    }
}
