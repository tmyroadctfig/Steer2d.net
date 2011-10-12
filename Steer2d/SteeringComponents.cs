/// Copyright (c) 2011 Luke Quinane
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
    /// The components returned from a steering operation.
    /// </summary>
    public struct SteeringComponents
    {
        /// <summary>
        /// The original steering force.
        /// </summary>
        public Vector2 SteeringForce { get; set; }

        /// <summary>
        /// The thrust component. If the value is negative then deceleration is required.
        /// </summary>
        public float Thrust { get; set; }

        /// <summary>
        /// The rotation quantity.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Checks if the values of the steering components are valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return 
                    !float.IsNaN(Thrust) && 
                    !float.IsNaN(Rotation);
            }
        }

        /// <summary>
        /// A instance to represent no steering force.
        /// </summary>
        public static readonly SteeringComponents NoSteering = new SteeringComponents();
    }
}
