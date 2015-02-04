using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Anarian.Helpers;
using Anarian.DataStructures.ScreenEffects;

namespace Anarian
{
    public class CreditManager
    {
        public bool Active;

        private enum CreditState
        {
            Scrolling,
            Transitioning
        }

        private CreditState state;

        private Texture2D background;

        private Color m_textColor;
        public SpriteFont SmallFont;
        public SpriteFont LargeFont;


        private List<RenderText> creditText;
        private List<CreditImage> creditImage;

        private Vector2 creditScrollSpeed;
        private Vector2 lastPosition;

        private int width;
        private int height;

        const float TEXT_SEPERATION = 50f;
        const float SIDE_BY_SIDE_IMAGE_SEPERATION = 25f;

        Fade m_fade;

        public event EventHandler InProgress;
        public event EventHandler Completed;

        public CreditManager(GraphicsDevice graphics, Color textColor, SpriteFont smallFont, SpriteFont largeFont)
        {
            Active = true;

            width = graphics.Viewport.Bounds.Width;
            height = graphics.Viewport.Bounds.Height;

            state = CreditState.Scrolling;

            creditScrollSpeed = new Vector2(0f, -0.06f);

            m_textColor = textColor;
            lastPosition = new Vector2(width / 2f, height + TEXT_SEPERATION);

            SmallFont = smallFont;
            LargeFont = largeFont;

            creditText = new List<RenderText> { };
            creditImage = new List<CreditImage> { };

            // Create the Fade
            m_fade = new Fade(graphics, Color.Black);
            m_fade.ProgressTick += m_fade_ProgressTick;
            m_fade.Completed += m_fade_Completed;
            m_fade.ChangeWithoutFade(FadeStatus.FadingToContent);
        }

        #region Fade Events
        void m_fade_ProgressTick(object sender, EventArgs e)
        {

        }
        void m_fade_Completed(object sender, EventArgs e)
        {
            if (Completed != null)
                Completed(this, null);
        }
        #endregion

        #region Credit Content
        public void AddHeader(string text)
        {
            Vector2 textSize = LargeFont.MeasureString(text);

            creditText.Add(new RenderText(text, new Vector2((width / 2f) - (textSize.X / 2f), lastPosition.Y + TEXT_SEPERATION), m_textColor, LargeFont));
            lastPosition.Y += TEXT_SEPERATION;
        }

        public void AddContent(string text)
        {
            Vector2 textSize = SmallFont.MeasureString(text);

            creditText.Add(new RenderText(text, new Vector2((width / 2f) - (textSize.X / 2f), lastPosition.Y + TEXT_SEPERATION), m_textColor, SmallFont));
            lastPosition.Y += TEXT_SEPERATION;
        }

        public void AddSpacing(float seperation)
        {
            lastPosition.Y += seperation;
        }
        public void AddSpacing()
        {
            lastPosition.Y += TEXT_SEPERATION;
        }

        public void AddCenteredImage(Texture2D texture)
        {
            Vector2 position = new Vector2(width / 2f, lastPosition.Y + TEXT_SEPERATION);
            position.X -= (texture.Width / 2f);

            creditImage.Add(new CreditImage(texture, position));
            lastPosition.Y += texture.Height + TEXT_SEPERATION;
        }

        public void AddTwoImage(Texture2D texture1, Texture2D texture2)
        {
            creditImage.Add(new CreditImage(texture1, new Vector2(50f, lastPosition.Y + TEXT_SEPERATION)));
            creditImage.Add(new CreditImage(texture2, new Vector2(50f + texture1.Width + SIDE_BY_SIDE_IMAGE_SEPERATION, lastPosition.Y + TEXT_SEPERATION)));
            lastPosition.Y += (texture1.Height > texture2.Height) ? texture1.Height : texture2.Height + TEXT_SEPERATION;
        }
        #endregion

        public void Update(GameTime gameTime)
        {
            switch (state)
            {
                case CreditState.Scrolling:
                    if (creditImage[creditImage.Count() - 1].position.Y <= ((height / 2) - (creditImage[creditImage.Count() - 1].image.Height / 2) + (TEXT_SEPERATION)))
                    {
                        state = CreditState.Transitioning;
                        m_fade.ChangeFadeStatus(FadeStatus.FadingIn);
                        break;
                    }

                    float elapsedTime = gameTime.ElapsedGameTime.Milliseconds;
                    foreach (RenderText i in creditText)
                    {
                        i.Location += creditScrollSpeed * elapsedTime;
                    }
                    foreach (CreditImage i in creditImage)
                    {
                        i.position += creditScrollSpeed * elapsedTime;
                    }

                    if (InProgress != null)
                        InProgress(this, null);

                    break;
                case CreditState.Transitioning:
                    m_fade.ApplyEffect(gameTime);

                    break;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (RenderText i in creditText) { i.Draw(spriteBatch); }
            foreach (CreditImage i in creditImage) { i.Draw(spriteBatch); }

            m_fade.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }
    }

    class CreditImage
    {
        public Texture2D image;
        public Color color;
        public Vector2 position;

        public CreditImage(Texture2D texture, Vector2 location)
        {
            image = texture;
            position = location;
            color = Color.White;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, position, color);
        }

    }
}
