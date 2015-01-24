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

    public class KeyboardPressedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public Keys KeyClicked { get; private set; }

        public KeyboardPressedEventArgs()
            : base(new Exception(), false, null)
        {
            KeyClicked = Keys.None;
        }
        public KeyboardPressedEventArgs(Keys keyboardKeyClicked)
            : base(new Exception(), false, null)
        {
            KeyClicked = keyboardKeyClicked;
        }
        public KeyboardPressedEventArgs(Keys keyboardKeyClicked, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            KeyClicked = keyboardKeyClicked;
        }
    }
}
