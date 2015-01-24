using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Interfaces
{
    public interface ISelectableEntity
    {
        /// <summary>
        /// Is the Entity Able to be Selected
        /// </summary>
        bool Selectable { get; set; }

        /// <summary>
        /// Is the Entity Currently Selected
        /// </summary>
        bool Selected { get; set; }
    }
}
