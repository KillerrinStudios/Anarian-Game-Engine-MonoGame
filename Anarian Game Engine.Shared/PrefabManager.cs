using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Anarian.DataStructures.Animation;
using Anarian.Helpers;
using Anarian.DataStructures;

namespace Anarian
{
    public class PrefabManager
    {
        #region Singleton
        static PrefabManager m_instance;
        public static PrefabManager Instance
        {
            get {
                if (m_instance == null) m_instance = new PrefabManager();
                return m_instance;
            }
            set { }
        }
        #endregion

        Dictionary<string, GameObject> m_prefabs;
        Dictionary<string, AnimatedModel> m_animations;

        private PrefabManager()
        {
            m_prefabs = new Dictionary<string, GameObject>();
            m_animations = new Dictionary<string, AnimatedModel>();
        }

        public void AddPrefab(GameObject prefab, string key)
        {
            Debug.WriteLine("Adding Prefab: " + key);
            m_prefabs.Add(key, prefab);
        }

        public void RemovePrefab(string key)
        {
            Debug.WriteLine("Removing Prefab: " + key);
            m_prefabs.Remove(key);
        }

        public GameObject GetPrefab(string key)
        {
            Debug.WriteLine("Getting Prefab: " + key);
            return m_prefabs[key];
        }

        public bool PrefabExists(string key)
        {
            return m_prefabs.ContainsKey(key);
        }
    }
}
