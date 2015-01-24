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
            set { m_model = value; CheckRayIntersection(new Ray()); }
        }

        public StaticGameObject()
            :base()
        {

        }

        public override bool CheckRayIntersection(Ray ray)
        {
            // Generate the bounding boxes
            m_boundingBoxes = new List<BoundingBox>();

            // Create the ModelTransforms
            Matrix[] modelTransforms = new Matrix[Model3D.Bones.Count];
            Model3D.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Check intersection
            foreach (ModelMesh mesh in Model3D.Meshes) {
                //BoundingSphere boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * WorldMatrix);
                BoundingBox boundingBox = mesh.GenerateBoundingBox(m_transform.WorldMatrix);
                m_boundingBoxes.Add(boundingBox);

                if (ray.Intersects(boundingBox) != null) return true;
            }
            return false;
        }


        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;
            
            // First we Update the children
            base.Update(gameTime);

            // Then we Update this
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
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
                for (int i = 0; i < m_boundingBoxes.Count; i++) {
                    if (!m_boundingBoxes[i].Intersects(camera.Frustum)) return;
                }
            }

            // Render This Object
            // Copy any parent transforms.
            Matrix[] boneTransforms = new Matrix[m_model.Bones.Count];
            m_model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in m_model.Meshes) {
                Debug.WriteLine(mesh.Name);
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.

                foreach (Effect effect in mesh.Effects) {
                    if (effect is BasicEffect) {
                        BasicEffect beffect = effect as BasicEffect;
                        beffect.World = boneTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix;
                        beffect.View = camera.View;
                        beffect.Projection = camera.Projection;
                        beffect.EnableDefaultLighting();
                        beffect.LightingEnabled = false;
                        beffect.PreferPerPixelLighting = true;
                        beffect.DiffuseColor = new Vector3(1, 1, 1);
                    }

                    if (effect is SkinnedEffect) {
                        SkinnedEffect seffect = effect as SkinnedEffect;
                        seffect.World = boneTransforms[mesh.ParentBone.Index] * m_transform.WorldMatrix;
                        seffect.View = camera.View;
                        seffect.Projection = camera.Projection;
                        seffect.EnableDefaultLighting();
                        seffect.PreferPerPixelLighting = true;
                        seffect.DiffuseColor = new Vector3(1, 1, 1);
                    }
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();

                if (m_renderBounds) {
                    //mesh.BoundingSphere.RenderBoundingSphere(graphics, m_transform.WorldMatrix, camera.View, camera.Projection, Color.Red);
                    for (int i = 0; i < m_boundingBoxes.Count; i++) {
                        m_boundingBoxes[i].DrawBoundingBox(graphics, Color.Red, camera, Matrix.Identity);
                    }
                }
            }
        }
        #endregion
    }
}
