using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Anarian.Interfaces;
using Anarian.Events;

namespace Anarian.DataStructures.Input
{
    public class Controller : IUpdatable
    {
        PlayerIndex m_playerIndex;
        GamePadType m_gamePadType;

        GamePadCapabilities m_gamePadCapabilities;

        GamePadState m_gamePadState;
        GamePadState m_prevGamePadState;

        bool m_isConnected;

        public PlayerIndex PlayerIndex { get { return m_playerIndex; } }
        public GamePadType GamePadType { get { return m_gamePadType; } }
        public GamePadCapabilities GamePadCapabilities { get { return m_gamePadCapabilities; } }
        public GamePadState GamePadState { get { return m_gamePadState; } }
        public GamePadState PrevGamePadState { get { return m_prevGamePadState; } }
        public bool IsConnected { get { return m_isConnected; } }

        public Controller(PlayerIndex playerIndex)
        {
            m_playerIndex = playerIndex;
            Reset();
        }

        public void Reset()
        {
            m_gamePadCapabilities = GamePad.GetCapabilities(m_playerIndex);
            m_gamePadType = m_gamePadCapabilities.GamePadType;

            m_gamePadState = GamePad.GetState(m_playerIndex);
            m_prevGamePadState = m_gamePadState;

            m_isConnected = m_gamePadState.IsConnected;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public void Update(GameTime gameTime)
        {
            //if (!m_isConnected) return;

            // Get the States
            m_prevGamePadState = m_gamePadState;
            m_gamePadState = GamePad.GetState(m_playerIndex);

            // Update if it is connected or not
            m_isConnected = m_gamePadState.IsConnected;

            #region Preform Events
            if (GamePadDown != null) {
                if (IsButtonDown(Buttons.A)) { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.A, PlayerIndex)); }
                if (IsButtonDown(Buttons.B)) { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.B, PlayerIndex)); }
                if (IsButtonDown(Buttons.X)) { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.X, PlayerIndex)); }
                if (IsButtonDown(Buttons.Y)) { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.Y, PlayerIndex)); }

                if (IsButtonDown(Buttons.LeftShoulder))     { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.LeftShoulder, PlayerIndex)); }
                if (IsButtonDown(Buttons.RightShoulder))    { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.RightShoulder, PlayerIndex)); }

                if (IsButtonDown(Buttons.LeftStick))    { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.LeftStick, PlayerIndex)); }
                if (IsButtonDown(Buttons.RightStick))   { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.RightStick, PlayerIndex)); }

                if (IsButtonDown(Buttons.DPadUp))       { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.DPadUp, PlayerIndex)); }
                if (IsButtonDown(Buttons.DPadDown))     { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.DPadDown, PlayerIndex)); }
                if (IsButtonDown(Buttons.DPadLeft))     { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.DPadLeft, PlayerIndex)); }
                if (IsButtonDown(Buttons.DPadRight))    { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.DPadRight, PlayerIndex)); }
            }

            if (GamePadClicked != null) {
                if (ButtonPressed(Buttons.A)) { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.A, PlayerIndex)); }
                if (ButtonPressed(Buttons.B)) { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.B, PlayerIndex)); }
                if (ButtonPressed(Buttons.X)) { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.X, PlayerIndex)); }
                if (ButtonPressed(Buttons.Y)) { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.Y, PlayerIndex)); }

                if (ButtonPressed(Buttons.LeftShoulder))    { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.LeftShoulder, PlayerIndex)); }
                if (ButtonPressed(Buttons.RightShoulder))   { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.RightShoulder, PlayerIndex)); }

                if (ButtonPressed(Buttons.LeftStick))   { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.LeftStick, PlayerIndex)); }
                if (ButtonPressed(Buttons.RightStick))  { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.RightStick, PlayerIndex)); }

                if (ButtonPressed(Buttons.DPadUp))      { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.DPadUp, PlayerIndex)); }
                if (ButtonPressed(Buttons.DPadDown))    { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.DPadDown, PlayerIndex)); }
                if (ButtonPressed(Buttons.DPadLeft))    { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.DPadLeft, PlayerIndex)); }
                if (ButtonPressed(Buttons.DPadRight))   { GamePadClicked(this, new GamePadPressedEventArgs(gameTime, Buttons.DPadRight, PlayerIndex)); }
            }

            if (GamePadMoved != null) {
                if (HasLeftThumbstickMoved()) {
                    GamePadMoved(this,
                                 new GamePadMovedEventsArgs(
                                     gameTime,
                                     Buttons.LeftStick,
                                     PlayerIndex,
                                     m_gamePadState.ThumbSticks.Left,
                                     m_prevGamePadState.ThumbSticks.Left - m_gamePadState.ThumbSticks.Left)
                                     );
                }
                if (HasRightThumbstickMoved()) {
                    GamePadMoved(this,
                                 new GamePadMovedEventsArgs(
                                     gameTime,
                                     Buttons.RightStick,
                                     PlayerIndex,
                                     m_gamePadState.ThumbSticks.Right,
                                     m_prevGamePadState.ThumbSticks.Right - m_gamePadState.ThumbSticks.Right)
                                     );
                }

                if (HasLeftTriggerMoved()) {
                    GamePadMoved(this,
                                 new GamePadMovedEventsArgs(
                                     gameTime,
                                     Buttons.LeftTrigger,
                                     PlayerIndex,
                                     new Vector2(0.0f, m_gamePadState.Triggers.Left),
                                     new Vector2(0.0f, m_prevGamePadState.Triggers.Left) - new Vector2(0.0f, m_gamePadState.Triggers.Left))
                                     );
                }
                if (HasRightTriggerMoved()) {
                    GamePadMoved(this,
                                 new GamePadMovedEventsArgs(
                                     gameTime,
                                     Buttons.RightTrigger,
                                     PlayerIndex,
                                     new Vector2(0.0f, m_gamePadState.Triggers.Right),
                                     new Vector2(0.0f, m_prevGamePadState.Triggers.Right) - new Vector2(0.0f, m_gamePadState.Triggers.Right))
                                     );
                }
            }
            #endregion
        }

        #region Helper Methods
        public bool ButtonPressed(Buttons button)
        {
            if (m_prevGamePadState.IsButtonDown(button) == true &&
                m_gamePadState.IsButtonUp(button) == true)
                return true;
            return false;
        }

        public bool IsButtonDown(Buttons button)
        {
            return m_gamePadState.IsButtonDown(button);
        }
        public bool IsButtonUp(Buttons button)
        {
            return m_gamePadState.IsButtonUp(button);
        }

        public void SetVibration(float leftMotor, float rightMotor)
        {
            GamePad.SetVibration(m_playerIndex, leftMotor, rightMotor);
        }

        #region Thumbsticks
        public bool HasLeftThumbstickMoved()
        {
            if (m_gamePadState.ThumbSticks.Left == Vector2.Zero) return false;
            return true;
        }
        public bool HasRightThumbstickMoved()
        {
            if (m_gamePadState.ThumbSticks.Right == Vector2.Zero) return false;
            return true;
        }
        #endregion

        #region Triggers
        public bool HasLeftTriggerMoved()
        {
            if (m_gamePadState.Triggers.Left == 0.0f) return false;
            return true;
        }
        public bool HasRightTriggerMoved()
        {
            if (m_gamePadState.Triggers.Right == 0.0f) return false;
            return true;
        }
        #endregion


        public bool HasInputChanged(bool ignoreThumbsticks)
        { 
            if ((m_gamePadState.IsConnected) && (m_gamePadState.PacketNumber != m_prevGamePadState.PacketNumber))
            {
                //ignore thumbstick movement
                if ((ignoreThumbsticks == true) && ((m_gamePadState.ThumbSticks.Left.Length() != m_prevGamePadState.ThumbSticks.Left.Length()) && (m_gamePadState.ThumbSticks.Right.Length() != m_prevGamePadState.ThumbSticks.Right.Length())))
                    return false;
                return true;
            }
            return false;
        }
        #endregion

        #region Events
        public static event GamePadDownEventHandler GamePadDown;
        public static event GamePadPressedEventHandler GamePadClicked;
        public static event GamePadMovedEventHandler GamePadMoved;
        #endregion
    }
}
