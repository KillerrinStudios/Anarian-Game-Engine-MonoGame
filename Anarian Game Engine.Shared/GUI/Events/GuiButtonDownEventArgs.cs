using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.GUI.Events
{
    public delegate void GuiButtonPressedEventHandler(object sender, GuiButtonPressedEventArgs e);
    public delegate void GuiButtonDownEventHandler(object sender, GuiButtonPressedEventArgs e);

    public class GuiButtonPressedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        public GuiButtonPressedEventArgs()
            : base(new Exception(), false, null)
        {
        }

        public GuiButtonPressedEventArgs(Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {

        }
    }
}
