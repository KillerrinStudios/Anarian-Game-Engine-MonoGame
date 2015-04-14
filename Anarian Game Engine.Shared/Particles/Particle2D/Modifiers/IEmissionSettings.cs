using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers
{
    public interface IEmissionSettings
    {
        bool Active { get; set; }

        bool CanEmmit(GameTime gameTime, ParticleEmitter2D emitter);
    }
}
