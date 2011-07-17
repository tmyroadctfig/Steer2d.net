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
        
        /// <summary>
        /// Gets the steering components for the steering force and vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle the steering force is operating on.</param>
        /// <param name="steeringForce">The steering force.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        public override SteeringComponents GetComponents(Vector2 steeringForce, float elapsedTime)
        {
            var rotation = VectorUtils.FindAngleBetweenTwoVectors(Vehicle.Direction, steeringForce);

            var maxRotation = Vehicle.RotationRate * elapsedTime;

            if (Math.Abs(rotation) > maxRotation)
            {
                rotation = MathHelper.Clamp(rotation, -maxRotation, maxRotation);
            }

            steeringForce.Normalize();
            var thrust = Vector2.Dot(Vehicle.Direction, steeringForce);

            // How parallel is the steering force compared to the vehicle direction
            // 1 is parallel, 0 is perpendicular, -1 anti-parallel
            float parallel = Vector2.Dot(Vehicle.Direction, steeringForce);

            if (parallel > 0)
            {
                thrust *= Vehicle.MaximumThrust;
            }
            else
            {
                thrust *= Vehicle.MaximumReverseThrust;
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
