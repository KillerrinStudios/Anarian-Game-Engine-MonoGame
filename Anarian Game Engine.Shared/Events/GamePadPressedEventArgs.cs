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

    public class GamePadPressedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public Buttons ButtonPressed { get; private set; }
        public PlayerIndex GamePadIndex { get; private set; }

        public GamePadPressedEventArgs()
            : base(new Exception(), false, null)
        {
            ButtonPressed = Buttons.BigButton;
            GamePadIndex = PlayerIndex.One;
        }
        public GamePadPressedEventArgs(Buttons buttonPressed, PlayerIndex playerIndex)
            : base(new Exception(), false, null)
        {
            ButtonPressed = buttonPressed;
            GamePadIndex = playerIndex;
        }
        public GamePadPressedEventArgs(Buttons buttonPressed, PlayerIndex playerIndex, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            ButtonPressed = buttonPressed;
            GamePadIndex = playerIndex;
        }
    }
}
