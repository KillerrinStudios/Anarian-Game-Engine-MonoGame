using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Interfaces
{
    public interface ICamera
    {
        float Near { get; set; }
        float Far { get; set; }
        float AspectRatio { get; set; }
        
        Matrix View { get; set; }
        Matrix Projection { get; set; }
        Matrix World { get; set; }

        BoundingFrustum Frustum { get; set; }

    }
}
