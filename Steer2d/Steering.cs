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
    public static class Steering
    {
        public static Vector2 Pursue(IMovingObstacle vehicle, IMovingObstacle quarry)
        {
            // How parallel is the quarry velocity compared to the vehicle velocity
            // 1 is parallel, 0 is perpendicular, -1 anti-parallel
            float parallel = Vector2.Dot(quarry.Velocity, vehicle.Velocity);

            if (parallel < -0.5)
            {
                return Seek(vehicle.Position, quarry.Position);
            }

            float predictionTime = 1; // MathHelper.SmoothStep(-1, 1, parallel);

            Vector2 predictedQuarryPosition = quarry.Position + (quarry.Velocity * predictionTime);

            float distanceToQuarry = (vehicle.Position - quarry.Position).Length();

            //if (vehicle.Velocity.Length() > distanceToQuarry)
            //{
            //    predictedQuarryPosition = quarry.Position;
            //}

            return Seek(vehicle.Position, predictedQuarryPosition);            
        }

        /// <summary>
        /// Steers towards a target.
        /// </summary>
        /// <param name="position">The current position.</param>
        /// <param name="target">The target position.</param>
        /// <returns>The direction.</returns>
        public static Vector2 Seek(Vector2 position, Vector2 target)
        {
            Vector2 desiredDirection = target - position;
            return desiredDirection;
        }

        /// <summary>
        /// Steers towards a target.
        /// </summary>
        /// <param name="position">The current position.</param>
        /// <param name="target">The target position.</param>
        /// <returns>The direction.</returns>
        public static Vector2 Seek(IVehicle vehicle, Vector2 target, float elapsedTime)
        {
            Vector2 desiredDirection = target - vehicle.Position;
            return desiredDirection;
        }

        public static SteeringComponents GetComponents(Vector2 direction, Vector2 steeringForce)
        {
            return new SteeringComponents()
            {
                SteeringForce = steeringForce,
                Rotation = VectorUtils.FindAngleBetweenTwoVectors(direction, steeringForce),
                Thrust = Vector2.Dot(direction, steeringForce)
            };
        }

        public static SteeringComponents GetComponents(IVehicle vehicle, Vector2 steeringForce, float elapsedTime)
        {
            var rotation = VectorUtils.FindAngleBetweenTwoVectors(vehicle.Direction, steeringForce);
            
            var maxRotation = vehicle.RotationRate * elapsedTime;

            if (Math.Abs(rotation) > maxRotation)
            {
                rotation = MathHelper.Clamp(rotation, -maxRotation, maxRotation);
            }

            steeringForce.Normalize();
            var thrust = Vector2.Dot(vehicle.Direction, steeringForce);

            // How parallel is the steering force compared to the vehicle direction
            // 1 is parallel, 0 is perpendicular, -1 anti-parallel
            float parallel = Vector2.Dot(vehicle.Direction, steeringForce);

            if (parallel > 0)
            {
                thrust *= vehicle.MaximumThrust;
            }
            else
            {
                thrust *= vehicle.MaximumReverseThrust;
            }

            thrust *= elapsedTime;

            return new SteeringComponents()
            {
                SteeringForce = steeringForce,
                Rotation = rotation,
                Thrust = thrust
            };
        }
    }
}
