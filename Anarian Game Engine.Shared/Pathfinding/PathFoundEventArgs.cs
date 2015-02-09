using Anarian.Events;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Pathfinding
{
    public delegate void PathFoundEventHandler(object sender, PathFoundEventArgs e);

    public class PathFoundEventArgs : AnarianEventArgs
    {
        public IPathfindable Pathfindable { get; private set; }

        public PathFoundEventArgs(GameTime gameTime, IPathfindable pathfindable)
            : base(gameTime, new Exception(), false, null)
        {
            Pathfindable = pathfindable;
        }
        public PathFoundEventArgs(GameTime gameTime, IPathfindable pathfindable, Exception e, bool canceled, Object state)
            : base(gameTime, e, canceled, state)
        {
            Pathfindable = pathfindable;
        }
    }
}
