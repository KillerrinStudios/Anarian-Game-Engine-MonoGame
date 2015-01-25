using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void KeyboardPressedEventHandler(object sender, KeyboardPressedEventArgs e);
    public delegate void KeyboardDownEventHandler(object sender, KeyboardPressedEventArgs e);

    public class KeyboardPressedEventArgs : AnarianEventArgs
    {
        public Keys KeyClicked { get; private set; }

        public KeyboardPressedEventArgs(GameTime gameTime)
            : base(gameTime)
        {
            KeyClicked = Keys.None;
        }
        public KeyboardPressedEventArgs(GameTime gameTime, Keys keyboardKeyClicked)
            : base(gameTime)
        {
            KeyClicked = keyboardKeyClicked;
        }
        public KeyboardPressedEventArgs(GameTime gameTime, Keys keyboardKeyClicked, Exception e, bool canceled, Object state)
            : base(gameTime, e, canceled, state)
        {
            KeyClicked = keyboardKeyClicked;
        }
    }
}
