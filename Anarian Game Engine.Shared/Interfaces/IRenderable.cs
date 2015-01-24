using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Anarian.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace Anarian.Interfaces
{
    public interface IRenderable
    {
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera);
    }
}
