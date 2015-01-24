using Anarian.Enumerators;
using Anarian.Interfaces;
using Anarian.Helpers;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Anarian.DataStructures.ScreenEffects
{
    public class Overlay : IScreenEffect, IRenderable
    {
        #region Fields/Properties
        ProgressStatus m_progressStatus;
        public ProgressStatus Progress { get { return m_progressStatus; } set { m_progressStatus = value; } }

        float m_fadePercentage;
        public float FadePercentage { get { return m_fadePercentage; } set { m_fadePercentage = value; } }

        Texture2D m_texture;
        public Texture2D Texture { get { return m_texture; } set { m_texture = value; } }

        Color m_colour;
        public Color Colour { get { return m_colour; } set { m_colour = value; } }
        #endregion

        public event EventHandler ProgressTick;
        public event EventHandler Completed;

        public Overlay(GraphicsDevice graphicsDevice, Color solidColour)
        {
            m_progressStatus = ProgressStatus.None;
            m_fadePercentage = 1.0f;

            ChangeFadeColor(graphicsDevice, solidColour);
        }

        public Overlay(Texture2D texture, Color colour)
        {
            m_progressStatus = ProgressStatus.None;
            m_fadePercentage = 1.0f;

            m_texture = texture;
            m_colour = colour;
        }

        #region Interface Implimentation
        void IScreenEffect.PreformEffect(GameTime gameTime) { ApplyEffect(gameTime); }
        void IScreenEffect.Draw(GameTime gameTime, SpriteBatch spriteBatch) { Draw(gameTime, spriteBatch); }
        ProgressStatus IScreenEffect.Progress { get { return Progress; } set { Progress = value; } }

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch); }
        #endregion

        #region Helper Methods
        public void ChangeFadeColor(GraphicsDevice graphicsDevice, Color colour)
        {
            m_colour = colour;
            m_texture = m_colour.CreateTextureFromSolidColor(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        }
        #endregion

        public void ApplyEffect(GameTime gameTime)
        {
            if (m_progressStatus == ProgressStatus.None) return;
            if (m_progressStatus == ProgressStatus.Completed) return;

            // Call the Events
            m_progressStatus = ProgressStatus.InProgress;
            if (ProgressTick != null)
                ProgressTick(this, null);

            m_progressStatus = ProgressStatus.Completed;
            if (Completed != null)
                Completed(this, null);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {            
            spriteBatch.Begin();
            spriteBatch.Draw(m_texture, Vector2.Zero, m_colour * m_fadePercentage);
            spriteBatch.End();
        }

        public override string ToString()
        {
            return "Progress: " + m_progressStatus.ToString() + " | " +
                   "FadePercentage: " + m_fadePercentage + " | " +
                   "Colour: " + m_colour.ToString();
        }
    }
}
