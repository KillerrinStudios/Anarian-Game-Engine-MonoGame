using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Anarian.Enumerators;
using Anarian.Events;
using Anarian.GUI.Events;
using Anarian.Helpers;

namespace Anarian.GUI
{
    public class Button : Element
    {
        public Texture2D NormalTexture { get; set; }
        public Texture2D DownTexture { get; set; }
        public Texture2D DisabledTexture { get; set; }

        public Label Label { get; set; }


        public Button(Texture2D _normalTexture, Vector2 _position, Rectangle? _sourceRectangle = null, float _depth = 0.0f)
            : base(_normalTexture, _position, Color.White, Vector2.One, null, 0.0f, _sourceRectangle, SpriteEffects.None, _depth)
        {
            m_guiState = GuiState.None;
            
            NormalTexture = _normalTexture;
        }

        public Button(GraphicsDevice graphics, Color texColor, Color downColor, Color disabledColor, Vector2 _position, Vector2 widthHeight, float _depth = 0.0f)
            : base(null, _position, Color.White, Vector2.One, null, 0.0f, null, SpriteEffects.None, _depth)
        {
            m_guiState = GuiState.None;

            NormalTexture = texColor.CreateTextureFromSolidColor(graphics, (int)widthHeight.X, (int)widthHeight.Y);
            DownTexture = downColor.CreateTextureFromSolidColor(graphics, (int)widthHeight.X, (int)widthHeight.Y);
            DisabledTexture = disabledColor.CreateTextureFromSolidColor(graphics, (int)widthHeight.X, (int)widthHeight.Y);

            Texture = NormalTexture;
            m_transform.WidthHeight = new Vector2(Texture.Width, Texture.Height);
            m_transform.SetOriginToCenter();
        }

        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (!m_active) return;

            // Draw the Children + Button
            base.Draw(gameTime, spriteBatch, graphics);

            // Draw the Label
            if (!m_visible) return;

            if (Label != null) {
                Label.Draw(gameTime, spriteBatch, graphics);
            }
        }

        public event GuiButtonPressedEventHandler Pressed;
        internal void CallPressed()
        {
            if (Pressed == null) return;
            Pressed(this, new GuiButtonPressedEventArgs());
        }

        public event GuiButtonDownEventHandler ButtonDown;
        internal void CallButtonDown()
        {
            if (ButtonDown == null) return;
            ButtonDown(this, new GuiButtonPressedEventArgs());
        }
    }
}
