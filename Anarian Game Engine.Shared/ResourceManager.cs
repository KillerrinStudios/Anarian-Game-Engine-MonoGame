using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Anarian.DataStructures.Animation;
using Anarian.Helpers;

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

        Dictionary<string, Texture2D> m_textures;
        Dictionary<string, Model> m_models;
        Dictionary<string, AnimatedModel> m_animatedModels;

        private ResourceManager()
        {
            m_textures = new Dictionary<string, Texture2D>();
            m_models = new Dictionary<string, Model>();
            m_animatedModels = new Dictionary<string, AnimatedModel>();
        }

        private string AssetName(string assetName)
        {
            char[] splitChars = {'/', '\\' };
            string[] assetNameSplit = assetName.Split(splitChars);
            return assetNameSplit[assetNameSplit.Length - 1];
        }

        public void LoadAsset(ContentManager Content, Type assetType, string assetName)
        {
            Debug.WriteLine("Loading Asset: " + assetName + " | " + assetType.Name);
            if (assetType == typeof(Texture2D)) { m_textures.Add(AssetName(assetName), Content.Load<Texture2D>(assetName)); }
            else if (assetType == typeof(Model)) { m_models.Add(AssetName(assetName), Content.Load<Model>(assetName)); }
            else if (assetType == typeof(AnimatedModel)) { m_animatedModels.Add(AssetName(assetName), CustomContentLoader.LoadAnimatedModel(Content, assetName)); }
        }

        public void AddAsset(object asset, string assetName)
        {
            Type assetType = asset.GetType();
            Debug.WriteLine("Adding Asset: " + assetName + " | " + assetType.Name);

            if (assetType == typeof(Texture2D)) { m_textures.Add(AssetName(assetName), (Texture2D)asset); }
            else if (assetType == typeof(Model)) { m_models.Add(AssetName(assetName), (Model)asset); }
            else if (assetType == typeof(AnimatedModel)) { m_animatedModels.Add(AssetName(assetName), (AnimatedModel)asset); }
        }

        public object GetAsset(Type assetType, string key)
        {
            Debug.WriteLine("Getting Asset: " + key + " | " + assetType.Name);

            if (assetType == typeof(Texture2D)) { return m_textures[key]; }
            else if (assetType == typeof(Model)) { return m_models[key]; }
            else if (assetType == typeof(AnimatedModel)) { return m_animatedModels[key]; }

            return null;
        }

        public static class EngineReservedAssetNames
        {
            public static string blankTextureName = "blankTexture_age";
        }
    }
}
