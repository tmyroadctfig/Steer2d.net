using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.SamplesFramework;
using Steer2d;
using Steer2d.Example.Entities;
using Microsoft.Xna.Framework;
using Steer2d.Utility;
using Microsoft.Xna.Framework.Input;

namespace FarseerPhysics.SamplesFramework
{
    /// <summary>
    /// Demo6 flocking.
    /// </summary>
    public class Demo6 : PhysicsGameScreen, IDemoScreen
    {
        private int _flockSize = 50;

        private bool OnlyUpdateOnKeyPress { get; set; }
        private Dictionary<Ship, Steering> _flockingShips = new Dictionary<Ship, Steering>();
        private Random _random = new Random();

        public Demo6()
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
            var width = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width - 100);
            var height = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height - 100);

            foreach (var ship in _flockingShips.Keys)
            {
                ship.Body.ResetDynamics();

                ship.Position = new Vector2(
                    width / 2 - _random.Next((int)width),
                    height / 2 - _random.Next((int)height));
            }
        }
        
        private void UpdateShip(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        
            foreach (var ship in _flockingShips.Keys)
            {
                SteeringComponents steeringComponents = _flockingShips[ship].Flock(_flockingShips.Keys, elapsedTime);
                ship.ApplySteering(steeringComponents);
            }
        }

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Demo6: Flocking";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows vehicles flocking");
            return sb.ToString();
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = Vector2.Zero;

            for (int i = 0; i < _flockSize; i++)
            {
                var ship = new Ship(World);
                ship.Colour = Color.Black;
                ship.MaximumSpeed *= 0.5f;
                var steering = new Steering(ship, new RotationPreferencedSteering());

                _flockingShips.Add(ship, steering);
            }

            ResetShip();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            
            int index = 0;
            foreach (var ship in _flockingShips.Keys)
            {
                ScreenManager.SpriteBatch.DrawString(
                    ScreenManager.Fonts.DetailsFont,
                    string.Format("ship{0} - rotational x:{1:0.00} y:{2:0.00} {3}", index, ship.Position.X, ship.Position.Y, ship.LastSteeringObjective),
                    new Vector2(2, index * 20),
                    Color.Black);

                index++;
            }

            ScreenManager.SpriteBatch.End();
            
            ScreenManager.LineBatch.Begin(Camera.SimProjection, Camera.SimView);

            var width = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width);
            var height = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height);
            var halfViewport = new Vector2(width / 2, height / 2);

            foreach (var ship in _flockingShips.Keys)
            {
                var position = new Vector2(ship.Position.X % width, ship.Position.Y % height) - halfViewport;
                
                var v1 = Vector2.Transform(new Vector2(0, -ConvertUnits.ToSimUnits(20)), Matrix.CreateRotationZ(ship.Body.Rotation)) + position;
                var v2 = Vector2.Transform(new Vector2(ConvertUnits.ToSimUnits(10), ConvertUnits.ToSimUnits(5)), Matrix.CreateRotationZ(ship.Body.Rotation)) + position;
                var v3 = Vector2.Transform(new Vector2(ConvertUnits.ToSimUnits(-10), ConvertUnits.ToSimUnits(5)), Matrix.CreateRotationZ(ship.Body.Rotation)) + position;

                ScreenManager.LineBatch.DrawLine(v1, v2, Color.Black);
                ScreenManager.LineBatch.DrawLine(v2, v3, Color.Black);
                ScreenManager.LineBatch.DrawLine(v3, v1, Color.Black);

                ScreenManager.LineBatch.DrawLine(position, position + ship.Body.LinearVelocity, Color.Blue);
            }

            ScreenManager.LineBatch.End();

            base.Draw(gameTime);
        }
    }
}
