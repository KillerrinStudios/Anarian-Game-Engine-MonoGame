using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Anarian.Enumerators;

namespace Anarian.GUI
{
    public class Element : GuiObject
    {
        protected GuiState m_guiState;

        #region Monogame SpriteBatch 
        public Texture2D Texture { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public Color Colour { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public float Depth { get; set; }
        #endregion

        public Element(Texture2D _texture, Vector2 _position, Color _color, Vector2 _scale, Vector2? _origin,
            float _rotation = 0.0f,
            Rectangle? _sourceRectangle = null,
            SpriteEffects _spriteEffects = SpriteEffects.None,
            float _depth = 0.0f)
            :base()
        {
            // Set the Transform
            m_transform.Position = _position;
            m_transform.Scale = _scale;
            m_transform.Rotation = _rotation;

            if (_texture == null) m_transform.WidthHeight = Vector2.Zero;
            else m_transform.WidthHeight = new Vector2(_texture.Width, _texture.Height);

            if (_origin.HasValue) m_transform.Origin = _origin.Value;
            else m_transform.SetOriginToCenter();

            // Set the rest of the Element
            Texture = _texture;
            SourceRectangle = _sourceRectangle;
            Colour = _color;
            SpriteEffect = _spriteEffects;
            Depth = _depth;

            m_guiState = GuiState.None;
        }

        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (!m_active) return;

            // Draw the Element
            if (!m_visible) return;
            if (Texture == null) return;

            // Since we don't want to be drawing the current element over the children,
            // we Draw the Children after we draw this
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, m_transform.WorldPosition, SourceRectangle, Colour, m_transform.WorldRotation, m_transform.Origin, m_transform.WorldScale, SpriteEffect, Depth);
            spriteBatch.End();

            // Draw the Children
            base.Draw(gameTime, spriteBatch, graphics);
        }
    }
}
