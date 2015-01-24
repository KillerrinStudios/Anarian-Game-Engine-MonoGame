using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.Enumerators;
using Microsoft.Xna.Framework.Input.Touch;

namespace Anarian.Events
{
    public delegate void TouchGestureEventHandler(object sender, TouchGestureEventArgs e);

    public class TouchGestureEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public int ID { get; private set; }
        public InputType InputType { get; private set; }
        public GestureSample Gesture { get; private set; }

        public TouchGestureEventArgs()
            : base(new Exception(), false, null)
        {
            ID = -1;
            InputType = Enumerators.InputType.None;
            Gesture = new GestureSample();
        }

        public TouchGestureEventArgs(GestureSample gesture)
            : base(new Exception(), false, null)
        {
            ID = 0;
            InputType = Enumerators.InputType.Touch;
            Gesture = gesture;
        }

        public TouchGestureEventArgs(GestureSample gesture, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            ID = 0;
            InputType = Enumerators.InputType.Touch;
            Gesture = gesture;
        }

        public override string ToString()
        {
            return "ID: " + ID + ", " +
                   "PointerPress: " + InputType.ToString() + ", " +
                   "Gesture: " + Gesture.ToString();
        }
    }
}
