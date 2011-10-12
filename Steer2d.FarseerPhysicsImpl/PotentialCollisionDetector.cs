using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace Steer2d.FarseerPhysicsImpl
{
    /// <summary>
    /// A Farseer Physics Engine based implementation of IPotentialCollisionDetector.
    /// </summary>
    public class PotentialCollisionDetector : IPotentialCollisionDetector
    {
        /// <summary>
        /// The fixture that is detecting nearest obstacles.
        /// </summary>
        Fixture _fixture;

        public PotentialCollisionDetector(Body body, Category collisionCategories, Category collidesWith)
        {
            // TODO: make the sensor configurable
            _fixture = FixtureFactory.AttachRectangle(1, 5, float.Epsilon * 10f, new Vector2(0, -2.5f), body);
            _fixture.IsSensor = true;
            _fixture.UserData = body.UserData;
            _fixture.CollisionCategories = collisionCategories;
            _fixture.CollidesWith = collidesWith;
        }

        private Fixture FindClosestCollider(Vector2 position)
        {
            float? closestDistance = null;
            Fixture closestCollider = null;
            
            var contactEdges = _fixture.Body.ContactList
                .GetContactEdges()
                .Where(ce => ce.Other != _fixture.Body)
                .ToList();

            contactEdges = contactEdges
                .Where(ce => ce.Contact.IsTouching())
                .ToList();

            foreach (var edge in contactEdges)
            {
                var distance = (edge.Other.Position - position).Length();

                if (!closestDistance.HasValue || closestDistance.Value > distance)
                {
                    closestDistance = distance;
                    closestCollider = edge.Contact.FixtureB;
                }
            }

            return closestCollider;
        }

        int count = 0;

        /// <summary>
        /// Finds the nearest obstacle that will potentially collide with the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle that detection is being performed for.</param>
        /// <param name="obstacles">The possible obstacles.</param>
        /// <param name="detectionPeriod">The time window to perform detection in. E.g. 1 second from current position.</param>
        /// <returns>The nearest potential obstacle or null if no obstacles are in the vehicle's path.</returns>
        public IObstacle FindNearestPotentialCollision(IVehicle vehicle, IEnumerable<IObstacle> obstacles, float detectionPeriod)
        {
            var closestColliderFixture = FindClosestCollider(vehicle.Position);

            if (closestColliderFixture == null)
            {
                // No collisions
                return null;
            }

            foreach (var obstacle in obstacles)
            {
                var entity = obstacle as IFarseerEntity;

                if (entity != null && entity.Body.Equals(closestColliderFixture.Body))
                {
                    return obstacle;
                }
            }

            return null;
        }
    }
}
