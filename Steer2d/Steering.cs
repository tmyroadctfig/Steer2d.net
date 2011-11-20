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
    /// Provides vehicle steering.
    /// </summary>
    public class Steering
    {
        /// <summary>
        /// The vehicle steering is working on.
        /// </summary>
        public IVehicle Vehicle { get; protected set; }

        /// <summary>
        /// The utility for getting the steering components.
        /// </summary>
        public IGetSteeringComponents GetSteeringComponents { get; protected set; }

        /// <summary>
        /// The potential collision detector. Used for obstacle avoidance.
        /// </summary>
        public IPotentialCollisionDetector PotentialCollisionDetector { get; protected set; }

        /// <summary>
        /// Creates a new steering instance.
        /// </summary>
        /// <param name="vehicle">The vehicle to use.</param>
        /// <param name="getSteeringComponents">The utility for getting the steering components.</param>
        public Steering(IVehicle vehicle, IGetSteeringComponents getSteeringComponents)
            : this(vehicle, getSteeringComponents, null)
        {
        }

        /// <summary>
        /// Creates a new steering instance.
        /// </summary>
        /// <param name="vehicle">The vehicle to use.</param>
        /// <param name="getSteeringComponents">The utility for getting the steering components.</param>
        /// <param name="potentialCollisionDetector">The potential collision detector.</param>
        public Steering(IVehicle vehicle, IGetSteeringComponents getSteeringComponents, IPotentialCollisionDetector potentialCollisionDetector)
        {
            Vehicle = vehicle;
            GetSteeringComponents = getSteeringComponents;
            PotentialCollisionDetector = potentialCollisionDetector;

            AvoidanceFactor = 1.1f;

            MaximumBoidDistance = 1000;
            MinimumBoidDistance = 50f;
            BoidCohesionDistance = 60f;
        }

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

            return GetComponents("Seek", steeringForce, elapsedTime);
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

            return GetComponents("Pursue", steeringForce, elapsedTime);
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

            return GetComponents("Evade", steeringForce, elapsedTime);
        }

        /// <summary>
        /// Steers to avoid the given obstacles.
        /// </summary>
        /// <param name="obstacles">The obstacles to avoid.</param>
        /// <param name="detectionPeriod">The time window to perform detection in. E.g. 1 second from current position.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        public SteeringComponents AvoidObstacles(IEnumerable<IObstacle> obstacles, float detectionPeriod, float elapsedTime)
        {
            IObstacle nearestObstacle = PotentialCollisionDetector
                .FindNearestPotentialCollision(Vehicle, obstacles, detectionPeriod);

            if (nearestObstacle != null)
            {
                // Find the steering direction
                var obstacleOffset = Vehicle.Position - nearestObstacle.Position;
                var parallel = Vehicle.Direction * Vector2.Dot(obstacleOffset, Vehicle.Direction);
                var perpendicular = obstacleOffset - parallel;

                // Offset to be past the obstacle's edge
                perpendicular.Normalize();
                var seekTo = nearestObstacle.Position + perpendicular * (nearestObstacle.Radius + (Vehicle.Radius * AvoidanceFactor));

                return GetComponents("Avoid obstacle", seekTo, elapsedTime);
            }

            return SteeringComponents.NoSteering;
        }

        /// <summary>
        /// The avoidance factor. If set to 1.1f then the steering will attempt to place 10% of the ship radius
        /// between the vehicle and the obstacle.
        /// </summary>
        public float AvoidanceFactor { get; set; }

        /// <summary>
        /// Steers to stay aligned and cohesive to the flock.
        /// </summary>
        /// <param name="flock">The flock of vehicles to stay with.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering force.</returns>
        public SteeringComponents Flock(IEnumerable<IVehicle> flock, float elapsedTime)
        {
            var closeBoids = FindCloseBoids(flock);

            if (!closeBoids.Any())
            {
                // Found no other boids!
                return SteeringComponents.NoSteering;
            }

            if (closeBoids.First().DistanceSquared < MinimumBoidDistance)
            {
                // Steer to separate
                return SeparationImpl(closeBoids, elapsedTime);
            }
            else if (closeBoids.First().DistanceSquared > BoidCohesionDistance)
            {
                // Steer for cohension
                return CohesionImpl(closeBoids, elapsedTime);
            }
            else
            {
                // Steer for alignment
                return AlignmentImpl(closeBoids, elapsedTime);
            }
        }

        /// <summary>
        /// Steers to separate from neighbours.
        /// </summary>
        /// <param name="neighbours">The vehicles to separate from.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering force.</returns>
        public SteeringComponents Separate(IEnumerable<IVehicle> neighbours, float elapsedTime)
        {
            var closeBoids = FindCloseBoids(neighbours);
            
            if (closeBoids.Any() && closeBoids.First().DistanceSquared < MinimumBoidDistance)
            {
                // Steer to separate
                return SeparationImpl(closeBoids, elapsedTime);
            }

            return SteeringComponents.NoSteering;
        }

        /// <summary>
        /// Separates from nearby neighbours.
        /// </summary>
        /// <param name="closeBoids">The close boids.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering force.</returns>
        protected SteeringComponents SeparationImpl(IEnumerable<BoidDistance> closeBoids, float elapsedTime)
        {
            Vector2 direction = closeBoids.Aggregate(Vector2.Zero, (p, b) => p + b.Boid.Position);
            direction /= closeBoids.Count();
            direction.Normalize();

            return GetComponents("Flocking: separation", direction, elapsedTime);
        }

        /// <summary>
        /// Steers for cohesion.
        /// </summary>
        /// <param name="closeBoids">The close boids.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering force.</returns>
        protected SteeringComponents CohesionImpl(IEnumerable<BoidDistance> closeBoids, float elapsedTime)
        {
            Vector2 direction = closeBoids.Aggregate(Vector2.Zero, (p, b) => p + b.Boid.Position);

            direction /= closeBoids.Count();
            direction -= Vehicle.Position;
            direction.Normalize();

            return GetComponents("Flocking: cohesion", direction, elapsedTime);
        }

        /// <summary>
        /// Steers for alignment.
        /// </summary>
        /// <param name="closeBoids">The close boids.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering force.</returns>
        protected SteeringComponents AlignmentImpl(IEnumerable<BoidDistance> closeBoids, float elapsedTime)
        {
            Vector2 direction = closeBoids.Aggregate(Vector2.Zero, (p, b) => p + b.Boid.Direction);

            direction /= closeBoids.Count();
            direction -= Vehicle.Direction;
            direction.Normalize();

            return GetComponents("Flocking: alignment", direction, elapsedTime);
        }

        /// <summary>
        /// Finds close boids for flocking. Any boids further than "MaximumBoidDistance" will be filtered.
        /// </summary>
        /// <param name="flock">The flock to find close boids in.</param>
        /// <returns>The close boids ordered by closest boid first.</returns>
        public IEnumerable<BoidDistance> FindCloseBoids(IEnumerable<IVehicle> flock)
        {
            // Filter out any boids that are too distance
            var maxBoidDistanceSquared = MaximumBoidDistance * MaximumBoidDistance;

            var closeBoids = flock
                .Where(b => !Vehicle.Equals(b))
                .Select(b => new BoidDistance
                    {
                        Boid = b, 
                        DistanceSquared = (b.Position - Vehicle.Position).LengthSquared()
                    });

            closeBoids = closeBoids
                .OrderBy(bd => bd.DistanceSquared);

            closeBoids = closeBoids
                .Where(bd => bd.DistanceSquared < MaximumBoidDistance);

            closeBoids = closeBoids
                .Take(5);

            return closeBoids;
        }

        /// <summary>
        /// The maximum distance between this vehicle and another before the other is not considered in flocking calculations.
        /// </summary>
        public float MaximumBoidDistance { get; set; }

        /// <summary>
        /// The minimum distance between this vehicle and another before this vehicle will steer away slightly.
        /// </summary>
        public float MinimumBoidDistance { get; set; }

        /// <summary>
        /// The distance between this vehicle and another before this vehicle will steer towards the other slightly.
        /// </summary>
        public float BoidCohesionDistance { get; set; }

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
                return GetComponents("Arrive at: seek", steeringForce, elapsedTime);
            }
            else
            {
                return GetSteeringComponents.ArriveAtImpl(Vehicle, distanceToTarget, stoppingDisance, 
                    SteeringHelper.Seek(estimatedPosition, target), elapsedTime);
            }
        }

        /// <summary>
        /// Gets the steering components for the steering force and vehicle.
        /// </summary>
        /// <param name="steeringObjective">A string describing the steering force.</param>
        /// <param name="steeringForce">The steering force.</param>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns>The steering components.</returns>
        protected SteeringComponents GetComponents(string steeringObjective, Vector2 steeringForce, float elapsedTime)
        {
            return GetSteeringComponents.GetComponents(Vehicle, steeringObjective, steeringForce, elapsedTime);
        }
    }
}
