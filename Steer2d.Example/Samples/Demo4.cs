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
    /// Demo4 evade.
    /// </summary>
    internal class Demo4 : PhysicsGameScreen, IDemoScreen
    {
        private bool OnlyUpdateOnKeyPress { get; set; }
        private Border _border;
        private Ship _ship1;
        private Steering _steering1;
        private Ship _ship2;
        private Steering _steering2;
        private Random _random = new Random();
        private Vector2 _target = Vector2.Zero;

        public Demo4()
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

            if (VectorUtils.EqualsWithin(_ship1.Position, _ship2.Position, 2.0f))
            {
                ResetShips();
            }

            if (VectorUtils.EqualsWithin(_ship1.Position, _target, 0.5f))
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

        private void ResetShips()
        {
            _ship1.Body.ResetDynamics();
            _ship2.Body.ResetDynamics();

            var width = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width - 100);
            var height = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height - 100);

            _ship1.Position = new Vector2(
                width / 2 - _random.Next((int)width),
                height / 2 - _random.Next((int)height));

            _ship2.Position = new Vector2(
                width / 2 - _random.Next((int)width),
                height / 2 - _random.Next((int)height));
        }

        private void UpdateShip(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((_ship1.Position - _ship2.Position).Length() < 15)
            {
                var steeringComponents1 = _steering1.Evade(_ship2, elapsedTime);
                _ship1.ApplySteering(steeringComponents1);
            }
            else
            {
                var steeringComponents1 = _steering1.Seek(_target, elapsedTime);
                _ship1.ApplySteering(steeringComponents1);
            }

            var steeringComponents2 = _steering2.Pursue(_ship1, elapsedTime);
            _ship2.ApplySteering(steeringComponents2);
        }

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Demo4: Evade";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows a vehicle (ship1 - cyan) evading another vehicle");
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
            _ship1.MaximumSpeed *= 0.5f;
            _steering1 = new RotationPreferencedSteering(_ship1);

            _ship2 = new Ship(World);
            _ship2.Colour = Color.Magenta;
            _ship2.MaximumSpeed *= 0.5f;
            _steering2 = new ThrustPreferencedSteering(_ship2);

            ResetShips();
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

            ScreenManager.SpriteBatch.End();

            _border.Draw();

            ScreenManager.LineBatch.Begin(Camera.SimProjection, Camera.SimView);

            _ship1.Draw(ScreenManager.LineBatch);
            _ship2.Draw(ScreenManager.LineBatch);

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