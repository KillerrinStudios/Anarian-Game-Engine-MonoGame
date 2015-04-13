using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers
{
    public interface IParticleModifier
    {
        void ApplyModifier(GameTime gameTime, Particle2D particle);
    }
}
