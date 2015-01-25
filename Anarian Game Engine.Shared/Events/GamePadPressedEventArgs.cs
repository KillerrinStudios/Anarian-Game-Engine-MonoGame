using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void GamePadPressedEventHandler(object sender, GamePadPressedEventArgs e);
    public delegate void GamePadDownEventHandler(object sender, GamePadPressedEventArgs e);

    public class GamePadPressedEventArgs : AnarianEventArgs
    {
        public Buttons ButtonPressed { get; private set; }
        public PlayerIndex GamePadIndex { get; private set; }

        public GamePadPressedEventArgs(GameTime gameTime)
            : base(gameTime)
        {
            ButtonPressed = Buttons.BigButton;
            GamePadIndex = PlayerIndex.One;
        }
        public GamePadPressedEventArgs(GameTime gameTime, Buttons buttonPressed, PlayerIndex playerIndex)
            : base(gameTime)
        {
            ButtonPressed = buttonPressed;
            GamePadIndex = playerIndex;
        }
        public GamePadPressedEventArgs(GameTime gameTime, Buttons buttonPressed, PlayerIndex playerIndex, Exception e, bool canceled, Object state)
            : base(gameTime, e, canceled, state)
        {
            ButtonPressed = buttonPressed;
            GamePadIndex = playerIndex;
        }
    }
}
