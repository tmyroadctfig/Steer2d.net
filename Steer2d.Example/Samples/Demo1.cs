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
    internal class Demo1 : PhysicsGameScreen, IDemoScreen
    {
        private bool OnlyUpdateOnKeyPress { get; set; }
        private Border _border;
        private Ship _ship1;
        private RotationPreferencedSteering _steering1;
        private Ship _ship2;
        private RotationPreferencedSteering _steering2;
        private Random _random = new Random();

        public Demo1()
        {
            OnlyUpdateOnKeyPress = true;
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            base.HandleInput(input, gameTime);

            if (OnlyUpdateOnKeyPress && input.IsNewKeyPress(Keys.Space))
            {
                UpdateShip(gameTime);
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

            var width = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width - 100);
            var height = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height - 100);
            
            _ship1.Position += new Vector2(
                width / 2 - _random.Next((int)width),
                height / 2 - _random.Next((int)height));

            _ship2.Position += new Vector2(
                width / 2 - _random.Next((int)width),
                height / 2 - _random.Next((int)height));
        }

        private void UpdateShip(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var steeringComponents1 = _steering1.Seek(Vector2.Zero, elapsedTime);
            _ship1.ApplySteering(steeringComponents1);

            var steeringComponents2 = _steering2.Seek(Vector2.Zero, elapsedTime);
            _ship2.ApplySteering(steeringComponents2);

            //if (VectorUtils.EqualsWithin(_ship.Body.Position, Vector2.Zero, 0.5f))
            //{
            //    ResetShip();
            //}
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

            _ship1 = new Ship(World);
            _steering1 = new RotationPreferencedSteering(_ship1);

            _ship2 = new Ship(World);
            _steering2 = new RotationPreferencedSteering(_ship2);

            ResetShip();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);

            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("x:{0:0.00} y:{1:0.00}", _ship1.Position.X, _ship1.Position.Y),
                new Vector2(2, 2),
                Color.White);
            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("x:{0:0.00} y:{1:0.00}", _ship2.Position.X, _ship2.Position.Y),
                new Vector2(2, 22),
                Color.White);

            ScreenManager.SpriteBatch.End();


            _border.Draw();

            ScreenManager.LineBatch.Begin(Camera.SimProjection, Camera.SimView);

            _ship1.Draw(ScreenManager.LineBatch);
            _ship2.Draw(ScreenManager.LineBatch);

            // Draw the target
            ScreenManager.LineBatch.DrawLine(
                new Vector2(0, 0),
                new Vector2(0, 10));

            ScreenManager.LineBatch.DrawLine(
                 new Vector2(0, 0),
                 new Vector2(10, 0));

            ScreenManager.LineBatch.End();

            base.Draw(gameTime);
        }
    }
}