using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void PointerPressedEventHandler(object sender, PointerPressedEventArgs e);
    public delegate void PointerDownEventHandler(object sender, PointerPressedEventArgs e);

    public class PointerPressedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public int ID { get; private set; }
        public float Pressure { get; private set; }
        public PointerPress Pointer { get; private set; }

        public Vector2 Position { get; private set; }
        public Vector2 DeltaPosition { get; private set; }

        public PointerPressedEventArgs()
            : base(new Exception(), false, null)
        {
            Pointer = PointerPress.None;
            Position = new Vector2(-1.0f, -1.0f);
            DeltaPosition = Vector2.Zero;
            ID = -1;
            Pressure = 0.0f;

        }

        #region Mouse
        /// <summary>
        /// Mouse Event Args
        /// </summary>
        public PointerPressedEventArgs(PointerPress pointerPress, Vector2 mousePosition, Vector2 deltaPosition)
            : base(new Exception(), false, null)
        {
            Pointer = pointerPress;
            Position = mousePosition;
            DeltaPosition = deltaPosition;
            SetupMouse();
        }

        /// <summary>
        /// Mouse Event Args
        /// </summary>
        public PointerPressedEventArgs(PointerPress pointerPress, Vector2 mousePosition, Vector2 deltaPosition, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            Pointer = pointerPress;
            Position = mousePosition;
            DeltaPosition = deltaPosition;
            SetupMouse();
        }

        private void SetupMouse()
        {
            ID = 0;
            Pressure = 1.0f;
        }
        #endregion

        #region Touch
        /// <summary>
        /// Touch Event Args
        /// </summary>
        public PointerPressedEventArgs(int id, PointerPress pointerPress, Vector2 mousePosition, Vector2 deltaPosition, float pressure)
            : base(new Exception(), false, null)
        {
            ID = id;
            Pointer = pointerPress;
            Position = mousePosition;
            DeltaPosition = deltaPosition;
            Pressure = pressure;
        }

        /// <summary>
        /// Touch Event Args
        /// </summary>
        public PointerPressedEventArgs(int id, PointerPress pointerPress, Vector2 mousePosition, Vector2 deltaPosition, float pressure, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            ID = id;
            Pointer = pointerPress;
            Position = mousePosition;
            DeltaPosition = deltaPosition;
            Pressure = pressure;
        }
        #endregion

        public override string ToString()
        {
            return "ID: " + ID + ", " +
                   "PointerPress: " + Pointer.ToString() + ", " +
                   "Position: " + Position.ToString() + ", " +
                   "DeltaPosition: " + DeltaPosition.ToString() + ", " +
                   "Pressure: " + Pressure;
        }
    }
}
