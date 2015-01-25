using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void ButtonDownEventHandler(object sender, ButtonPressedEventArgs e);
    public delegate void ButtonPressedEventHandler(object sender, ButtonPressedEventArgs e);

    public class ButtonPressedEventArgs : AnarianEventArgs
    {

        public ButtonPressedEventArgs(GameTime gameTime)
            : base(gameTime)
        {

        }
        public ButtonPressedEventArgs(GameTime gameTime, Exception e, bool canceled, Object state)
            : base(gameTime, e, canceled, state)
        {

        }
    }
}
