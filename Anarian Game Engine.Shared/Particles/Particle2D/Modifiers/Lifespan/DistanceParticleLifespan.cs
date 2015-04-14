using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Lifespan
{
    public class DistanceParticleLifespan : IParticleLifespan
    {
        Vector2 m_lifespan;
        Vector2 IParticleLifespan.TotalLifespan { get { return m_lifespan; } set { m_lifespan = value; } }

        public Vector2 TargetPosition;

        public DistanceParticleLifespan(Vector2 targetPosition)
        {
            m_lifespan = Vector2.One;
            TargetPosition = targetPosition;
        }

        float IParticleLifespan.GetNormalizedLifespan(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle) { return GetNormalizedLifespan(gameTime, emitter, particle); }
        public float GetNormalizedLifespan(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle)
        {
            float distanceEmitterToTarget = Vector2.Distance(emitter.Position, TargetPosition);
            float distanceEmitterToParticle = Vector2.Distance(emitter.Position, particle.Position);
            float difference = (distanceEmitterToParticle / distanceEmitterToTarget);

            return MathHelper.Clamp(difference, 0.0f, 1.0f);
        }

        bool IParticleLifespan.IsAlive(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle) { return IsAlive(gameTime, emitter, particle); }
        public bool IsAlive(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle)
        {
            float distanceEmitterToTarget = Vector2.Distance(emitter.Position, TargetPosition);
            float distanceEmitterToParticle = Vector2.Distance(emitter.Position, particle.Position);
            float difference = (distanceEmitterToTarget - distanceEmitterToParticle);

            if (difference <= 0.0f)
            {
                particle.Alive = false;
                return false;
            }

            particle.Alive = true;
            return true;
        }
    }
}
