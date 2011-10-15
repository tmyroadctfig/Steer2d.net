using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.SamplesFramework;

namespace Steer2d.Example.Entities
{
    public class Ship : IVehicle
    {
        public Body Body { get; set; }

        public Ship(World world)
        {
            var vertices = new Vertices(new List<Vector2>() 
            {
                new Vector2(0, ConvertUnits.ToSimUnits(-20)),
                new Vector2(ConvertUnits.ToSimUnits(10), ConvertUnits.ToSimUnits(5)),
                new Vector2(ConvertUnits.ToSimUnits(-10), ConvertUnits.ToSimUnits(5)),
            });

            Body = BodyFactory.CreateBody(world);
            Body.BodyType = BodyType.Dynamic;
            Body.UserData = this;

            FixtureFactory.AttachPolygon(vertices, 1, Body);

            MaximumSpeed = ConvertUnits.ToSimUnits(300);
            MaximumThrust = ConvertUnits.ToSimUnits(1000);
            MaximumReverseThrust = ConvertUnits.ToSimUnits(1000);
            RotationRate = (float) Math.PI * 2;
            Colour = Color.Red;
        }

        public float Radius { get { return ConvertUnits.ToSimUnits(25); } }

        public Vector2 Position
        {
            get { return Body.Position; }
            set { Body.Position = value; }
        }

        public Vector2 Velocity { get { return Body.LinearVelocity; } }

        public Vector2 Direction
        {
            get { return Vector2.Transform(new Vector2(0, -1), Matrix.CreateRotationZ(Body.Rotation)); }
        }

        public Color Colour { get; set; }

        public float MaximumSpeed { get; set; }

        public float MaximumThrust { get; set; }

        public float MaximumReverseThrust { get; set; }

        public float RotationRate { get; set; }
        
        public void LimitVelocity()
        {
            if (Body.LinearVelocity.Length() > MaximumSpeed)
            {
                var limitedVelocity = Body.LinearVelocity;
                limitedVelocity.Normalize();
                limitedVelocity *= MaximumSpeed;

                Body.LinearVelocity = limitedVelocity;
            }
        }

        public Vector2 Thrust { get; set; }

        public void Draw(LineBatch lineBatch)
        {
            var v1 = Vector2.Transform(new Vector2(0, -ConvertUnits.ToSimUnits(20)), Matrix.CreateRotationZ(Body.Rotation)) + Body.Position;
            var v2 = Vector2.Transform(new Vector2(ConvertUnits.ToSimUnits(10), ConvertUnits.ToSimUnits(5)), Matrix.CreateRotationZ(Body.Rotation)) + Body.Position;
            var v3 = Vector2.Transform(new Vector2(ConvertUnits.ToSimUnits(-10), ConvertUnits.ToSimUnits(5)), Matrix.CreateRotationZ(Body.Rotation)) + Body.Position;

            lineBatch.DrawLine(v1, v2, Colour);
            lineBatch.DrawLine(v2, v3, Colour);
            lineBatch.DrawLine(v3, v1, Colour);
            
            lineBatch.DrawLine(Body.Position, Body.Position + Velocity, Color.Blue);

            var thrust = Thrust;
            thrust.Normalize();
            thrust *= ConvertUnits.ToSimUnits(100);
            lineBatch.DrawLine(Body.Position, Body.Position + thrust, Color.Orange);
        }

        public void ApplySteering(SteeringComponents steeringComponents1)
        {
            if (steeringComponents1.IsValid)
            {
                // Apply rotation
                Body.Rotation -= steeringComponents1.Rotation;
                Body.AngularVelocity = 0;

                // Apply thrust
                var thrust = Direction;
                thrust.Normalize();
                thrust *= steeringComponents1.Thrust;
                Thrust = thrust;

                Body.LinearVelocity += thrust;
            }

            LimitVelocity();
        }
    }
}
