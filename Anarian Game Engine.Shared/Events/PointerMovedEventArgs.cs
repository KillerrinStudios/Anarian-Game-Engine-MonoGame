using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void PointerMovedEventHandler(object sender, PointerMovedEventArgs e);

    public class PointerMovedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public int ID { get; private set; }
        public InputType InputType { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 DeltaPosition { get; private set;}

        public PointerMovedEventArgs()
            : base(new Exception(), false, null)
        {
            Position = new Vector2(-1.0f, -1.0f);
            DeltaPosition = Vector2.Zero;
            ID = -1;
            InputType = Enumerators.InputType.None;
        }

        #region Mouse
        public PointerMovedEventArgs(Vector2 mousePosition, Vector2 deltaMousePosition)
            : base(new Exception(), false, null)
        {
            Position = mousePosition;
            DeltaPosition = deltaMousePosition;
            SetupMouse();
        }
        public PointerMovedEventArgs(Vector2 mousePosition, Vector2 deltaMousePosition, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            Position = mousePosition;
            DeltaPosition = deltaMousePosition;
            SetupMouse();
        }

        private void SetupMouse()
        {
            ID = 0;
            InputType = Enumerators.InputType.Mouse;
        }
        #endregion

        #region Touch
        public PointerMovedEventArgs(int id, Vector2 mousePosition, Vector2 deltaMousePosition)
            : base(new Exception(), false, null)
        {
            ID = id;
            InputType = Enumerators.InputType.Touch;
            Position = mousePosition;
            DeltaPosition = deltaMousePosition;
        }
        public PointerMovedEventArgs(int id, Vector2 mousePosition, Vector2 deltaMousePosition, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            ID = id;
            InputType = Enumerators.InputType.Touch;
            Position = mousePosition;
            DeltaPosition = deltaMousePosition;
        }
        #endregion

        public override string ToString()
        {
            return "ID: " + ID + ", " +
                   "PointerPress: " + InputType.ToString() + ", " +
                   "Position: " + Position.ToString() + ", " +
                   "DeltaPosition: " + DeltaPosition.ToString();
        }
    }
}
