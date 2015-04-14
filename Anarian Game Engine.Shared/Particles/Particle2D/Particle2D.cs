using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles.Particle2D
{
    public class Particle2D : IUpdatable
    {
        public bool Alive;

        public float TimeAlive;
        public float TotalLifespan;

        public Vector2 InitialPosition;
        public Color InitialColor;
        public float InitialScale;

        public Texture2D Texture;
        public Color Colour;

        public Vector2 Origin;

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public float Scale;

        public float Rotation;
        public float RotationSpeed;

        public Particle2D()
        {
            Initialize(null, Color.White,
                       Vector2.Zero, Vector2.Zero, Vector2.Zero,
                       2.0f,
                       1.0f, 0.0f, 0.0f);
        }

        public virtual void Initialize(Texture2D texture, Color colour,
                                       Vector2 position, Vector2 velocity, Vector2 acceleration,
                                       float totalLifespan = 0.0f,
                                       float scale = 1.0f, float rotation = 0.0f, float rotationSpeed = 0.0f)
        {
            Alive = true;
            TimeAlive = 0.0f;
            TotalLifespan = totalLifespan;

            InitialPosition = position;
            InitialColor = colour;
            InitialScale = scale;

            Texture = texture;
            Colour = colour;

            if (Texture != null)
                Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            else Origin = Vector2.Zero;

            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;

            Scale = scale;
            Rotation = rotation;
            RotationSpeed = rotationSpeed;
        }

        #region Update/Draw
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public virtual void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Velocity += Acceleration * deltaTime;
            Position += Velocity * deltaTime;
            Rotation += RotationSpeed * deltaTime;

            TimeAlive += deltaTime;
        }
        #endregion

    }
}
