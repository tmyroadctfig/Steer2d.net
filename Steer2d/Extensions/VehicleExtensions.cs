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

namespace Steer2d
{
    public static class VehicleExtensions
    {
        /// <summary>
        /// Gets the stopping distance for the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle to get the stopping distance for.</param>
        /// <returns>The stopping distance.</returns>
        public static float GetStoppingDistance(this IVehicle vehicle)
        {
            // From: v^2 = u^2 + 2as, and since v = 0:            
            // s = u^2 / 2a
            return vehicle.Velocity.LengthSquared() / 2 * vehicle.MaximumReverseThrust;
        }
    }
}
