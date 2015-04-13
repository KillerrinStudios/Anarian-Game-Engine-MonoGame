using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Concrete
{
    public class ContinuousEmitter : IEmissionSettings
    {
        bool IEmissionSettings.CanEmmit(GameTime gameTime, ParticleEmitter2D emitter)
        {

            return true;
        }
    }
}
