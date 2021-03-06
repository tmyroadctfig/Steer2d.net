﻿/// Copyright (c) 2011 Luke Quinane
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
/// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
/// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
/// permit persons to whom the Software is furnished to do so, subject to the following conditions:
/// 
/// The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
/// the Software.
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
/// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
/// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
/// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steer2d
{
    /// <summary>
    /// The interface for a vehicle that steering can direct.
    /// </summary>
    public interface IVehicle : IMovingObstacle
    {
        /// <summary>
        /// The current direction of the vehicle (as a unit vector).
        /// </summary>
        Vector2 Direction { get; }

        /// <summary>
        /// The maximum foward thrust.
        /// </summary>
        float MaximumThrust { get; }

        /// <summary>
        /// The maximum reverse thrust.
        /// </summary>
        float MaximumReverseThrust { get; }

        /// <summary>
        /// The rotation rate in radians / second.
        /// </summary>
        float RotationRate { get; }

        /// <summary>
        /// The maximum speed the vehicle can move at.
        /// </summary>
        float? MaximumSpeed { get; }
    }
}
