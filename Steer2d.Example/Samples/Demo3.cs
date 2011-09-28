using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steer2d.Utility;
using Steer2d;
using Steer2d.Example.Entities;
using System;
using Microsoft.Xna.Framework.Input;

namespace FarseerPhysics.SamplesFramework
{
    /// <summary>
    /// Demo3 pursuit.
    /// </summary>
    internal class Demo3 : PhysicsGameScreen, IDemoScreen
    {
        private bool OnlyUpdateOnKeyPress { get; set; }
        private Border _border;
        private Ship _ship1;
        private Steering _steering1;
        private Ship _ship2;
        private Steering _steering2;
        private Ship _ship3;
        private Steering _steering3;
        private Ship _ship4;
        private Steering _steering4;
        private Random _random = new Random();
        private Vector2 _target = Vector2.Zero;

        public Demo3()
        {
            OnlyUpdateOnKeyPress = false;
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            base.HandleInput(input, gameTime);

            if (OnlyUpdateOnKeyPress && input.IsNewKeyPress(Keys.N))
            {
                UpdateShip(gameTime);
            }

            if (VectorUtils.EqualsWithin(_ship1.Position, _target, 0.5f) ||
                VectorUtils.EqualsWithin(_ship1.Position, _ship2.Position, 0.5f) ||
                VectorUtils.EqualsWithin(_ship3.Position, _target, 0.5f) ||
                VectorUtils.EqualsWithin(_ship3.Position, _ship4.Position, 0.5f))
            {
                var width = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width - 100);
                var height = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height - 100);

                _target = new Vector2(
                    width / 2 - _random.Next((int)width),
                    height / 2 - _random.Next((int)height));
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!OnlyUpdateOnKeyPress)
            {
                UpdateShip(gameTime);
            }
        }

        private void ResetShip()
        {
            _ship1.Body.ResetDynamics();
            _ship2.Body.ResetDynamics();
            _ship3.Body.ResetDynamics();
            _ship4.Body.ResetDynamics();

            var width = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width - 100);
            var height = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height - 100);

            _ship1.Position = new Vector2(
                width / 2 - _random.Next((int)width),
                height / 2 - _random.Next((int)height));

            _ship2.Position = new Vector2(
                width / 2 - _random.Next((int)width),
                height / 2 - _random.Next((int)height));

            _ship3.Position = new Vector2(
                width / 2 - _random.Next((int)width),
                height / 2 - _random.Next((int)height));

            _ship4.Position = new Vector2(
                width / 2 - _random.Next((int)width),
                height / 2 - _random.Next((int)height));
        }

        private void UpdateShip(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            var steeringComponents1 = _steering1.Seek(_target, elapsedTime);
            _ship1.ApplySteering(steeringComponents1);

            var steeringComponents2 = _steering2.Pursue(_ship1, elapsedTime);
            _ship2.ApplySteering(steeringComponents2);

            var steeringComponents3 = _steering3.Seek(_target, elapsedTime);
            _ship3.ApplySteering(steeringComponents3);

            var steeringComponents4 = _steering4.Pursue(_ship3, elapsedTime);
            _ship4.ApplySteering(steeringComponents4);
        }

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Demo3: Pursuit";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows vehicles (ship2 - magenta & ship4 - red) pursing other vehicles");
            return sb.ToString();
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = Vector2.Zero;

            _border = new Border(World, this, ScreenManager.GraphicsDevice.Viewport);

            _ship1 = new Ship(World);
            _ship1.Colour = Color.Cyan;
            _ship1.MaximumSpeed *= 0.7f;
            _steering1 = new RotationPreferencedSteering(_ship1);

            _ship2 = new Ship(World);
            _ship2.Colour = Color.Magenta;
            _steering2 = new ThrustPreferencedSteering(_ship2);

            _ship3 = new Ship(World);
            _ship3.Colour = Color.YellowGreen;
            _ship3.MaximumSpeed *= 0.7f;
            _steering3 = new ThrustPreferencedSteering(_ship3);

            _ship4 = new Ship(World);
            _ship4.Colour = Color.Red;
            _steering4 = new RotationPreferencedSteering(_ship4);

            ResetShip();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);

            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("ship1 - rotational x:{0:0.00} y:{1:0.00}", _ship1.Position.X, _ship1.Position.Y),
                new Vector2(2, 2),
                _ship1.Colour);
            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("ship2 - thrust     x:{0:0.00} y:{1:0.00}", _ship2.Position.X, _ship2.Position.Y),
                new Vector2(2, 22),
                _ship2.Colour);
            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("ship3 - thrust     x:{0:0.00} y:{1:0.00}", _ship3.Position.X, _ship3.Position.Y),
                new Vector2(2, 42),
                _ship3.Colour);
            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("ship4 - thrust     x:{0:0.00} y:{1:0.00}", _ship4.Position.X, _ship4.Position.Y),
                new Vector2(2, 62),
                _ship4.Colour);

            ScreenManager.SpriteBatch.End();

            _border.Draw();

            ScreenManager.LineBatch.Begin(Camera.SimProjection, Camera.SimView);

            _ship1.Draw(ScreenManager.LineBatch);
            _ship2.Draw(ScreenManager.LineBatch);
            _ship3.Draw(ScreenManager.LineBatch);
            _ship4.Draw(ScreenManager.LineBatch);

            // Draw the target
            ScreenManager.LineBatch.DrawLine(
                new Vector2(0, 0) + _target,
                new Vector2(0, 5) + _target);

            ScreenManager.LineBatch.DrawLine(
                 new Vector2(0, 0) + _target,
                 new Vector2(5, 0) + _target);

            ScreenManager.LineBatch.End();

            base.Draw(gameTime);
        }
    }
}