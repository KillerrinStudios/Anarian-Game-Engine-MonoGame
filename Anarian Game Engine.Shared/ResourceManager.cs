using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Anarian.DataStructures.Animation;
using Anarian.Helpers;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Anarian
{
    public class ResourceManager
    {
        #region Singleton
        static ResourceManager m_instance;
        public static ResourceManager Instance
        {
            get {
                if (m_instance == null) m_instance = new ResourceManager();
                return m_instance;
            }
            set { }
        }
        #endregion

        Dictionary<string, SpriteFont> m_spriteFonts;

        Dictionary<string, Song> m_songs;
        Dictionary<string, SoundEffect> m_soundEffects;

        Dictionary<string, Texture2D> m_texture2Ds;

        Dictionary<string, Model> m_models;
        Dictionary<string, AnimatedModel> m_animatedModels;

        private ResourceManager()
        {
            m_spriteFonts = new Dictionary<string, SpriteFont>();

            m_songs = new Dictionary<string, Song>();
            m_soundEffects = new Dictionary<string, SoundEffect>();

            m_texture2Ds = new Dictionary<string, Texture2D>();

            m_models = new Dictionary<string, Model>();
            m_animatedModels = new Dictionary<string, AnimatedModel>();
        }

        private string AssetName(string assetName)
        {
            char[] splitChars = {'/', '\\' };
            string[] assetNameSplit = assetName.Split(splitChars);
            return assetNameSplit[assetNameSplit.Length - 1];
        }

        /// <summary>
        /// Loads and asset and adds it to the manager
        /// </summary>
        /// <param name="Content">The ContentManager</param>
        /// <param name="assetType">The type of the asset to Load, ex. Texture2D, Model, AnimatedModel</param>
        /// <param name="assetName">The name of the asset to load</param>
        /// <param name="key">If not supplied, key set to assetName</param>
        /// <returns>The Loaded Asset</returns>
        public object LoadAsset(ContentManager Content, Type assetType, string assetName, string key = "")
        {
            Debug.WriteLine("Loading Asset: " + assetName + " | " + assetType.Name);

            if (string.IsNullOrWhiteSpace(key)) key = AssetName(assetName);
            object loadedAsset = null;

            if (assetType == typeof(Texture2D)) { loadedAsset = Content.Load<Texture2D>(assetName); m_texture2Ds.Add(key, (Texture2D)loadedAsset); }

            else if (assetType == typeof(Model)) { loadedAsset = Content.Load<Model>(assetName); m_models.Add(key, (Model)loadedAsset); }
            else if (assetType == typeof(AnimatedModel)) { loadedAsset = CustomContentLoader.LoadAnimatedModel(Content, assetName); m_animatedModels.Add(key, (AnimatedModel)loadedAsset); }

            else if (assetType == typeof(SpriteFont)) { loadedAsset = Content.Load<SpriteFont>(assetName); m_spriteFonts.Add(key, (SpriteFont)loadedAsset); }

            else if (assetType == typeof(Song)) { loadedAsset = Content.Load<Song>(assetName); m_songs.Add(key, (Song)loadedAsset); }
            else if (assetType == typeof(SoundEffect)) { loadedAsset = Content.Load<SoundEffect>(assetName); m_soundEffects.Add(key, (SoundEffect)loadedAsset); }

            return loadedAsset;
        }

        public void AddAsset(object asset, string assetName)
        {
            Type assetType = asset.GetType();
            Debug.WriteLine("Adding Asset: " + assetName + " | " + assetType.Name);

            if (assetType == typeof(Texture2D)) { m_texture2Ds.Add(AssetName(assetName), (Texture2D)asset); }

            else if (assetType == typeof(Model)) { m_models.Add(AssetName(assetName), (Model)asset); }
            else if (assetType == typeof(AnimatedModel)) { m_animatedModels.Add(AssetName(assetName), (AnimatedModel)asset); }

            else if (assetType == typeof(SpriteFont)) { m_spriteFonts.Add(AssetName(assetName), (SpriteFont)asset); }

            else if (assetType == typeof(Song)) { m_songs.Add(AssetName(assetName), (Song)asset); }
            else if (assetType == typeof(SoundEffect)) { m_soundEffects.Add(AssetName(assetName), (SoundEffect)asset); }
        }

        public object GetAsset(Type assetType, string key)
        {
            Debug.WriteLine("Getting Asset: " + key + " | " + assetType.Name);

            if (assetType == typeof(Texture2D)) { return m_texture2Ds[key]; }

            else if (assetType == typeof(Model)) { return m_models[key]; }
            else if (assetType == typeof(AnimatedModel)) { return m_animatedModels[key]; }

            else if (assetType == typeof(SpriteFont)) { return m_spriteFonts[key]; }

            else if (assetType == typeof(Song)) { return m_songs[key]; }
            else if (assetType == typeof(SoundEffect)) { return m_soundEffects[key]; }

            return null;
        }

        public bool AssetExists(string key)
        {
            return (m_spriteFonts.ContainsKey(key) ||

                    m_songs.ContainsKey(key) ||
                    m_soundEffects.ContainsKey(key) ||

                    m_texture2Ds.ContainsKey(key) ||

                    m_models.ContainsKey(key) ||
                    m_animatedModels.ContainsKey(key));
        }

        public static class EngineReservedAssetNames
        {
            public static string blankTextureName = "blankTexture_age";
        }
    }
}
