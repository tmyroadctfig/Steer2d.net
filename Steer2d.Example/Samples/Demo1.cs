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
        private Body _rectangle;
        private Sprite _rectangleSprite;
        private Ship _ship;
        private Random _random = new Random();

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            UpdateShip(gameTime);

            base.HandleInput(input, gameTime);
        }

        Vector2 ScreenCenter
        {
            get
            {
                return new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width,
                    ScreenManager.GraphicsDevice.Viewport.Height) / 2;
            }
        }

        private void ResetShip()
        {
            _ship.Body.ResetDynamics();

            var width = ScreenManager.GraphicsDevice.Viewport.Width - 100;
            var height = ScreenManager.GraphicsDevice.Viewport.Height - 100;

            _ship.Position = ScreenCenter;

            _ship.Position += new Vector2(
                width / 2 - _random.Next(width),
                height / 2 - _random.Next(height));
        }

        private void UpdateShip(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var steering = Steering.GetComponents(_ship, Steering.Seek(_ship.Body.Position, ScreenCenter), elapsedTime);

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

            if (VectorUtils.EqualsWithin(_ship.Body.Position, ScreenCenter, 0.5f))
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

            _rectangle = BodyFactory.CreateRectangle(World, 5f, 5f, 1f);
            _rectangle.BodyType = BodyType.Dynamic;

            SetUserAgent(_rectangle, 100f, 100f);

            // create sprite based on body
            _rectangleSprite = new Sprite(ScreenManager.Assets.TextureFromShape(_rectangle.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));

            _ship = new Ship(World, this);

            ResetShip();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            ScreenManager.SpriteBatch.Draw(_rectangleSprite.Texture, ConvertUnits.ToDisplayUnits(_rectangle.Position),
                                           null,
                                           Color.White, _rectangle.Rotation, _rectangleSprite.Origin, 1f,
                                           SpriteEffects.None, 0f);

            //_ship.Draw(ScreenManager.SpriteBatch, _lineBrush);

            //// Draw the target
            //_lineBrush.Draw(
            //    ScreenManager.SpriteBatch,
            //    ScreenManager.ScreenCenter + new Vector2(0, -10),
            //    ScreenManager.ScreenCenter + new Vector2(0, 10));

            //_lineBrush.Draw(
            //     ScreenManager.SpriteBatch,
            //     ScreenManager.ScreenCenter + new Vector2(-10, 0),
            //     ScreenManager.ScreenCenter + new Vector2(10, 0));

            //ScreenManager.SpriteBatch.DrawString(
            //    _spriteFont,
            //    string.Format("x:{0:0.00} y:{1:0.00}", _ship.Position.X, _ship.Position.Y),
            //    new Vector2(40, 40),
            //    Color.White);

            ScreenManager.SpriteBatch.End();
            _border.Draw();
            base.Draw(gameTime);
        }
    }
}