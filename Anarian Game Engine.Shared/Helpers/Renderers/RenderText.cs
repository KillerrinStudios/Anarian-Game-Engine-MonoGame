using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Anarian.Helpers
{
    public class RenderText
    {
        public string Text { get; set; }
        public Vector2 Location { get; set; }
        public Rectangle CollideRectangle { get; set; }
        public Color Colour { get; set; }
        public SpriteFont Font { get; set; }

        public RenderText(string text, Vector2 location, Color color, SpriteFont font)
        {
            Text = text;
            Location = location;
            Colour = color;
            Font = font;

            Vector2 widthHeight = font.MeasureString(text);
            CollideRectangle = new Rectangle((int)Location.X, (int)Location.Y, (int)widthHeight.X, (int)widthHeight.Y);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, Location, Colour);
        }
        public static void Draw(SpriteBatch spriteBatch, string customText, Vector2 location, Color clr, SpriteFont font)
        {
            spriteBatch.DrawString(font, customText, location, clr);
        }
    }
}
