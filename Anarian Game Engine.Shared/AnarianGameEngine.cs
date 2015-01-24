using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Anarian;
using Anarian.DataStructures;
using Anarian.Interfaces;
using Anarian.Helpers;

namespace Anarian
{
    public class AnarianGameEngine : Game
    {
        #region Fields/Properties
        #region MonoGame
        protected GraphicsDeviceManager graphics;
        public GraphicsDeviceManager Graphics { get { return graphics; } protected set { graphics = value; } }

        protected SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch { get { return spriteBatch; } protected set { spriteBatch = value; } }

        protected Color m_backgroundColor;
        public Color BackgroundColor { get { return m_backgroundColor; } set { m_backgroundColor = value; } }

        protected Random m_random;
        public Random Random { get { return m_random; } protected set { m_random = value; } }
        #endregion

        #region Managers
        protected SceneManager m_sceneManager;
        public SceneManager SceneManager { get { return m_sceneManager; } }

        protected ResourceManager m_resourceManager;
        public ResourceManager ResourceManager { get { return m_resourceManager; } }

        protected InputManager m_inputManager;
        public InputManager InputManager { get { return m_inputManager; } }

        protected GUIManager m_guiManager;
        public GUIManager GuiManager { get { return m_guiManager; } }
        #endregion

        #endregion

        public AnarianGameEngine()
            :base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Disable the Fixed TimeStep infavor of a Variable one
            IsFixedTimeStep = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_random = new System.Random();

            m_resourceManager = ResourceManager.Instance;
            m_inputManager = InputManager.Instance;
            m_sceneManager = SceneManager.Instance;
            m_guiManager = GUIManager.Instance;
        }

        /// <summary>
        /// Preforms MonoGame Initializations
        /// </summary>
        protected virtual void PostInitialize()
        {
            base.Initialize();
        }

        #region Load/Unload Content
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected virtual void LoadContent(Color backgroundColor)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Set the Background Color
            m_backgroundColor = backgroundColor;

            // Create the Scene
            m_sceneManager.CurrentScene = new Level(graphics.GraphicsDevice);

            // Create Textures which will be needed in the engine
            // Blank Texture
            Texture2D blankTexture = Color.White.CreateTextureFromSolidColor(GraphicsDevice, 1, 1);
            m_resourceManager.AddAsset(blankTexture, ResourceManager.EngineReservedAssetNames.blankTextureName);
        }

        /// <summary>
        /// Preforms MonoGame Content Load
        /// </summary>
        protected virtual void PostLoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        #endregion

        #region Updates
        /// <summary>
        /// Updates the Managers managed by the GameEngine
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void PreUpdate(GameTime gameTime)
        {
            // First, we Update the Inputs
            m_inputManager.Update(gameTime);
        }

        /// <summary>
        /// Has the GameEngine Update the SceneNodes then Update the GUI
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // First we Update the SceneNodes
            if (m_sceneManager != null) {
                m_sceneManager.Update(gameTime);
            }

            // Now we Update the GUI
            m_guiManager.Update(gameTime);
        }

        /// <summary>
        /// Is called after Updating is completed to advance the Game to Draw
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        protected virtual void PostUpdate(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Clears the Screen to the specified Background Color
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        protected virtual void PreDraw(GameTime gameTime)
        {
            GraphicsDevice.Clear(m_backgroundColor);
        }

        /// <summary>
        /// Has the GameEngine Render the SceneNodes then Draw the GUI
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // First we Render the Scene
            if (m_sceneManager != null) {
                m_sceneManager.Draw(gameTime, spriteBatch, graphics.GraphicsDevice);
            }

            // Then we Draw the GUI
            m_guiManager.Draw(gameTime, spriteBatch, graphics.GraphicsDevice);
        }

        /// <summary>
        /// Called after Rendering is comopleted to advance the Game back to the Update Method
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        protected virtual void PostDraw(GameTime gameTime)
        {
            // Since we don't call MonoGame.Draw on the Draw method,
            // We call it here so that the screen will render
            base.Draw(gameTime);
        }
        #endregion
    }
}
