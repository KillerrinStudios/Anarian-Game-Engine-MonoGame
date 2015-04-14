using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Particles
{
    public class TextureColorPair
    {
        public Texture2D Texture;
        public Color Colour;

        public TextureColorPair(Texture2D texture)
        {
            Texture = texture;
            Colour = Color.White;
        }
        public TextureColorPair(Texture2D texture, Color color)
        {
            Texture = texture;
            Colour = color;
        }
    }
}
