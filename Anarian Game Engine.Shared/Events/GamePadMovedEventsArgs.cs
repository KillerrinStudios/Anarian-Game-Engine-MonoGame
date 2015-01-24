using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void GamePadMovedEventHandler(object sender, GamePadMovedEventsArgs e);

    public class GamePadMovedEventsArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public Buttons ButtonPressed { get; private set; }
        public PlayerIndex GamePadIndex { get; private set; }

        public Vector2 Position { get; private set; }
        public Vector2 DeltaPosition { get; private set; }

        public GamePadMovedEventsArgs()
            : base(new Exception(), false, null)
        {
            ButtonPressed = Buttons.BigButton;
            GamePadIndex = PlayerIndex.One;
            Position = Vector2.Zero;
            Position = Vector2.Zero;
        }
        public GamePadMovedEventsArgs(Buttons buttonPressed, PlayerIndex playerIndex, Vector2 position, Vector2 deltaPosition)
            : base(new Exception(), false, null)
        {
            ButtonPressed = buttonPressed;
            GamePadIndex = playerIndex;
            Position = position;
            Position = deltaPosition;
        }
        public GamePadMovedEventsArgs(Buttons buttonPressed, PlayerIndex playerIndex, Vector2 position, Vector2 deltaPosition, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            ButtonPressed = buttonPressed;
            GamePadIndex = playerIndex;
            Position = position;
            Position = deltaPosition;
        }
    }

}
