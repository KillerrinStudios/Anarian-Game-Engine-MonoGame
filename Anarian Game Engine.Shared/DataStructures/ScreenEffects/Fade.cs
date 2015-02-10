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
    public enum FadeStatus
    {
        None,
        FadingToContent,
        FadingIn
    }

    public class Fade : IScreenEffect, IRenderable
    {
        #region Fields/Properties
        ProgressStatus m_progressStatus;
        public ProgressStatus Progress { get { return m_progressStatus; } set { m_progressStatus = value; } }

        FadeStatus m_fadeStatus;
        public FadeStatus FadeStatus { get { return m_fadeStatus; } protected set { m_fadeStatus = value; } }

        float m_fadePercentage;
        public float FadePercentage { get { return m_fadePercentage; } protected set { m_fadePercentage = value; } }

        float m_fadeRate;
        public float FadeRate { get { return m_fadeRate; } set { m_fadeRate = value; } }


        public readonly float MinimumFade = 0.0f;
        public readonly float MaximumFade = 1.0f;


        Texture2D m_fadeTexture;
        public Texture2D FadeTexture { get { return m_fadeTexture; } set { m_fadeTexture = value; } }

        Color m_fadeColour;
        public Color FadeColour { get { return m_fadeColour; } set { m_fadeColour = value; } }

        private Rectangle m_screenRect;
        #endregion

        public event EventHandler ProgressTick;
        public event EventHandler Completed;

        public bool Active { get; set; }

        public Fade(GraphicsDevice graphicsDevice, Color solidColour, float fadeRate = 0.003f)
        {
            Active = true;

            m_progressStatus = ProgressStatus.None;
            m_fadeStatus = ScreenEffects.FadeStatus.FadingToContent;
            m_fadeRate = fadeRate;
            m_fadePercentage = MinimumFade;

            ChangeFadeColor(graphicsDevice, solidColour);
        }

        public Fade(Texture2D texture, float fadeRate = 0.003f)
        {
            Active = true;

            m_progressStatus = ProgressStatus.None;
            m_fadeStatus = ScreenEffects.FadeStatus.FadingToContent;
            m_fadeRate = fadeRate;
            m_fadePercentage = MinimumFade;

            m_fadeTexture = texture;
            m_fadeColour = Color.White;
        }

        #region Interface Implimentation
        void IScreenEffect.PreformEffect(GameTime gameTime) { ApplyEffect(gameTime); }
        void IScreenEffect.Draw(GameTime gameTime, SpriteBatch spriteBatch) { Draw(gameTime, spriteBatch); }
        ProgressStatus IScreenEffect.Progress { get { return Progress; } set { Progress = value; } }

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch); }
        #endregion

        #region Helper Methods
        public void ChangeFadeStatus(FadeStatus fadeStaus)
        {
            m_fadeStatus = fadeStaus;

            switch (fadeStaus) {
                case FadeStatus.None:
                    Progress = ProgressStatus.Completed;
                    break;
                case FadeStatus.FadingToContent:
                case FadeStatus.FadingIn:
                default:
                    Progress = ProgressStatus.NotStarted;
                    break;
            }
        }

        public void ChangeWithoutFade(FadeStatus fadeStatus)
        {
            m_fadeStatus = fadeStatus;
            Progress = ProgressStatus.Completed;

            switch (m_fadeStatus) {
                case FadeStatus.FadingToContent: m_fadePercentage = 0.0f; break;
                case FadeStatus.FadingIn: m_fadePercentage = 1.0f; break;
            }

            if (Completed != null)
                Completed(this, null);
        }

        public void ChangeFadeColor(GraphicsDevice graphicsDevice, Color colour)
        {
            m_fadeColour = colour;
            m_screenRect = graphicsDevice.Viewport.GetViewportRectangle();

            m_fadeTexture = m_fadeColour.CreateTextureFromSolidColor(graphicsDevice, 1, 1);
        }
        #endregion

        public void ApplyEffect(GameTime gameTime)
        {
            if (m_progressStatus == ProgressStatus.None) return;
            if (m_progressStatus == ProgressStatus.Completed) return;
            if (m_fadeStatus == FadeStatus.None) return;
            
            switch (m_fadeStatus) {
                case FadeStatus.FadingToContent:    m_fadePercentage -= m_fadeRate * gameTime.DeltaTime();  break;
                case FadeStatus.FadingIn:           m_fadePercentage += m_fadeRate * gameTime.DeltaTime();  break;
            }

            m_fadePercentage = MathHelper.Clamp(m_fadePercentage, MinimumFade, MaximumFade);
            if (m_fadePercentage <= MinimumFade || m_fadePercentage >= MaximumFade)
            {
                m_progressStatus = ProgressStatus.Completed;

                if (Completed != null)
                    Completed(this, null);
            }
            else {
                m_progressStatus = ProgressStatus.InProgress;

                if (ProgressTick != null)
                    ProgressTick(this, null);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Active) return;

            try
            {
                spriteBatch.Begin();
                spriteBatch.Draw(m_fadeTexture, m_screenRect, m_fadeColour * m_fadePercentage);
            }
            catch (Exception) { }

            try
            {
                spriteBatch.End();
            }
            catch (Exception) { }
        }

        public override string ToString()
        {
            return "Progress: " + m_progressStatus.ToString() + " | " +
                   "FadeStatus: " + m_fadeStatus.ToString() + " | " +
                   "FadePercentage: " + m_fadePercentage + " | " +
                   "FadeRate: " + m_fadeRate + " | " +
                   "Colour: " + m_fadeColour.ToString();
        }
    }
}
