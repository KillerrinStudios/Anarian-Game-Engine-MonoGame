using Anarian.Particles.Particle2D.Modifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D
{
    public class ParticleEmitter2D
    {        
        public IEmissionSettings EmissionSettings;
        public IParticleLifespan ParticleLifespan;

        public readonly uint MaxNumberOfParticles;
        public List<Particle2D> m_activeParticles;
        public List<Particle2D> m_inactiveParticles;

        public ParticleEmitter2D(uint maxNumberOfParticles)
        {
            MaxNumberOfParticles = maxNumberOfParticles;
            m_activeParticles = new List<Particle2D>((int)MaxNumberOfParticles);
            m_inactiveParticles = new List<Particle2D>((int)MaxNumberOfParticles);
        }
    }
}
