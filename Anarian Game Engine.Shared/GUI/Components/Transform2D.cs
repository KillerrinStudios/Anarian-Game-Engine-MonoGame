using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Anarian.Interfaces;
using Anarian.Enumerators;
using Anarian.Helpers;

using Microsoft.Xna.Framework;

namespace Anarian.GUI.Components
{
    public class Transform2D : Component2D,
                             IEnumerable, IUpdatable, IMoveable
    {
        #region Fields/Properties
        #region Transforms
        Vector2 m_lastPosition;
        Vector2 m_position;
        public Vector2 Position
        {
            get { return m_position; }
            set
            {
                m_lastPosition = m_position;
                m_position = value;
            }
        }

        Vector2 m_scale;
        public Vector2 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        Vector2 m_origin;
        public Vector2 Origin
        {
            get { return m_origin; }
            set { m_origin = value; }
        }

        float m_rotation;
        public float Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }
        #endregion

        #region Helper Properties
        Vector2 m_widthHeight;
        public Vector2 WidthHeight
        {
            get { return m_widthHeight; }
            set { m_widthHeight = value; }
        }
        public Rectangle PositionRect
        {
            get
            {
                return new Rectangle((int)m_position.X, (int)m_position.Y, (int)m_widthHeight.X, (int)m_widthHeight.Y);
            }
            set {
                Position = new Vector2(value.X, value.Y);
                m_widthHeight = new Vector2(value.Width, value.Height);
            }
        }
        #endregion

        #region World Location
        public Vector2 WorldPosition
        {
            get
            {
                Vector2 pos = m_position;

                if (m_parent != null) {
                    pos += m_parent.WorldPosition;
                }
                return pos;
            }
        }

        public Vector2 WorldScale
        {
            get
            {
                Vector2 sca = m_scale;

                if (m_parent != null) {
                    sca += m_parent.WorldScale;
                }
                return sca;
            }
        }
        public float WorldRotation
        {
            get
            {
                float rot = m_rotation;

                if (m_parent != null) {
                    rot += m_parent.WorldRotation;
                }
                return rot;
            }
        }
        #endregion
        #endregion

        public Transform2D(GuiObject guiObject)
            :base(guiObject, ComponentTypes.Transform)
        {
            Reset();
        }

        public Transform2D(GuiObject guiObject, Vector2 position, Vector2 scale, float rotation)
            : base(guiObject, ComponentTypes.Transform)
        {
            Reset();

            m_position = position;
            m_scale = scale;
            m_rotation = rotation;
        }

        public override void Reset()
        {
            base.Reset();

            // Basic Transforms
            m_position = Vector2.Zero;
            m_scale = Vector2.One;
            m_rotation = 0.0f;

            // Advanced Transforms
            m_origin = Vector2.Zero;
            m_widthHeight = Vector2.One;

            // Setup Children
            m_parent = null;
            m_children = new List<Transform2D>();
        }

        #region Interfaces
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }

        void IMoveable.Move(GameTime gameTime, Vector3 movement) { Move(gameTime, movement); }
        void IMoveable.MoveVertical(GameTime gameTime, float amount) { MoveVertical(gameTime, amount); }
        void IMoveable.MoveHorizontal(GameTime gameTime, float amount) { MoveHorizontal(gameTime, amount); }
        void IMoveable.MoveForward(GameTime gameTime, float amount) { MoveForward(gameTime, amount); }
        void IMoveable.MoveToPosition(GameTime gameTime, Vector3 point) { MoveToPosition(gameTime, new Vector2(point.X, point.Y)); }

        public IEnumerator GetEnumerator()
        {
            foreach (var child in m_children) {
                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return child;
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {

        }

        #region Helper Methods
        /// <summary>
        /// A Direction Vector facing a specified point
        /// </summary>
        /// <param name="point">The point we wish to face</param>
        /// <returns>A normalized direction vector</returns>
        public Vector2 CreateDirectionVector(Vector2 point)
        {
            Vector2 direction = point - m_position;
            direction.Normalize();

            return direction;
        }

        public void SetOriginToCenter()
        {
            m_origin = m_widthHeight / 2.0f;
        }
        #endregion

        #region Movements
        public void Move(GameTime gameTime, Vector3 movement)
        {
            MoveHorizontal(gameTime, movement.X);
            MoveVertical(gameTime, movement.Y);
            MoveForward(gameTime, movement.Z);
        }

        public void MoveHorizontal(GameTime gameTime, float amount) { }
        public void MoveVertical(GameTime gameTime, float amount) { }
        public void MoveForward(GameTime gameTime, float amount) { }

        public bool MoveToPosition(GameTime gameTime, Vector2 point, float boundry = 0.025f)
        {
            //Debug.WriteLine(Vector3.Distance(m_position, point));
            
            // Check if we even need to enter the method or not
            if (Vector2.Distance(m_position, point) <= boundry) return true;

            Vector2 direction = CreateDirectionVector(point);

            Vector2 speed = direction * 0.002f;
            Position += speed * gameTime.DeltaTime();

            // Rotate to the point
            RotateToFaceForward(gameTime);

            // Exit out of the method signaling whether we have arrived yet
            if (Vector2.Distance(m_position, point) <= boundry) return true;
            return false;
        }
        #endregion

        #region Rotations
        public void RotateToFaceForward(GameTime gameTime) { RotateToPoint(gameTime, m_lastPosition); }
        public void RotateToPoint(GameTime gameTime, Vector2 point)
        {
            //Calculate the distance from the square to the mouse's X and Y position
            float XDistance = m_position.X - point.X;
            float YDistance = m_position.Y - point.Y;

            //Calculate the required rotation by doing a two-variable arc-tan
            m_rotation = (float)Math.Atan2(YDistance, XDistance);
        }
        #endregion

        #region Parent/Children
        Transform2D m_parent;
        Transform2D Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        List<Transform2D> m_children;
        public List<Transform2D> GetChildren() { return m_children; }
        public void AddChild(Transform2D child)
        {
            child.Parent = this;
            m_children.Add(child);
        }
        public void RemoveChild(int index) { m_children.RemoveAt(index); }
        public Transform2D GetChild(int index) { return m_children[index]; }

        public void ClearChildren() { m_children.Clear(); }
        #endregion

        public override string ToString()
        {
            return base.ToString() +
                   "Position: " + WorldPosition + " " +
                   "Scale: " + WorldScale + " " +
                   "Rotation: " + WorldRotation;
        }
    }
}
