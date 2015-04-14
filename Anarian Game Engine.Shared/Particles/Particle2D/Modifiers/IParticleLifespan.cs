using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers
{
    public interface IParticleLifespan
    {
        Vector2 TotalLifespan { get; set; }

        float GetNormalizedLifespan(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle);
        bool IsAlive(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle);
    }
}
