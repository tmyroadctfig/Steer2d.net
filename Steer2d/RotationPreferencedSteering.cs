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
using Steer2d.Utility;

namespace Steer2d
{
    /// <summary>
    /// A steering stategy that preferences rotation over thrust.
    /// </summary>
    public class RotationPreferencedSteering : IGetSteeringComponents
    {
        public RotationPreferencedSteering()
        {
            NonRotationWindow = MathHelper.ToRadians(15);
        }

        /// <summary>
        /// The angle allowed before only rotation will be applied (in radians). E.g. don't only rotate if +/- 15 deg. of the target.
        /// </summary>
        public float NonRotationWindow { get; set; }

        /// <summary>
        /// Gets the steering components for the steering force and vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle the steering force is operating on.</param>
        /// <param name="steeringForce">The steering force.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        public SteeringComponents GetComponents(IVehicle vehicle, Vector2 steeringForce, float elapsedTime)
        {
            var rotation = VectorUtils.FindAngleBetweenTwoVectors(vehicle.Direction, steeringForce);

            var maxRotation = vehicle.RotationRate * elapsedTime;
            var clampedRotation = rotation;

            if (Math.Abs(rotation) > maxRotation)
            {
                clampedRotation = MathHelper.Clamp(rotation, -maxRotation, maxRotation);
            }

            var normalizedSteeringForce = steeringForce;
            normalizedSteeringForce.Normalize();

            float thrust = 0;

            if (Math.Abs(rotation) < NonRotationWindow || Math.Abs(rotation - Math.PI) < NonRotationWindow)
            {
                thrust = Vector2.Dot(vehicle.Direction, normalizedSteeringForce);
                
                // How parallel is the steering force compared to the vehicle direction
                // 1 is parallel, 0 is perpendicular, -1 anti-parallel
                float parallel = Vector2.Dot(vehicle.Direction, normalizedSteeringForce);

                if (parallel > 0)
                {
                    thrust *= vehicle.MaximumThrust;
                }
                else
                {
                    thrust *= vehicle.MaximumReverseThrust;
                }

                thrust *= elapsedTime;
            }

            return new SteeringComponents()
            {
                SteeringTarget = steeringForce,
                SteeringForce = normalizedSteeringForce,
                Rotation = clampedRotation,
                Thrust = thrust
            };
        }

        public SteeringComponents ArriveAtImpl(IVehicle vehicle, float distanceToTarget, float stoppingDisance, 
            Vector2 steeringForce, float elapsedTime)
        {
            var components = GetComponents(vehicle, steeringForce, elapsedTime);

            var rampedSpeed = vehicle.MaximumSpeed * distanceToTarget / stoppingDisance;

            steeringForce.Normalize();
            if (Vector2.Dot(vehicle.Velocity, steeringForce) > rampedSpeed)
            {
                components.Thrust = -0.01f;
            }

            return components;
        }
    }
}
