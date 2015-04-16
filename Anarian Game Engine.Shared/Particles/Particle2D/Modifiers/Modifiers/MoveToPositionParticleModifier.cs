using Anarian.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Anarian.Particles.Particle2D.Modifiers.Modifiers
{
    public class MoveToPositionParticleModifier : IParticleModifier
    {
        public Vector2 TargetPosition;
        public float MovementSpeed;

        public MoveToPositionParticleModifier(Vector2 targetPosition, float speed)
        {
            TargetPosition = targetPosition;
            MovementSpeed = speed;
        }

        void IParticleModifier.ApplyModifier(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle) { ApplyModifier(gameTime, emitter, particle); }
        public void ApplyModifier(GameTime gameTime, ParticleEmitter2D emitter, Particle2D particle)
        {
            Vector2 direction = TargetPosition - particle.Position;
            direction.Normalize();

            Vector2 speed = direction * MovementSpeed;
            particle.Position += speed * gameTime.DeltaTime();
        }
    }
}
