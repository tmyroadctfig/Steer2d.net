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
    /// The interface for a class that can get steering components from a steering force.
    /// </summary>
    public interface IGetSteeringComponents
    {
        /// <summary>
        /// Gets the steering components for the steering force and vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle the steering force is operating on.</param>
        /// <param name="steeringObjective">A string describing the steering force.</param>
        /// <param name="steeringForce">The steering force.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        SteeringComponents GetComponents(IVehicle vehicle, string steeringObjective, Vector2 steeringForce, float elapsedTime);

        /// <summary>
        /// Arrives at the target based on the given parameters.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <param name="distanceToTarget">The distance to the target.</param>
        /// <param name="stoppingDisance">The minimum stopping distance.</param>
        /// <param name="steeringForce">The steering force.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        SteeringComponents ArriveAtImpl(IVehicle vehicle, float distanceToTarget, float stoppingDisance,
            Vector2 steeringForce, float elapsedTime);
    }
}
