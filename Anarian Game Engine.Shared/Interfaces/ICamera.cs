using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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


        Ray GetMouseRay(Vector2 mousePosition, Viewport viewport);
        Vector2 ProjectToScreenCoordinates(Vector3 position, Viewport viewport);
        BoundingFrustum UnprojectRectangle(Rectangle source, Viewport viewport);

        void Update(GameTime gameTime);
    }
}
