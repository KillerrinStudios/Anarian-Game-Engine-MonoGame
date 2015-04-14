using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Modifiers
{
    public class PositionProjectionParticleModifier : IParticleModifier
    {
        public ICamera Camera;
        public Viewport Viewport;
        public float ParticleYAxis;

        public PositionProjectionParticleModifier(ICamera camera, Viewport viewport, float particleYAxis = 0.0f)
        {
            Camera = camera;
            Viewport = viewport;
            ParticleYAxis = particleYAxis;
        }

        void IParticleModifier.ApplyModifier(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle) { ApplyModifier(gameTime, emitter, particle); }
        public void ApplyModifier(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle)
        {
            Vector2 position = Camera.ProjectToScreenCoordinates(new Vector3(particle.Position.X, ParticleYAxis, particle.Position.Y), Viewport);
            particle.Position = position;
        }
    }
}
