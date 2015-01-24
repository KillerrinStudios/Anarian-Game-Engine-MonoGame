using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Anarian;
using Anarian.DataStructures;
using Anarian.Interfaces;

namespace Anarian.Helpers
{
    public static class PrimitiveHelper2D
    {
        public static Texture2D Texture;

        #region Helper Methods
        public static void SetDefaultTexture()
        {
            Texture = ResourceManager.Instance.GetAsset(typeof(Texture2D), ResourceManager.EngineReservedAssetNames.blankTextureName) as Texture2D;
        }
        private static Rectangle GetCenterOfPoint(int size, Vector2 position)
        {
            Rectangle rect = new Rectangle();
            rect.X = (int)(position.X - (size / 2.0f));
            rect.Y = (int)(position.Y - (size / 2.0f));
            rect.Width = size;
            rect.Height = size;
            return rect;
        }
        #endregion

        /// <summary>
        /// Draws Square Points on the screen at the specified Position, Size and Color
        /// </summary>
        /// <param name="spriteBatch">The Spritebatch</param>
        /// <param name="color">The color of the point</param>
        /// <param name="size">The Size of the point</param>
        /// <param name="param">Vector2s representing position on the screen</param>
        public static void DrawPoints(SpriteBatch spriteBatch, Color color, int size, params Vector2[] param)
        {
            if (Texture == null) SetDefaultTexture();
            
            spriteBatch.Begin();
            for (int i = 0; i < param.Length; i++)
            {
                Rectangle rect = GetCenterOfPoint(size, param[i]);
                spriteBatch.Draw(Texture, rect, color);
            }
            spriteBatch.End();
        }

        public static void DrawRect(SpriteBatch spriteBatch, Color color, Rectangle rect)
        {
            if (Texture == null) SetDefaultTexture();

            spriteBatch.Begin();
            spriteBatch.Draw(Texture, rect, color);
            spriteBatch.End();
        }

        public static void DrawRect(SpriteBatch spriteBatch, Color color, Vector2 startCorner, Vector2 endCorner)
        {
            Vector2 size = endCorner - startCorner;
            Rectangle rect = new Rectangle((int)startCorner.X, (int)startCorner.Y, (int)size.X, (int)size.Y);
            DrawRect(spriteBatch, color, rect);
        }

        /// <summary>
        /// Draws Square Points on the screen at the specified Position, Size and Color
        /// </summary>
        /// <param name="spriteBatch">The Spritebatch</param>
        /// <param name="color">The color of the point</param>
        /// <param name="size">The Size of the point</param>
        /// <param name="param">Vector2s representing position on the screen</param>
        public static void DrawLines(SpriteBatch spriteBatch, Color color, int size, Vector2 p1, Vector2 p2, params Vector2[] param)
        {
            if (Texture == null) SetDefaultTexture();
            
            // Do the math
            Rectangle r = new Rectangle((int)p1.X, (int)p1.Y, (int)(p2 - p1).Length() + size, size);
            Vector2 v = Vector2.Normalize(p1 - p2);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (p1.Y > p2.Y) angle = MathHelper.TwoPi - angle;

            // Draw the Line
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.End();

            // Recursively Draw Lines until there are no more left
            if (param.Length > 0) {
                List<Vector2> leftoverParams = new List<Vector2>();
                for (int i = 1; i < param.Length; i++) {
                    leftoverParams.Add(param[i]);
                }

                DrawLines(spriteBatch, color, size, p2, param[0], leftoverParams.ToArray());
            }
        }

        /// <summary>
        /// Draws an arc at the specified Position, Color and Radius
        /// </summary>
        /// <param name="spriteBatch">The Spritebatch</param>
        /// <param name="color">The Color of the circle</param>
        /// <param name="size">The Size of the point</param>
        /// <param name="arc">How far along the arc to draw in degrees</param>
        /// <param name="radius">The Radius of the arc</param>
        /// <param name="position">The Position on the screen</param>
        public static void DrawArc(SpriteBatch spriteBatch, Color color, int size, float arc, float radius, Vector2 position)
        {
            for (float i = 0; i < arc; i += 1.0f) {
                Vector2 point = new Vector2(
                    (radius * 2.0f) * (float)Math.Cos((double)MathHelper.ToRadians(i)),
                    (radius * 2.0f) * (float)Math.Sin((double)MathHelper.ToRadians(i))
                );
                point += position;

                DrawPoints(spriteBatch, color, size, point);
            }
        }

        /// <summary>
        /// Draws a Sine Wave at the specified Position, Color and Radius
        /// </summary>
        /// <param name="spriteBatch">The Spritebatch</param>
        /// <param name="color">The Color of the circle</param>
        /// <param name="size">The Size of the point</param>
        /// <param name="position">The Position on the screen</param>
        /// <param name="amplitude">The peak deviation of the function from zero</param>
        /// <param name="frequency">The number of oscillations (cycles) that occur each second of time</param>
        /// <param name="maxTime">The maximum ammount of time the Sine will go from </param>
        /// <param name="phase">Where (In Degrees) in its cycle the oscillation is at t = 0 </param>
        /// <example>DrawSineWave(spriteBatch, Color.Red, 4, new Vector2(0.0f, 600.0f), 100.0f, 0.006f, GraphicsDevice.Viewport.Width, 0.0f);</example>
        public static void DrawSineWave(SpriteBatch spriteBatch, Color color, int size, Vector2 position, float amplitude, float frequency, float maxTime, float phase = 0.0f)
        {
            for (float i = 0.0f; i < maxTime; i += 1.0f) {
                float y = amplitude * (float)Math.Sin((double)((2.0f * Math.PI * frequency) * i + MathHelper.ToRadians(phase)));

                Vector2 point = new Vector2(i, y);
                point += position;

                DrawPoints(spriteBatch, color, size, point);
            }
        }

        /// <summary>
        /// Draws a Circle Outline at the specified Position, Color and Radius
        /// </summary>
        /// <param name="spriteBatch">The Spritebatch</param>
        /// <param name="color">The Color of the circle</param>
        /// <param name="size">The Size of the point</param>
        /// <param name="radius">The Radius of the point</param>
        /// <param name="position">The Position on the screen</param>
        public static void DrawCircle(SpriteBatch spriteBatch, Color color, int size, float radius, Vector2 position)
        {
            DrawArc(spriteBatch, color, size, 360.0f, radius, position);
        }
    }
}
