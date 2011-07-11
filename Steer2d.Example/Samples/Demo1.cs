using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steer2d.Utility;
using Steer2d;
using Steer2d.Example.Entities;
using System;

namespace FarseerPhysics.SamplesFramework
{
    internal class Demo1 : PhysicsGameScreen, IDemoScreen
    {
        private Border _border;
        private Ship _ship;
        private Random _random = new Random();

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            UpdateShip(gameTime);

            base.HandleInput(input, gameTime);
        }

        private void ResetShip()
        {
            _ship.Body.ResetDynamics();

            var width = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width - 100);
            var height = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height - 100);
            
            _ship.Position += new Vector2(
                width / 2 - _random.Next((int)width),
                height / 2 - _random.Next((int)height));
        }

        private void UpdateShip(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var steering = Steering.GetComponents(_ship, Steering.Seek(_ship.Body.Position, Vector2.Zero), elapsedTime);

            if (steering.IsValid)
            {
                // Apply rotation
                _ship.Body.Rotation -= steering.Rotation;
                _ship.Body.AngularVelocity = 0;

                // Apply thrust
                var thrust = _ship.Direction;
                thrust.Normalize();
                thrust *= steering.Thrust;
                _ship.Thrust = thrust;

                _ship.Body.LinearVelocity += thrust;
            }

            _ship.LimitVelocity();

            if (VectorUtils.EqualsWithin(_ship.Body.Position, Vector2.Zero, 0.5f))
            {
                ResetShip();
            }
        }

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Demo1: Seeking to a point";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows a single vehicle seeking to a point");
            return sb.ToString();
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = Vector2.Zero;

            _border = new Border(World, this, ScreenManager.GraphicsDevice.Viewport);

            _ship = new Ship(World);

            ResetShip();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);

            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("x:{0:0.00} y:{1:0.00}", _ship.Position.X, _ship.Position.Y),
                new Vector2(2, 2),
                Color.White);

            ScreenManager.SpriteBatch.End();


            _border.Draw();

            ScreenManager.LineBatch.Begin(Camera.SimProjection, Camera.SimView);

            _ship.Draw(ScreenManager.LineBatch);

            // Draw the target
            ScreenManager.LineBatch.DrawLine(
                new Vector2(0, -1),
                new Vector2(0, 1));

            ScreenManager.LineBatch.DrawLine(
                 new Vector2(-1, 0),
                 new Vector2(1, 0));

            ScreenManager.LineBatch.End();

            base.Draw(gameTime);
        }
    }
}