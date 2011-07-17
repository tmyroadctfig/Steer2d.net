using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Steer2d.Utility;

namespace Steer2d
{
    /// <summary>
    /// A steering stategy that preferences thrust over rotation.
    /// </summary>
    public class ThrustPreferencedSteering : Steering
    {
        public ThrustPreferencedSteering(IVehicle vehicle) : base(vehicle) { }

        public override SteeringComponents Seek(Vector2 target, float elapsedTime)
        {
            var estimatedPosition = Vehicle.Position + Vehicle.Velocity * elapsedTime;
            var steeringForce = SteeringHelper.Seek(estimatedPosition, target);

            return GetComponents(Vehicle, steeringForce, elapsedTime);
        }

        /// <summary>
        /// Gets the steering components for the steering force and vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle the steering force is operating on.</param>
        /// <param name="steeringForce">The steering force.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
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

        /// <summary>
        /// Gets the steering components from the steering force and current direction.
        /// </summary>
        /// <param name="direction">The current direction of the vehicle.</param>
        /// <param name="steeringForce">The steering force.</param>
        /// <returns>The steering components.</returns>
        public static SteeringComponents GetComponents(Vector2 direction, Vector2 steeringForce)
        {
            return new SteeringComponents()
            {
                SteeringForce = steeringForce,
                Rotation = VectorUtils.FindAngleBetweenTwoVectors(direction, steeringForce),
                Thrust = Vector2.Dot(direction, steeringForce)
            };
        }
    }
}
