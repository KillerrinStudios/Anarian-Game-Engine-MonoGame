using System;
using System.Collections.Generic;
using System.Text;
using Anarian.DataStructures;
using Anarian.DataStructures.Components;

namespace Anarian.Interfaces
{
    public interface IScene
    {
        ICamera Camera
        {
            get;
            set;
        }

        Transform SceneNode
        {
            get;
            set;
        }
    }
}
