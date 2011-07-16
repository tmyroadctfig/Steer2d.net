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
    public class RotationPreferencedSteering : Steering
    {
        public RotationPreferencedSteering(IVehicle vehicle) : base(vehicle) { }

        public override SteeringComponents Seek(Vector2 target, float elapsedTime)
        {
            var steeringForce = SteeringHelper.Seek(Vehicle.Position, target);

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
            // TODO: actually preference rotation!!

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
