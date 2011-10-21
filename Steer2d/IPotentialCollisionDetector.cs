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
    /// <summary>
    /// The interface for a potential collision detector.
    /// </summary>
    public interface IPotentialCollisionDetector
    {
        /// <summary>
        /// Finds the nearest obstacle that will potentially collide with the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle that detection is being performed for.</param>
        /// <param name="obstacles">The possible obstacles.</param>
        /// <param name="detectionPeriod">The time window to perform detection in. E.g. 1 second from current position.</param>
        /// <returns>The nearest potential obstacle or null if no obstacles are in the vehicle's path.</returns>
        IObstacle FindNearestPotentialCollision(IVehicle vehicle, IEnumerable<IObstacle> obstacles, float detectionPeriod);
    }
}
