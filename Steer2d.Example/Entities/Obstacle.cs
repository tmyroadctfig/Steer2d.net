using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework.Graphics;
using Steer2d.FarseerPhysicsImpl;

namespace Steer2d.Example.Entities
{
    public class Obstacle : IObstacle, IFarseerEntity
    {
        public Body Body { get; set; }
        Sprite _circle;

        public Obstacle(World world, float radius)
        {
            Radius = radius;

            Body = BodyFactory.CreateBody(world);
            Body.BodyType = BodyType.Dynamic;
            Body.UserData = this;

            var fixture = FixtureFactory.AttachCircle(Radius, 1, Body);
            fixture.UserData = this;
        }

        public float Radius { get; set; }

        public Vector2 Position { get { return Body.Position; } }

        public void LoadContent(PhysicsGameScreen screen)
        {
            AssetCreator creator = screen.ScreenManager.Assets;
            _circle = new Sprite(creator.CircleTexture(Radius, MaterialType.Blank, Color.Gray, 1f));
        }

        public void Draw(SpriteBatch batch)
        {
            var displayVector = ConvertUnits.ToDisplayUnits(Body.Position + new Vector2(-Radius, Radius));
            var p = new Vector2(displayVector.X, -displayVector.Y);
            batch.Draw(_circle.Texture, p, Color.White);
        }
    }
}
