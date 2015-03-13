using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Anarian.DataStructures.Animation;
using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using Anarian.Helpers;
using Anarian.DataStructures.Animation.Aux;
using System.Runtime.CompilerServices;

namespace Anarian.DataStructures
{
    public class AnimatedGameObject : GameObject, IUpdatable, IRenderable
    {
        #region Fields/Properties
        protected AnimatedModel m_model;
        public AnimatedModel Model3D
        {
            get { return m_model; }
            set 
            { 
                m_model = value;
                CreateAnimationState();
                CreateBounds();
            }
        }

        protected AnimationState m_animationState;
        public AnimationState AnimationState
        {
            get { return m_animationState; }
            set { m_animationState = value; }
        }
        public void CreateAnimationState() { if (m_model != null) m_animationState = new AnimationState(m_model); }
        #endregion

        public AnimatedGameObject()
            :base()
        {
        }

        public override void CreateBounds()
        {
            base.CreateBounds();

            // Get the ModelTransforms
            Matrix[] modelTransforms = new Matrix[m_animationState.Bones.Count];
            Model3D.Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Check intersection
            foreach (ModelMesh mesh in Model3D.Model.Meshes)
            {
                var boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix);
                m_boundingSpheres.Add(boundingSphere);
            }
        }

        public override bool CheckRayIntersection(Ray ray)
        {
            // Create the ModelTransforms
            Matrix[] modelTransforms = new Matrix[m_animationState.Bones.Count];
            Model3D.Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Check intersection
            foreach (ModelMesh mesh in Model3D.Model.Meshes) {

                var boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix);
                if (ray.Intersects(boundingSphere).HasValue) return true;
            }
            return false;
        }


        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        #region Animation Helpers
        public virtual AnimationPlayer PlayClip(AnimationClip clip) {
            return m_animationState.PlayClip(clip);
        }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;
            
            // We first Update the Children
            base.Update(gameTime);
            
            // Now we update the Animation
            m_animationState.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (!m_active) return;

            // We Draw the base here so that the Children get taken care of
            base.Draw(gameTime, spriteBatch, graphics, camera);

            // Now that the children have been rendered...
            // We check if we are visible on the screen,
            // We check if we have a model,
            // Then we render it
            if (!m_visible) return;
            if (m_model == null) return;

            // Check Against Frustrum to cull out objects
            if (m_cullDraw) {
                bool collided = false;
                for (int i = 0; i < m_boundingSpheres.Count; i++)
                {
                    if (camera.Frustum.Intersects(m_boundingSpheres[i])) { collided = true; break; }
                }

                if (!collided) return;
            }

            // Finally, we render This Object
            Model3D.Draw(gameTime, graphics, camera, Transform.WorldMatrix, m_animationState,
                (Effect delEffect, GraphicsDevice delGraphics, ICamera delCamera, GameTime delGameTime) =>
                    {
                        SetupEffects(delEffect, delGraphics, delCamera, delGameTime);
                    }
            );
            

            if (m_renderBounds) {
                foreach (ModelMesh mesh in m_model.Model.Meshes)
                {
                    mesh.BoundingSphere.RenderBoundingSphere(graphics, m_transform.WorldMatrix, camera.View, camera.Projection, Color.Red);
                }
            }
        }

        protected virtual void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            if (effect is BasicEffect)
            {
                BasicEffect beffect = effect as BasicEffect;
                beffect.EnableDefaultLighting();
                beffect.PreferPerPixelLighting = true;
            }

            if (effect is SkinnedEffect)
            {
                SkinnedEffect seffect = effect as SkinnedEffect;
                seffect.EnableDefaultLighting();
                seffect.PreferPerPixelLighting = true;
            }
        }
        #endregion
    }
}
