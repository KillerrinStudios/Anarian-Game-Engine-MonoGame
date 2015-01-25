using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Events
{
    public delegate void AnarianEventHandler(object sender, AnarianEventArgs e);

    public class AnarianEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public GameTime GameTime { get; protected set; }

        public AnarianEventArgs(GameTime gameTime)
            : base(new Exception(), false, null)
        {
            GameTime = gameTime;
        }
        public AnarianEventArgs(GameTime gameTime, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            GameTime = gameTime;
        }
    }
}
