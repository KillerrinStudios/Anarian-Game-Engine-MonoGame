using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using Anarian.Helpers;

namespace Anarian.DataStructures
{
    public class StaticGameObject : GameObject, IUpdatable, IRenderable
    {
        Model m_model;
        public Model Model3D
        {
            get { return m_model; }
            set { m_model = value; CreateBounds(); SaveDefaultEffects(); }
        }

        public StaticGameObject()
            :base()
        {

        }
        public override void Reset()
        {
            base.Reset();
        }

        #region Operator Overloads
        public static explicit operator StaticGameObject(AnimatedGameObject animatedGameObject)
        {
            StaticGameObject sGO = new StaticGameObject();
            sGO.Model3D = animatedGameObject.Model3D;
            sGO.Transform = new Transform(sGO, animatedGameObject.Transform);

            return sGO;
        }
        #endregion

        public override void CreateBounds()
        {
            base.CreateBounds();

            // Get the ModelTransforms
            Matrix[] modelTransforms = new Matrix[Model3D.Bones.Count];
            Model3D.CopyAbsoluteBoneTransformsTo(modelTransforms);
            
            // Check intersection
            foreach (ModelMesh mesh in Model3D.Meshes)
            {
                var boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix);
                m_boundingSpheres.Add(boundingSphere);
            }
        }

        public override void SaveDefaultEffects()
        {
            base.SaveDefaultEffects();

            foreach (ModelMesh mesh in m_model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    m_defaultEffects.Add(effect);
                }
            }
        }

        public override void RestoreDefaultEffects()
        {
            base.RestoreDefaultEffects();
            foreach (ModelMesh mesh in m_model.Meshes)
            {
                try
                {
                    for (int i = 0; i < mesh.MeshParts.Count; i++)
                    {
                        mesh.MeshParts[i].Effect = m_defaultEffects[i];
                    }
                }
                catch (Exception) { }
            }
        }

        public override bool CheckRayIntersection(Ray ray)
        {
            // Create the ModelTransforms
            Matrix[] modelTransforms = new Matrix[Model3D.Bones.Count];
            Model3D.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Check intersection
            foreach (ModelMesh mesh in Model3D.Meshes) {
                var boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix);
                if (ray.Intersects(boundingSphere).HasValue) return true;
            }
            return false;
        }
        public override bool CheckFrustumIntersection(BoundingFrustum frustum)
        {
            // Create the ModelTransforms
            Matrix[] modelTransforms = new Matrix[Model3D.Bones.Count];
            Model3D.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Check intersection
            foreach (ModelMesh mesh in Model3D.Meshes)
            {
                var boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix);
                if (frustum.Intersects(boundingSphere)) return true;
            }
            return false;
        }
        public override bool CheckSphereIntersection(BoundingSphere sphere)
        {
            // Create the ModelTransforms
            Matrix[] modelTransforms = new Matrix[Model3D.Bones.Count];
            Model3D.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Check intersection
            foreach (ModelMesh mesh in Model3D.Meshes)
            {
                var boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix);
                if (sphere.Intersects(boundingSphere)) return true;
            }
            return false;
        }


        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;
            
            // First we Update the children
            base.Update(gameTime);
        }

        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            // We Draw the base here so that the Children get taken care of
            // We grab the result so we can know if it was visible or not
            var result = base.Draw(gameTime, spriteBatch, graphics, camera);
            if (!result) return false;

            // Now that the children have been rendered...
            // We check if we have a model,
            // Then we render it
            if (m_model == null) return false;

            // Render This Object
            // Copy any parent transforms.
            Matrix[] boneTransforms = new Matrix[m_model.Bones.Count];
            m_model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Draw the model. A model can have multiple meshes, so loop.
            try
            {
                foreach (ModelMesh mesh in m_model.Meshes)
                {
                    //Debug.WriteLine(mesh.Name);
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.

                    foreach (Effect effect in mesh.Effects)
                    {
                        if (effect is BasicEffect)
                        {
                            BasicEffect beffect = effect as BasicEffect;
                            beffect.World = boneTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix;
                            beffect.View = camera.View;
                            beffect.Projection = camera.Projection;
                        }

                        if (effect is SkinnedEffect)
                        {
                            SkinnedEffect seffect = effect as SkinnedEffect;
                            seffect.World = boneTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix;
                            seffect.View = camera.View;
                            seffect.Projection = camera.Projection;
                        }

                        SetupEffects(effect, graphics, camera, gameTime);
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();

                    if (m_renderBounds)
                    {
                        mesh.BoundingSphere.RenderBoundingSphere(graphics, m_transform.WorldMatrix, camera.View, camera.Projection, BoundingSphereColor);
                    }
                }
            }
            catch (Exception) { return false; }
            return true;
        }

        protected virtual void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            if (effect is BasicEffect)
            {
                BasicEffect beffect = effect as BasicEffect;
                beffect.EnableDefaultLighting();
                beffect.LightingEnabled = true;
                beffect.PreferPerPixelLighting = true;
                beffect.DiffuseColor = new Vector3(1, 1, 1);
            }

            if (effect is SkinnedEffect)
            {
                SkinnedEffect seffect = effect as SkinnedEffect;
                seffect.EnableDefaultLighting();
                seffect.PreferPerPixelLighting = true;
                seffect.DiffuseColor = new Vector3(1, 1, 1);
            }
        }
        #endregion
    }
}
