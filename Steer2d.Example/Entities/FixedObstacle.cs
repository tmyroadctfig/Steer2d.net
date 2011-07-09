using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Steer2d.Example.Entities
{
    public class FixedObstacle : IObstacle
    {
        Body _body;

        public FixedObstacle(World world, float radius)
        {
            Radius = radius;

            _body = BodyFactory.CreateBody(world);
            _body.BodyType = BodyType.Dynamic;

            FixtureFactory.AttachCircle(radius, 1, _body);            
        }

        public float Radius { get; set; }

        public Vector2 Position { get { return _body.Position; } }
    }
}
