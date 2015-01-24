using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Anarian.GUI
{
    public class Label : GuiObject
    {
        #region Fields/Properties
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Color Colour { get; set; }

        /// <summary>
        /// Wrapper around Transform.WidthHeight for readability
        /// </summary>
        public Vector2 StringSize { get { return m_transform.WidthHeight; } protected set { m_transform.WidthHeight = value; } }
        #endregion

        public Label(SpriteFont font, string text, Vector2 position, Color color)
            :base()
        {
            // Set the Transform
            m_transform.Position = position;
            
            // Set Everything Else
            Text = text;
            Font = font;
            Colour = color;

            StringSize = font.MeasureString(text);
            m_transform.SetOriginToCenter();
        }

        public void ApplyFormattedText(string str, params object[] parameters)
        {
            Text = String.Format(str, parameters);
        }

        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (!m_active) return;

            // Draw the Children
            base.Draw(gameTime, spriteBatch, graphics);

            // Draw the Label
            if (!m_visible) return;
            if (Font == null || string.IsNullOrEmpty(Text)) return;

            spriteBatch.Begin();
            spriteBatch.DrawString(Font, Text, m_transform.WorldPosition, Colour, m_transform.WorldRotation, m_transform.Origin, m_transform.WorldScale, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
    }
}
