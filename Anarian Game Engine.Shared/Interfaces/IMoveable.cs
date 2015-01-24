using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Anarian.Interfaces
{
    public interface IMoveable
    {
        // Generic Movements
        void Move(GameTime gameTime, Vector3 movement);

        void MoveVertical(GameTime gameTime, float amount);

        void MoveHorizontal(GameTime gameTime, float amount);

        void MoveForward(GameTime gameTime, float amount);


        // Specific Movements
        void MoveToPosition(GameTime gameTime, Vector3 point);
    }
}
