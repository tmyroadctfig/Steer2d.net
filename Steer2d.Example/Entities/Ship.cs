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
        Sprite _shipSprite;
        Sprite _lineSprite;

        public Body Body { get; set; }

        public Ship(World world, PhysicsGameScreen screen)
        {
            var vertices = new Vertices(new List<Vector2>() 
            {
                new Vector2(0, -20),
                new Vector2(10, 5),
                new Vector2(-10, 5),
            });

            Body = BodyFactory.CreateBody(world);
            Body.BodyType = BodyType.Dynamic;

            FixtureFactory.AttachPolygon(vertices, 1, Body);

            AssetCreator creator = screen.ScreenManager.Assets;
            _shipSprite = new Sprite(creator.TextureFromVertices(vertices, MaterialType.Dots, Color.SaddleBrown, 2f));
            _lineSprite = new Sprite(creator.TextureFromVertices(vertices, MaterialType.Dots, Color.SaddleBrown, 2f));

            MaximumSpeed = 300;
            MaximumThrust = 1000;
            MaximumReverseThrust = 1000;
            RotationRate = (float) Math.PI * 2;
        }

        public float Radius { get { return 25; } }

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

        public float MaximumSpeed { get; set; }

        public float MaximumThrust { get; set; }

        public float MaximumReverseThrust { get; set; }

        public float RotationRate { get; set; }
        
        public void LimitVelocity()
        {
            if (Body.LinearVelocity.Length() > MaximumSpeed)
            {
                Body.LinearVelocity.Normalize();
                Body.LinearVelocity *= MaximumSpeed;
            }
        }

        public Vector2 Thrust { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            //var v1 = Vector2.Transform(new Vector2(0, -20), Matrix.CreateRotationZ(Body.Rotation)) + Body.Position;
            //var v2 = Vector2.Transform(new Vector2(10, 5), Matrix.CreateRotationZ(Body.Rotation)) + Body.Position;
            //var v3 = Vector2.Transform(new Vector2(-10, 5), Matrix.CreateRotationZ(Body.Rotation)) + Body.Position;

            //_lineBrush.Color = Color.Black;
            //_lineBrush.Draw(spriteBatch, v1, v2);
            //_lineBrush.Draw(spriteBatch, v2, v3);
            //_lineBrush.Draw(spriteBatch, v3, v1);

            //_lineBrush.Color = Color.Blue;
            //_lineBrush.Draw(spriteBatch, Body.Position, Body.Position + Velocity);

            //var thrust = Thrust;
            //thrust.Normalize();
            //thrust *= 100;
            //_lineBrush.Color = Color.Green;
            //_lineBrush.Draw(spriteBatch, Body.Position, Body.Position + thrust);
        }
    }
}
