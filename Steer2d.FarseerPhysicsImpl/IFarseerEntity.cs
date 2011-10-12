using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;

namespace Steer2d.FarseerPhysicsImpl
{
    /// <summary>
    /// A farseer entity.
    /// </summary>
    public interface IFarseerEntity
    {
        /// <summary>
        /// The entity's body.
        /// </summary>
        Body Body { get; }
    }
}
