using Anarian.Enumerators;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.DataStructures
{
    public class UniversalCamera : ICamera, IMoveable
    {
        public static float Aspect4x3 = 4 / 3;
        public static float Aspect16x9 = 16 / 9;

        #region Fields/Properties
        public CameraMode CurrentCameraMode;
        public BoundingFrustum Frustum { get; protected set; }

        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }
        public Matrix World { get; protected set; }

        
        #region View Properties
        public Vector3 Position { get; set; }
        public Vector3 Target { get; protected set; }
        public Matrix CameraRotation { get; set; }

        public Vector3 DefaultCameraPosition { get; set; }
        public Matrix DefaultCameraRotation { get; set; }

        public Vector3 MinClamp { get; set; }
        public Vector3 MaxClamp { get; set; }

        #region Free Camera
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }

        public float Speed;
        #endregion

        #region Chase/Orbit Camera
        public Matrix WorldPositionToChase;

        private Vector3 desiredPosition;
        private Vector3 desiredTarget;
        private Vector3 offsetDistance;

        #endregion
        #endregion

        #region Projection Properties
        float m_fov;
        float m_aspectRatio;
        float m_zNear;
        float m_zFar;

        public float FoV
        {
            get { return m_fov; }
            set { CreateProjectionMatrix(value, m_aspectRatio, m_zNear, m_zFar); }
        }
        public float AspectRatio
        {
            get { return m_aspectRatio; }
            set { CreateProjectionMatrix(m_fov, value, m_zNear, m_zFar); }
        }
        public float Near
        {
            get { return m_zNear; }
            set { CreateProjectionMatrix(m_fov, m_aspectRatio, value, m_zFar); }
        }
        public float Far
        {
            get { return m_zFar; }
            set { CreateProjectionMatrix(m_fov, m_aspectRatio, m_zNear, value); }
        }
        #endregion
        #endregion

        public UniversalCamera()
        {
            CurrentCameraMode = CameraMode.Free;
            
            DefaultCameraPosition = new Vector3(0, 0, 50);
            DefaultCameraRotation = Matrix.Identity;

            MinClamp = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            MaxClamp = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            ResetCamera();
        }

        #region Resets
        public void ResetCamera()
        {
            // Free Camera Stuff
            Yaw = 0.0f;
            Pitch = 0.0f;
            Roll = 0.0f;

            Speed = 0.3f;

            // Chase Camera Stuff
            desiredPosition = Position;
            desiredTarget = Target;
            offsetDistance = new Vector3(0, 0, 50);

            WorldPositionToChase = new Matrix();

            // Standard Camera Stuff
            Position = DefaultCameraPosition;
            Target = new Vector3();

            CameraRotation = DefaultCameraRotation;
            
            View = Matrix.Identity;
            CreateProjectionMatrix(MathHelper.ToRadians(45.0f), 16 / 9, 0.5f, 500.0f);
            World = Matrix.Identity;
        }

        public void ResetViewToDefaults()
        {
            Position = DefaultCameraPosition;
            CameraRotation = DefaultCameraRotation;
        }

        public void ResetRotations()
        {
            Yaw = 0.0f;
            Pitch = 0.0f;
            Roll = 0.0f;
            CameraRotation = DefaultCameraRotation;
        }
        #endregion

        public void CreateProjectionMatrix(float fov, float aspectRatio, float near, float far)
        {
            m_fov = fov;
            m_aspectRatio = aspectRatio;
            m_zNear = near;
            m_zFar = far;

            Projection = Matrix.CreatePerspectiveFieldOfView(m_fov, m_aspectRatio, m_zNear, m_zFar);
            Frustum = new BoundingFrustum(View * Projection);
        }

        #region Helper Methods
        public void SwitchCameraMode()
        {
            ResetCamera();
            CurrentCameraMode++;

            if (CurrentCameraMode >= CameraMode.Count)
            {
                CurrentCameraMode = 0;
            }
        }

        public Ray GetMouseRay(Vector2 mousePosition, Viewport viewport)
        {
            Vector3 nearPoint = new Vector3(mousePosition, 0);
            Vector3 farPoint = new Vector3(mousePosition, 1);

            // Unproject these points into world space
            nearPoint = viewport.Unproject(nearPoint, Projection, View, World);
            farPoint = viewport.Unproject(farPoint, Projection, View, World);

            // Create the Ray and Return
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        public Vector2 ProjectToScreenCoordinates(Vector3 position, Viewport viewport)
        {
            // Project the 3d position first
            Vector3 screenPos3D = viewport.Project(position, Projection, View, World);

            // Just to make it easier to use we create a Vector2 from screenPos3D
            Vector2 screenPos2D = new Vector2(screenPos3D.X, screenPos3D.Y);
            return screenPos2D;
        }

        public BoundingFrustum UnprojectRectangle(Rectangle source, Viewport viewport)
        {
            //http://forums.create.msdn.com/forums/p/6690/35401.aspx , by "The Friggm"
            // Many many thanks to him...

            // Point in screen space of the center of the region selected
            Vector2 regionCenterScreen = new Vector2(source.Center.X, source.Center.Y);

            // Generate the projection matrix for the screen region
            Matrix regionProjMatrix = Projection;

            // Calculate the region dimensions in the projection matrix. M11 is inverse of width, M22 is inverse of height.
            regionProjMatrix.M11 /= ((float)source.Width / (float)viewport.Width);
            regionProjMatrix.M22 /= ((float)source.Height / (float)viewport.Height);

            // Calculate the region center in the projection matrix. M31 is horizonatal center.
            regionProjMatrix.M31 = (regionCenterScreen.X - (viewport.Width / 2f)) / ((float)source.Width / 2f);

            // M32 is vertical center. Notice that the screen has low Y on top, projection has low Y on bottom.
            regionProjMatrix.M32 = -(regionCenterScreen.Y - (viewport.Height / 2f)) / ((float)source.Height / 2f);

            return new BoundingFrustum(View * regionProjMatrix);
        }
        #endregion

        public void Update(GameTime gameTime)
        {
            UpdateViewMatrix();
        }
        private void UpdateViewMatrix()
        {
            Position = Vector3.Clamp(Position, MinClamp, MaxClamp);

            switch (CurrentCameraMode)
            {
                case CameraMode.Free:
                    CameraRotation.Forward.Normalize();
                    CameraRotation.Up.Normalize();
                    CameraRotation.Right.Normalize();
                    
                    CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Right, Pitch);
                    CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Up, Yaw);
                    CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Forward, Roll);

                    Yaw = 0.0f;
                    Pitch = 0.0f;
                    Roll = 0.0f;

                    Target = Position + CameraRotation.Forward;
                    break;

                case CameraMode.Chase:
                    CameraRotation.Forward.Normalize();
                    WorldPositionToChase.Right.Normalize();
                    WorldPositionToChase.Up.Normalize();
 
                    CameraRotation = Matrix.CreateFromAxisAngle(CameraRotation.Forward, Roll);

                    desiredTarget = WorldPositionToChase.Translation;
                    Target = desiredTarget;
                    Target += WorldPositionToChase.Right * Yaw;
                    Target += WorldPositionToChase.Up * Pitch;

                    desiredPosition = Vector3.Transform(offsetDistance, WorldPositionToChase);
                    Position = Vector3.SmoothStep(Position, desiredPosition, .15f);
 
                    Yaw = MathHelper.SmoothStep(Yaw, 0f, .1f);
                    Pitch = MathHelper.SmoothStep(Pitch, 0f, .1f);
                    Roll = MathHelper.SmoothStep(Roll, 0f, .2f);
                    break;

                case CameraMode.Orbit:
                    CameraRotation.Forward.Normalize();
                     
                    CameraRotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw) * Matrix.CreateFromAxisAngle(CameraRotation.Forward, Roll);
                     
                    desiredPosition = Vector3.Transform(offsetDistance, CameraRotation);
                    desiredPosition += WorldPositionToChase.Translation;
                    Position = desiredPosition;

                    Target = WorldPositionToChase.Translation;
                     
                    Roll = MathHelper.SmoothStep(Roll, 0f, .2f);
                    break;
            }

            View = Matrix.CreateLookAt(Position, Target, CameraRotation.Up);
            Frustum = new BoundingFrustum(View * Projection);
        }

        public void Move(GameTime gameTime, Vector3 addedVector)
        {
            Position += (Speed * addedVector); // *(float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        #region ICamera Implimentation
        void ICamera.Update(GameTime gameTime) { Update(gameTime); }

        float ICamera.FieldOfView
        {
            get { return FoV; }
            set { FoV = value; }
        }
        float ICamera.Near
        {
            get { return Near; }
            set { Near = value; }
        }
        float ICamera.Far
        {
            get { return Far; }
            set { Far = value; }
        }
        float ICamera.AspectRatio
        {
            get { return AspectRatio; }
            set { AspectRatio = value; }
        }

        Vector3 ICamera.Position
        {
            get { return Position; }
            set { Position = value; }
        }
        Vector3 ICamera.LookAt
        {
            get { return Target; }
            set { Target = value; }
        }
        Vector3 ICamera.Up
        {
            get { return CameraRotation.Up; }
            set { } //CameraRotation.Up = value; }
        }

        float ICamera.Pitch
        {
            get { return Pitch; }
            set { Pitch = value; }
        }
        float ICamera.Yaw
        {
            get { return Yaw; }
            set { Yaw = value; }
        }
        float ICamera.Roll
        {
            get { return Roll; }
            set { Roll = value; }
        }

        Matrix ICamera.View
        {
            get { return View; }
            set { View = value; }
        }
        Matrix ICamera.Projection
        {
            get { return Projection; }
            set { Projection = value; }
        }
        Matrix ICamera.World
        {
            get { return World; }
            set { World = value; }
        }
        BoundingFrustum ICamera.Frustum
        {
            get { return Frustum; }
            set { Frustum = value; }
        }
        #endregion

        #region Moveable Implementation
        void IMoveable.Move(GameTime gameTime, Vector3 movement)
        {
            Move(gameTime, movement);
        }

        void IMoveable.MoveHorizontal(GameTime gameTime, float amount)
        {
            Vector3 movement = new Vector3(amount, 0.0f, 0.0f);
            Move(gameTime, movement * CameraRotation.Right);
        }
        void IMoveable.MoveVertical(GameTime gameTime, float amount)
        {
            Vector3 movement = new Vector3(0.0f, amount, 0.0f);
            Move(gameTime, movement * CameraRotation.Up);
        }
        void IMoveable.MoveForward(GameTime gameTime, float amount)
        {
            Vector3 movement = new Vector3(0.0f, 0.0f, amount);
            Move(gameTime, movement * CameraRotation.Forward);
        }

        void IMoveable.MoveToPosition(GameTime gameTime, Vector3 point) { Position = point; }
        #endregion
    }
}
