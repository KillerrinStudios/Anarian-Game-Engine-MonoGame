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
    public class KeyboardManager : IUpdatable
    {
        #region Fields and Properties
        KeyboardState m_keyboardState;
        KeyboardState m_prevKeyboardState;

        public KeyboardState KeyboardState { get { return m_keyboardState; } }
        public KeyboardState PrevKeyboardState { get { return m_prevKeyboardState; } }
        #endregion

        public KeyboardManager()
        {
            m_keyboardState = Keyboard.GetState();
            m_prevKeyboardState = m_keyboardState;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public void Update(GameTime gameTime)
        {
            m_prevKeyboardState = m_keyboardState;
            m_keyboardState = Keyboard.GetState();

            // If we have subscribers to the events, call them now
            if (KeyboardDown != null) {
                Keys[] keysDown = m_keyboardState.GetPressedKeys();
                for (int i = 0; i < keysDown.Length; i++) {
                    KeyboardDown(this, new KeyboardPressedEventArgs(keysDown[i]));
                }
            }

            if (KeyboardPressed != null) {
                Keys[] prevKeysDown = m_prevKeyboardState.GetPressedKeys();
                for (int i = 0; i < prevKeysDown.Length; i++)
                    if (KeyPressed(prevKeysDown[i]))
                        KeyboardPressed(this, new KeyboardPressedEventArgs(prevKeysDown[i]));
            }
        }

        #region Helper Methods
        public bool KeyPressed(Keys key)
        {
            if (m_prevKeyboardState.IsKeyDown(key) == true &&
                m_keyboardState.IsKeyUp(key) == true)
                return true;
            return false;
        }

        public bool IsKeyDown(Keys key)
        {
            return m_keyboardState.IsKeyDown(key);
        }
        public bool IsKeyUp(Keys key)
        {
            return m_keyboardState.IsKeyUp(key);
        }
        #endregion

        #region Events
        public event KeyboardDownEventHandler KeyboardDown;
        public event KeyboardPressedEventHandler KeyboardPressed;
        #endregion
    }
}
