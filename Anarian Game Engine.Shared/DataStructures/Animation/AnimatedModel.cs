using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using Anarian.DataStructures.Animation.Aux;

namespace Anarian.DataStructures.Animation
{
    /// <summary>
    /// An encloser for an XNA model that we will use that includes support for
    /// bones, animation, and some manipulations.
    /// </summary>
    public class AnimatedModel
    {
        #region Fields
        /// <summary>
        /// The model asset name
        /// </summary>
        private string m_assetName = "";

        /// <summary>
        /// The actual underlying XNA model
        /// </summary>
        private Model model = null;

        private ModelExtra m_modelExtra = null;
        public ModelExtra ModelExtra { get { return m_modelExtra; } }

        /// <summary>
        /// The Default Animation State for this Model
        /// </summary>
        private AnimationState m_animationState;
        #endregion

        #region Properties
        /// <summary>
        /// The asset name of the Animated Model
        /// </summary>
        public string AssetName { get { return m_assetName; } }

        /// <summary>
        /// The actual underlying XNA model
        /// </summary>
        public Model Model { get { return model; } }

        public AnimationState AnimationState { get { return m_animationState; } }

        /// <summary>
        /// The model animation clips
        /// </summary>
        public List<AnimationClip> Clips { get { return m_modelExtra.Clips; } }

        #endregion

        /// <summary>
        /// Animated Model wrapper to convert AnimationAux.AnimatedModel data structures across cross assemblies
        /// </summary>
        /// <param name="oldModel">A Loaded model from the AnimationAux.AnimatedModel namespace</param>
        public AnimatedModel(AnimationAux.AnimatedModel oldModel)
        {
            m_assetName = oldModel.AssetName;

            model = oldModel.Model;
            m_modelExtra = new ModelExtra(oldModel.ModelExtra);

            Setup();
        }

        public void Setup()
        {
            // Set the Default Animation State
            m_animationState = new AnimationState(this);
        }

        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip)
        {
            return m_animationState.PlayClip(clip);
        }

        #region Operator Overloads
        public static implicit operator Model(AnimatedModel animatedModel)
        {
            return animatedModel.Model;
        }
        #endregion

        /// <summary>
        /// Draws the Model to the screen
        /// </summary>
        /// <param name="graphics">The graphics device to draw on</param>
        /// <param name="view"> The Camera View</param>
        /// <param name="projection">The Camera Projection</param>
        /// <param name="world">The World Matrix of the Model</param>
        /// <param name="animationState">An Instanced Animation State to render the animation using. Set to null to use the models Default AnimationState</param>
        /// <remarks>Warning: Drawing using the Default AnimationState will effect every object drawn with the default.</remarks>
        public void Draw(GraphicsDevice graphics, Matrix view, Matrix projection, Matrix world, AnimationState animationState)
        {
            if (model == null)
                return;

            // Save the variables out of the animation state
            List<Bone> bones;
            ModelExtra modelExtra;
            AnimationPlayer animationPlayer;

            // Determine whether to use the default AnimationState or the Instanced one
            if (animationState == null) {
                bones = m_animationState.Bones;
                modelExtra = m_animationState.ModelExtra;
                animationPlayer = m_animationState.AnimationPlayer;
            }
            else {
                bones = animationState.Bones;
                modelExtra = animationState.ModelExtra;
                animationPlayer = animationState.AnimationPlayer;
            }

            // Now that we have determined which AnimationState to use, we can begin animating the model

            // Compute all of the bone absolute transforms
            Matrix[] boneTransforms = new Matrix[bones.Count];

            for (int i = 0; i < bones.Count; i++) {
                Bone bone = bones[i];
                bone.ComputeAbsoluteTransform();

                boneTransforms[i] = bone.AbsoluteTransform;
            }

            // Determine the skin transforms from the skeleton
            Matrix[] skeleton = new Matrix[modelExtra.Skeleton.Count];
            for (int s = 0; s < modelExtra.Skeleton.Count; s++) {
                Bone bone = bones[modelExtra.Skeleton[s]];
                skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
            }

            // Draw the model.
            foreach (ModelMesh modelMesh in model.Meshes) {
                foreach (Effect effect in modelMesh.Effects) {
                    if (effect is BasicEffect) {
                        BasicEffect beffect = effect as BasicEffect;
                        beffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
                        beffect.View = view;
                        beffect.Projection = projection;
                        beffect.EnableDefaultLighting();
                        beffect.PreferPerPixelLighting = true;
                    }

                    if (effect is SkinnedEffect) {
                        SkinnedEffect seffect = effect as SkinnedEffect;
                        seffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
                        seffect.View = view;
                        seffect.Projection = projection;
                        seffect.EnableDefaultLighting();
                        seffect.PreferPerPixelLighting = true;
                        seffect.SetBoneTransforms(skeleton);
                    }
                }

                modelMesh.Draw();
            }
        }
    }
}
