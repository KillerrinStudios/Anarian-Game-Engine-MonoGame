using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Lifespan
{
    public class TimebasedParticleLifespan : IParticleLifespan
    {
        Vector2 m_lifespan;
        public Vector2 Lifespan { get { return m_lifespan; } set { m_lifespan = value; } }
        Vector2 IParticleLifespan.TotalLifespan { get { return Lifespan; } set { Lifespan = value; } }

        public TimebasedParticleLifespan(float maximumLifespan)
        {
            m_lifespan = new Vector2(maximumLifespan, maximumLifespan);
        }
        public TimebasedParticleLifespan(float minimumLifespan, float maximumLifespan)
        {
            m_lifespan = new Vector2(minimumLifespan, maximumLifespan);
        }

        float IParticleLifespan.GetNormalizedLifespan(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle) { return GetNormalizedLifespan(gameTime, emitter, particle); }
        public float GetNormalizedLifespan(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle)
        {
            return particle.TimeAlive / particle.MaxLifespan;
        }

        bool IParticleLifespan.IsAlive(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle) { return IsAlive(gameTime, emitter, particle); }
        public bool IsAlive(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle)
        {
            if (particle.TimeAlive >= particle.MaxLifespan)
            {
                particle.Alive = false;
                return false;
            }

            particle.Alive = true;
            return true;
        }
    }
}
