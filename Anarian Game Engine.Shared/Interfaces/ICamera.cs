using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Interfaces
{
    public interface ICamera
    {
        float FieldOfView { get; set; }
        float Near { get; set; }
        float Far { get; set; }
        float AspectRatio { get; set; }

        Vector3 Position { get; set; }
        Vector3 LookAt { get; set; }
        Vector3 Up { get; set; }

        float Pitch { get; set; }
        float Yaw { get; set; }
        float Roll { get; set; }

        Matrix View { get; set; }
        Matrix Projection { get; set; }
        Matrix World { get; set; }

        BoundingFrustum Frustum { get; set; }

        void Update(GameTime gameTime);
    }
}
