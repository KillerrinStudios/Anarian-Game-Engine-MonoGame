using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Anarian.GUI;
using Anarian.DataStructures.Input;
using Anarian.Enumerators;
using Anarian.Events;
using Anarian.Interfaces;
using Anarian.DataStructures;

namespace Anarian
{
    public class GUIManager :IDisposable, IUpdatable, IRenderable
    {
        #region Singleton
        static GUIManager m_instance;
        public static GUIManager Instance
        {
            get
            {
                if (m_instance == null) m_instance = new GUIManager();
                return m_instance;
            }
            set { }
        }
        #endregion

        #region Fields/Properties
        public bool Initialized { get; private set; }

        public IScene2D CurrentScene { get; set; }
        #endregion

        #region Constructor
        private GUIManager()
        {
            Initialized = false;
        }

        public void Dispose()
        {
            Initialized = false;

            // Subscribe to Pointer Events
            InputManager.Instance.PointerMoved -= Instance_PointerMoved;
            InputManager.Instance.PointerPressed -= Instance_PointerPressed;
            InputManager.Instance.PointerDown -= Instance_PointerDown;

            // Subscribe to Keyboard Events
            InputManager.Instance.Keyboard.KeyboardDown -= Keyboard_KeyboardDown;
            InputManager.Instance.Keyboard.KeyboardPressed -= Keyboard_KeyboardPressed;

            // Subscribe to Controller Events
            Controller.GamePadDown -= GUIManager_GamePadDown;
            Controller.GamePadClicked -= GUIManager_GamePadClicked;
            Controller.GamePadMoved -= GUIManager_GamePadMoved;

            // Surpress the Finalize
            GC.SuppressFinalize(this);
        }
        

        public void Initialize()
        {
            // Subscribe to Pointer Events
            InputManager.Instance.PointerMoved += Instance_PointerMoved;
            InputManager.Instance.PointerPressed += Instance_PointerPressed;
            InputManager.Instance.PointerDown += Instance_PointerDown;

            // Subscribe to Keyboard Events
            InputManager.Instance.Keyboard.KeyboardDown += Keyboard_KeyboardDown;
            InputManager.Instance.Keyboard.KeyboardPressed += Keyboard_KeyboardPressed;

            // Subscribe to Controller Events
            Controller.GamePadDown += GUIManager_GamePadDown;
            Controller.GamePadClicked += GUIManager_GamePadClicked;
            Controller.GamePadMoved += GUIManager_GamePadMoved;

            // Create a Root Menu
            CurrentScene = new Menu();

            // Set the Initialization Flag
            Initialized = true;
        }
        #endregion

        #region Interfaces
        void IDisposable.Dispose() { Dispose(); }
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics); }
        #endregion

        public void Update(GameTime gameTime)
        {
            if (!Initialized) return;

            if (CurrentScene != null) {
                CurrentScene.SceneNode.GuiObject.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (!Initialized) return;

            if (CurrentScene != null) {
                CurrentScene.SceneNode.GuiObject.Draw(gameTime, spriteBatch, graphics);
            }
        }

        #region Events
        #region Pointer Events
        void Instance_PointerDown(object sender, PointerPressedEventArgs e)
        {
            if (!Initialized) return;
            if (CurrentScene != null) {
                CurrentScene.HandlePointerDown(sender, e);
            }
        }

        void Instance_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!Initialized) return;
            if (CurrentScene != null) {
                CurrentScene.HandlePointerPressed(sender, e);
            }
        }

        void Instance_PointerMoved(object sender, PointerMovedEventArgs e)
        {
            if (!Initialized) return;
            if (CurrentScene != null) {
                CurrentScene.HandlePointerMoved(sender, e);
            }
        }
        #endregion

        #region Keyboard Events
        void Keyboard_KeyboardPressed(object sender, KeyboardPressedEventArgs e)
        {
            
        }

        void Keyboard_KeyboardDown(object sender, KeyboardPressedEventArgs e)
        {

        }
        #endregion

        #region Controller Events
        void GUIManager_GamePadDown(object sender, GamePadPressedEventArgs e)
        {

        }

        void GUIManager_GamePadClicked(object sender, GamePadPressedEventArgs e)
        {

        }

        void GUIManager_GamePadMoved(object sender, GamePadMovedEventsArgs e)
        {
        }
        #endregion
        #endregion
    }
}
