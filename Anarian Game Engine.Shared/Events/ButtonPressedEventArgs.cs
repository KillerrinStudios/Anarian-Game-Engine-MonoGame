using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void ButtonDownEventHandler(object sender, ButtonPressedEventArgs e);
    public delegate void ButtonPressedEventHandler(object sender, ButtonPressedEventArgs e);

    public class ButtonPressedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        public ButtonPressedEventArgs()
            : base(new Exception(), false, null)
        {

        }
        public ButtonPressedEventArgs(Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {

        }
    }
}
