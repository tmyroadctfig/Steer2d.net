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
