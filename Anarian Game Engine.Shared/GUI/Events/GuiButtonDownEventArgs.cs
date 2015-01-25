using Anarian.Events;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.GUI.Events
{
    public delegate void GuiButtonPressedEventHandler(object sender, GuiButtonPressedEventArgs e);
    public delegate void GuiButtonDownEventHandler(object sender, GuiButtonPressedEventArgs e);

    public class GuiButtonPressedEventArgs : AnarianEventArgs
    {

        public GuiButtonPressedEventArgs(GameTime gameTime)
            : base(gameTime)
        {
        }

        public GuiButtonPressedEventArgs(GameTime gameTime, Exception e, bool canceled, Object state)
            : base(gameTime, e, canceled, state)
        {

        }
    }
}
