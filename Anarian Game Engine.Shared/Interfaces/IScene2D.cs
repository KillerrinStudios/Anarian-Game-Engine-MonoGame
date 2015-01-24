using System;
using System.Collections.Generic;
using System.Text;

using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.GUI.Components;
using Anarian.Events;

namespace Anarian.Interfaces
{
    public interface IScene2D
    {
        Transform2D SceneNode
        {
            get;
            set;
        }

        #region Event Implimentations
        void HandlePointerDown(object sender, PointerPressedEventArgs e);
        void HandlePointerPressed(object sender, PointerPressedEventArgs e);
        void HandlePointerMoved(object sender, PointerMovedEventArgs e);
        #endregion
    }
}
