using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Anarian.DataStructures.Animation.Aux;
using Anarian.Enumerators;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Anarian.DataStructures.Animation
{
    public class AnimationState : IUpdatable
    {
        #region Fields/Properties
        AnimationPlayer m_animationPlayer;
        public AnimationPlayer AnimationPlayer
        {
            get { return m_animationPlayer; }
            set { m_animationPlayer = value; }
        }

        ModelExtra m_modelExtra;
        public ModelExtra ModelExtra
        {
            get { return m_modelExtra; }
            set { m_modelExtra = value; }
        }

        List<Bone> m_bones;
        public List<Bone> Bones 
        { 
            get { return m_bones; }
            set { m_bones = value; }
        }

        public List<AnimationClip> Clips { get { return m_modelExtra.Clips; } }

        AnimatedModel m_animatedModel;
        #endregion

        public AnimationState()
        {
            m_animationPlayer = null;
            m_modelExtra = null;
            m_bones = new List<Bone>();
            m_animatedModel = null;
        }
        public AnimationState(AnimatedModel animatedModel)
        {
            m_animationPlayer = null;

            m_modelExtra = new ModelExtra(animatedModel.ModelExtra);

            System.Diagnostics.Debug.Assert(m_modelExtra != null);
            m_bones = new List<Bone>();
            ObtainBones(animatedModel.Model);

            // Cache the Animated Model
            m_animatedModel = animatedModel;
        }

        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip)
        {
            // Create a clip player and assign it to this model
            m_animationPlayer = new AnimationPlayer(clip, this);
            return m_animationPlayer;
        }

        #region Helper Methods
        private void ObtainBones(Model model)
        {
            m_bones.Clear();
            foreach (ModelBone bone in model.Bones) {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? m_bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                m_bones.Add(newBone);
            }

            //System.Diagnostics.Debug.WriteLine("{0}: Total Bones {1}", assetName, bones.Count);
        }

        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones) {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }
        #endregion

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public void Update(GameTime gameTime)
        {
            if (m_animationPlayer != null) m_animationPlayer.Update(gameTime);
        }
    }
}
