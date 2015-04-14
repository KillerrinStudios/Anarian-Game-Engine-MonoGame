using Anarian.DataStructures;
using Anarian.Events;
using Anarian.Interfaces;
using Anarian.Particles.Particle2D.Modifiers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Anarian.Particles.Particle2D
{
    public abstract class ParticleEmitter2D : AnarianObject, IUpdatable, IRenderable
    {
        public readonly uint MaxNumberOfParticles;
        public List<Particle2D> m_activeParticles;
        public List<Particle2D> m_inactiveParticles;

        public int ActiveCount { get { return m_activeParticles.Count; } }
        public int InactiveCount { get { return m_inactiveParticles.Count; } }

        public event AnarianEventHandler OnEmission;

        #region Emitter Settings
        public IEmissionSettings EmissionSettings;
        public IParticleLifespan ParticleLifespan;

        public List<Texture2D> ParticleTextures;
        public List<IParticleModifier> ParticleModifiersPreUpdate;
        public List<IParticleModifier> ParticleModifiersPostUpdate;

        public Vector2 Position;

        /// <summary>
        /// minInitialSpeed and maxInitialSpeed are used to control the initial velocity
        /// of the particles. The particle's initial speed will be a random number 
        /// between these two. The direction is determined by the function 
        /// PickRandomDirection, which can be overriden.
        /// </summary>
        protected float minInitialSpeed;
        protected float maxInitialSpeed;

        /// <summary>
        /// minRotationSpeed and maxRotationSpeed control the particles' angular
        /// velocity: the speed at which particles will rotate. Each particle's rotation
        /// speed will be a random number between minRotationSpeed and maxRotationSpeed.
        /// Use smaller numbers to make particle systems look calm and wispy, and large 
        /// numbers for more violent effects.
        /// </summary>
        protected float minAcceleration;
        protected float maxAcceleration;

        /// <summary>
        /// minLifetime and maxLifetime are used to control the lifetime. Each
        /// particle's lifetime will be a random number between these two. Lifetime
        /// is used to determine how long a particle "lasts." Also, in the base
        /// implementation of Draw, lifetime is also used to calculate alpha and scale
        /// values to avoid particles suddenly "popping" into view
        /// </summary>
        protected float minRotationSpeed;
        protected float maxRotationSpeed;

        /// <summary>
        /// to get some additional variance in the appearance of the particles, we give
        /// them all random scales. the scale is a value between minScale and maxScale,
        /// and is additionally affected by the particle's lifetime to avoid particles
        /// "popping" into view.
        /// </summary>
        protected float minScale;
        protected float maxScale;

        /// <summary>
        /// different effects can use different blend states. fire and explosions work
        /// well with additive blending, for example.
        /// </summary>
        protected BlendState blendState;
        #endregion

        public ParticleEmitter2D(uint maxNumberOfParticles, IEmissionSettings emissionSetting, IParticleLifespan particleLifespan)
            :base()
        {
            MaxNumberOfParticles = maxNumberOfParticles;
            m_activeParticles = new List<Particle2D>((int)MaxNumberOfParticles);
            m_inactiveParticles = new List<Particle2D>((int)MaxNumberOfParticles);

            // Emitter Settings
            EmissionSettings = emissionSetting;
            ParticleLifespan = particleLifespan;

            ParticleTextures = new List<Texture2D>();
            ParticleModifiersPreUpdate = new List<IParticleModifier>();
            ParticleModifiersPostUpdate = new List<IParticleModifier>();

            Position = Vector2.Zero;

            InitializeConstants();
            Reset();
        }
        public virtual void Reset()
        {
            m_activeParticles.Clear();
            m_inactiveParticles.Clear();

            for (int i = 0; i < MaxNumberOfParticles; i++)
            {
                Particle2D p = new Particle2D();
                m_inactiveParticles.Add(p);
            }
        }

        protected abstract void InitializeConstants();

        #region Helpers Methods
        protected virtual void InitializeParticle(GameTime gameTime, Particle2D particle)
        {
            if (ParticleTextures.Count == 0) return;
            Texture2D texture = ParticleTextures[ParticleHelpers.RandomBetween(0, ParticleTextures.Count)];
            if (texture == null) return;

            Color color = Color.White;

            // first, call PickRandomDirection to figure out which way the particle
            // will be moving. velocity and acceleration's values will come from this.
            Vector2 direction = PickRandomDirection();

            // pick some random values for our particle
            float lifespan = ParticleHelpers.RandomBetween(ParticleLifespan.TotalLifespan.X, ParticleLifespan.TotalLifespan.Y);
            float velocity =  ParticleHelpers.RandomBetween(minInitialSpeed, maxInitialSpeed);
            float acceleration = ParticleHelpers.RandomBetween(minAcceleration, maxAcceleration);
            float scale = ParticleHelpers.RandomBetween(minScale, maxScale);
            float rotationSpeed = ParticleHelpers.RandomBetween(minRotationSpeed, maxRotationSpeed);

            // then initialize it with those random values. initialize will save those,
            // and make sure it is marked as active.
            particle.Initialize(texture, color,
                                Position, velocity * direction, acceleration * direction,
                                lifespan,
                                scale, 0.0f, rotationSpeed);
        }
        protected virtual Vector2 PickRandomDirection()
        {
            float angle = ParticleHelpers.RandomBetween(0, MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        #endregion

        #region Update/Draw
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public virtual void Update(GameTime gameTime)
        {
            for (int i = m_activeParticles.Count - 1; i > -1; i--)
            {
                if (!ParticleLifespan.IsAlive(gameTime, this, m_activeParticles[i]))
                {
                    m_inactiveParticles.Add(m_activeParticles[i]);
                    m_activeParticles.RemoveAt(i);
                }
                else
                {
                    foreach (var modifier in ParticleModifiersPreUpdate)
                        modifier.ApplyModifier(gameTime, this, m_activeParticles[i]);

                    m_activeParticles[i].Update(gameTime);

                    foreach (var modifier in ParticleModifiersPostUpdate)
                        modifier.ApplyModifier(gameTime, this, m_activeParticles[i]);
                }
            }

            if (EmissionSettings.CanEmmit(gameTime, this))
            {
                if (OnEmission != null)
                    OnEmission(this, new AnarianEventArgs(gameTime));

                for (int i = m_inactiveParticles.Count - 1; i > -1; i--)
                {
                    var particle = m_inactiveParticles[i];

                    m_activeParticles.Add(particle);
                    m_inactiveParticles.RemoveAt(i);

                    InitializeParticle(gameTime, particle);
                }
            }
        }

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, blendState);

            foreach (var particle in m_activeParticles)
            {
                if (!particle.Alive) continue;

                spriteBatch.Draw(particle.Texture, particle.Position, null, particle.Colour,
                                 particle.Rotation, particle.Origin, particle.Scale, SpriteEffects.None, 0.0f);
            }

            spriteBatch.End();
        }
        #endregion
    }
}
