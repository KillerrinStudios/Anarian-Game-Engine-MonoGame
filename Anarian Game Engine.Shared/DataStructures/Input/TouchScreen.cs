using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Anarian.Interfaces;
using Anarian.Enumerators;
using Anarian.Events;

namespace Anarian.DataStructures.Input
{
    public class TouchScreen : IUpdatable
    {
        TouchCollection m_touchCollection;
        TouchCollection m_prevTouchCollection;

        TouchPanelCapabilities m_touchPanelCapabilities;

        bool m_isConnected;
        bool m_isReadOnly;

        #region Properties
        public TouchCollection TouchCollection { get { return m_touchCollection; } }
        public TouchCollection PrevTouchCollection { get { return m_prevTouchCollection; } }
        
        public TouchPanelCapabilities TouchPanelCapabilities { get { return m_touchPanelCapabilities; } }

        public GestureType EnabledGestures {
            get { return TouchPanel.EnabledGestures; }
            set { TouchPanel.EnabledGestures = value; }
        }

        public DisplayOrientation DislayOrientation { get { return TouchPanel.DisplayOrientation; } }

        public int DisplayWidth { get { return TouchPanel.DisplayWidth; } }
        public int DisplayHeight { get { return TouchPanel.DisplayHeight; } }

        public bool IsConnected { get { return m_isConnected; } }
        public bool IsReadOnly  { get { return m_isReadOnly; } }
        public bool IsGestureAvailable { get { return TouchPanel.IsGestureAvailable; } }

        public bool EnableMouseGestures {
            get { return TouchPanel.EnableMouseGestures; }
            set { TouchPanel.EnableMouseGestures = value; }
        }
        public bool EnableMouseTouchPoint {
            get { return TouchPanel.EnableMouseTouchPoint; }
            set { TouchPanel.EnableMouseTouchPoint = value; }
        }
        #endregion

        public TouchScreen()
        {
            m_touchCollection = TouchPanel.GetState();
            m_prevTouchCollection = m_touchCollection;

            m_isConnected = m_touchCollection.IsConnected;
            m_isReadOnly = m_touchCollection.IsReadOnly;
            m_touchPanelCapabilities = TouchPanel.GetCapabilities();
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public void Update(GameTime gameTime)
        {
            if (!m_isConnected) return;

            m_prevTouchCollection = m_touchCollection;
            m_touchCollection = TouchPanel.GetState();

            // Preform Events
            if (TouchDown != null) {
                foreach (var touch in m_touchCollection) {
                    if (IsTouchDown(touch.Id)) {
                            TouchDown(this, new PointerPressedEventArgs(touch.Id, PointerPress.Touch, touch.Position, GetDeltaTouchPosition(touch.Id), touch.Pressure));
                    }
                }
            }

            if (TouchPressed != null) {
                foreach (var touch in m_touchCollection) {
                    if (WasTouchPressed(touch.Id)) {
                        TouchPressed(this, new PointerPressedEventArgs(touch.Id, PointerPress.Touch, touch.Position, GetDeltaTouchPosition(touch.Id), touch.Pressure));
                    }
                }
            }

            if (TouchMoved != null) {
                foreach (var touch in m_touchCollection) {
                    TouchLocation prevLocation;
                    bool prevLocationAvailable = touch.TryGetPreviousLocation(out prevLocation);
                    if (!prevLocationAvailable) continue;

                    if (touch.Position != prevLocation.Position) {
                        TouchMoved(this, new PointerMovedEventArgs(touch.Id, touch.Position, touch.Position - prevLocation.Position));
                    }
                }
            }

            if (OnGesture != null) {
                List<GestureSample> allGestures = ReadAllGestures();
                foreach (var gesture in allGestures) {
                    OnGesture(this, new TouchGestureEventArgs(gesture));
                }
            }
        }

        #region Helper Methods
        public bool IsTouchDown (int id)
        {
            foreach (var i in m_touchCollection) {
                if (i.Id == id) {
                    if (i.State == TouchLocationState.Pressed ||
                        i.State == TouchLocationState.Moved) {
                            return true;
                    }
                    break;
                }
            }
            return false;
        } 

        public bool WasTouchPressed (int id)
        {
            foreach (TouchLocation location in m_touchCollection) {
                if (location.Id == id) {
                    TouchLocation prevLocation;
                    bool prevLocationAvailable = location.TryGetPreviousLocation(out prevLocation);
                    if (!prevLocationAvailable) break;

                    if (location.State == TouchLocationState.Released &&
                        prevLocation.State == TouchLocationState.Pressed) {
                        return true;
                    }

                    if (location.State == TouchLocationState.Released &&
                        prevLocation.State == TouchLocationState.Moved) {
                        return true;
                    }

                    break;
                }
            }

            return false;
        }

        public GestureSample? ReadGesture()
        {
            if (TouchPanel.IsGestureAvailable) {
                return TouchPanel.ReadGesture();
            }
            return null;
        }

        public List<GestureSample> ReadAllGestures()
        {
            List<GestureSample> allGestures = new List<GestureSample>();
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                allGestures.Add(gesture);
            }

            return allGestures;
        }

        public bool DidTouchMove(int id)
        {
            foreach (TouchLocation location in m_touchCollection) {
                if (location.Id == id) {
                    TouchLocation prevLocation;
                    bool prevLocationAvailable = location.TryGetPreviousLocation(out prevLocation);
                    if (!prevLocationAvailable) break;

                    if (location.Position == prevLocation.Position) {
                        return true;
                    }

                    break;
                }
            }

            return false;
        }

        public Vector2 GetDeltaTouchPosition(int id)
        {
            foreach (TouchLocation location in m_touchCollection) {
                if (location.Id == id) {
                    TouchLocation prevLocation;
                    bool prevLocationAvailable = location.TryGetPreviousLocation(out prevLocation);
                    if (!prevLocationAvailable) break;

                    return prevLocation.Position - location.Position;
                }
            }

            return Vector2.Zero;
        }
        #endregion

        #region Events
        public event PointerDownEventHandler TouchDown;
        public event PointerPressedEventHandler TouchPressed;
        public event PointerMovedEventHandler TouchMoved;

        public event TouchGestureEventHandler OnGesture;
        #endregion

    }
}
