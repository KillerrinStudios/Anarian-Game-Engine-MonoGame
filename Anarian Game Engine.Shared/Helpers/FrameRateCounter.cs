using Anarian.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Anarian.Helpers
{
	/// <summary>
	/// A reusable component for tracking the frame rate.
	/// </summary>
	public class FrameRateCounter : IUpdatable, IRenderable
	{
		#region Fields
		SpriteFont m_spriteFont;

		Vector2 m_position;
		Vector2 m_fpsPos;

		int m_frameRate = 0;
		int m_frameCounter = 0;

		TimeSpan m_elapsedTime = TimeSpan.Zero;

		public SpriteFont SpriteFont { get { return m_spriteFont; } set { m_spriteFont = value; } }
		public Vector2 Position { get { return m_position; } set { m_position = value; } }

		public int FrameRate { get { return m_frameRate; } }
		public int FrameCounter { get { return m_frameCounter; } }
		public TimeSpan ElapsedTime { get { return m_elapsedTime; } }
		#endregion

        public FrameRateCounter()
        {
            m_spriteFont = null;
            Reset();
        }
		public FrameRateCounter(SpriteFont spriteFont)
		{
			m_spriteFont = spriteFont;
            Reset();
		}

        public void Reset()
        {
            m_position = new Vector2(10.0f, 10.0f);
            m_fpsPos = new Vector2();
        }

		void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
		public void Update(GameTime gameTime)
		{
			m_elapsedTime += gameTime.ElapsedGameTime;

			if (m_elapsedTime > TimeSpan.FromSeconds(1))
			{
				m_elapsedTime -= TimeSpan.FromSeconds(1);
				m_frameRate = m_frameCounter;
				m_frameCounter = 0;
			}
		}

		void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
		{
			m_frameCounter++;
			string fps = string.Format("{0} FPS", m_frameRate);

            if (m_spriteFont == null) return;
			m_fpsPos = new Vector2((graphics.Viewport.Width - m_spriteFont.MeasureString(fps).X) - 15, 10);

			spriteBatch.Begin();
			spriteBatch.DrawString(m_spriteFont, fps, m_position, Color.White);
			spriteBatch.End();
		}
	}
}
