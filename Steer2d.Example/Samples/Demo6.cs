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
        private bool OnlyUpdateOnKeyPress { get; set; }
        private Border _border;
        private Ship _ship1;
        private Steering _steering1;
        private Dictionary<Ship, Steering> _flockingShips = new Dictionary<Ship, Steering>();
        private Random _random = new Random();
        private Vector2 _target = Vector2.Zero;

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

        private void ResetShip()
        {
            _ship1.Body.ResetDynamics();

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
            
            var steeringComponents1 = _steering1.Seek(_target, elapsedTime);
            _ship1.ApplySteering(steeringComponents1);

            IList<Ship> flock = new List<Ship>(_flockingShips.Keys);
            flock.Add(_ship1);

            bool pursue = ((int)(gameTime.TotalGameTime.TotalSeconds * 10)) % 2 == 0;

            foreach (var ship in _flockingShips.Keys)
            {
                SteeringComponents steeringComponents;


                if (pursue)
                {
                    steeringComponents = _flockingShips[ship].Pursue(_ship1, elapsedTime);
                }
                else
                {
                    steeringComponents = _flockingShips[ship].Flock(flock, elapsedTime);
                }

                //steeringComponents = _flockingShips[ship].Flock(flock, elapsedTime);

                //if (!steeringComponents.IsValid)
                //{
                //    steeringComponents = _flockingShips[ship].Pursue(_ship1, elapsedTime);
                //}

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

            _border = new Border(World, this, ScreenManager.GraphicsDevice.Viewport);

            _ship1 = new Ship(World);
            _ship1.Colour = Color.Cyan;
            _ship1.MaximumSpeed *= 0.6f;
            _steering1 = new Steering(_ship1, new RotationPreferencedSteering());

            for (int i = 0; i < 5; i++)
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

            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("ship1 - rotational x:{0:0.00} y:{1:0.00}", _ship1.Position.X, _ship1.Position.Y),
                new Vector2(2, 2),
                _ship1.Colour);

            int index = 0;
            foreach (var ship in _flockingShips.Keys)
            {
                ScreenManager.SpriteBatch.DrawString(
                    ScreenManager.Fonts.DetailsFont,
                    string.Format("ship{0} - rotational x:{1:0.00} y:{2:0.00} {3}", index, ship.Position.X, ship.Position.Y, ship.LastSteeringObjective),
                    new Vector2(2, 20 + index * 20),
                    Color.Black);

                index++;
            }

            ScreenManager.SpriteBatch.End();

            _border.Draw();

            ScreenManager.LineBatch.Begin(Camera.SimProjection, Camera.SimView);

            _ship1.Draw(ScreenManager.LineBatch);

            foreach (var ship in _flockingShips.Keys)
            {
                ship.Draw(ScreenManager.LineBatch);
            }

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
