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
using Steer2d.Utility;

namespace Steer2d
{
    /// <summary>
    /// The base class for vehicle steering.
    /// </summary>
    public abstract class Steering
    {
        /// <summary>
        /// The vehicle steering is working on.
        /// </summary>
        public IVehicle Vehicle { get; protected set; }

        /// <summary>
        /// Creates a new steering instance.
        /// </summary>
        /// <param name="vehicle">The vehicle to use.</param>
        public Steering(IVehicle vehicle)
        {
            Vehicle = vehicle;
        }

        /// <summary>
        /// Gets the steering components for the steering force and vehicle.
        /// </summary>
        /// <param name="steeringForce">The steering force.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        public abstract SteeringComponents GetComponents(Vector2 steeringForce, float elapsedTime);

        /// <summary>
        /// Seeks to a target point.
        /// </summary>
        /// <param name="target">The target to seek to.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        public virtual SteeringComponents Seek(Vector2 target, float elapsedTime)
        {
            var estimatedPosition = Vehicle.Position + Vehicle.Velocity * elapsedTime;
            var steeringForce = SteeringHelper.Seek(estimatedPosition, target);

            return GetComponents(steeringForce, elapsedTime);
        }

        /// <summary>
        /// Pursues another vehicle.
        /// </summary>
        /// <param name="target">The target to pursue.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        public virtual SteeringComponents Pursue(IVehicle target, float elapsedTime)
        {
            var estimatedPosition = Vehicle.Position + Vehicle.Velocity * elapsedTime;
            var estimatedTagetPosition = target.Position + target.Velocity * elapsedTime;
            var steeringForce = SteeringHelper.Seek(estimatedPosition, estimatedTagetPosition);

            return GetComponents(steeringForce, elapsedTime);
        }

        /// <summary>
        /// Evades another vehicle.
        /// </summary>
        /// <param name="target">The target to evade.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        public virtual SteeringComponents Evade(IVehicle target, float elapsedTime)
        {
            var estimatedPosition = Vehicle.Position + Vehicle.Velocity * elapsedTime;
            var estimatedTagetPosition = target.Position + target.Velocity * elapsedTime;
            var steeringForce = SteeringHelper.Flee(estimatedPosition, estimatedTagetPosition);

            return GetComponents(steeringForce, elapsedTime);
        }

        /// <summary>
        /// Arrives at a target point, stopping on arrival.
        /// </summary>
        /// <param name="target">The target to arrive at.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        public virtual SteeringComponents ArriveAt(Vector2 target, float elapsedTime)
        {            
            var estimatedPosition = Vehicle.Position + Vehicle.Velocity * elapsedTime;
            var distanceToTarget = (target - estimatedPosition).Length();
            var stoppingDisance = Vehicle.GetStoppingDistance();

            if (distanceToTarget > stoppingDisance)
            {
                var steeringForce = SteeringHelper.Seek(estimatedPosition, target);
                return GetComponents(steeringForce, elapsedTime);
            }
            else
            {
                return ArriveAtImpl(distanceToTarget, stoppingDisance, 
                    SteeringHelper.Seek(estimatedPosition, target), elapsedTime);
            }
        }

        /// <summary>
        /// Arrives at the target based on the given parameters.
        /// </summary>
        /// <param name="distanceToTarget">The distance to the target.</param>
        /// <param name="stoppingDisance">The minimum stopping distance.</param>
        /// <param name="steeringForce">The steering force.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        protected abstract SteeringComponents ArriveAtImpl(float distanceToTarget, float stoppingDisance, 
            Vector2 steeringForce, float elapsedTime);
    }
}
