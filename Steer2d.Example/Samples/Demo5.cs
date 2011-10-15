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
using System.Collections.Generic;
using Steer2d.FarseerPhysicsImpl;

namespace FarseerPhysics.SamplesFramework
{
    /// <summary>
    /// Demo5 avoid.
    /// </summary>
    internal class Demo5 : PhysicsGameScreen, IDemoScreen
    {
        private bool OnlyUpdateOnKeyPress { get; set; }
        private Border _border;
        private Ship _ship1;
        private Steering _steering1;
        private SimpleAvoidAi _ship1Ai;
        private Ship _ship2;
        private Steering _steering2;
        private SimpleAvoidAi _ship2Ai;
        private Random _random = new Random();
        private Vector2 _target = Vector2.Zero;
        private IList<Obstacle> _obstacles = new List<Obstacle>();

        public Demo5()
        {
            OnlyUpdateOnKeyPress = false;

            Settings.UseFPECollisionCategories = true;
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            base.HandleInput(input, gameTime);

            if (OnlyUpdateOnKeyPress && input.IsNewKeyPress(Keys.N))
            {
                UpdateShip(gameTime);
            }

            if (VectorUtils.EqualsWithin(_ship1.Position, _target, 0.5f) ||
                VectorUtils.EqualsWithin(_ship2.Position, _target, 0.5f))
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

            _ship1Ai.Update(elapsedTime, _target, _obstacles);
            _ship2Ai.Update(elapsedTime, _target, _obstacles);
        }

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Demo5: Avoid Obstacles";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows a vehicles avoiding obstacles");
            return sb.ToString();
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = Vector2.Zero;

            _border = new Border(World, this, ScreenManager.GraphicsDevice.Viewport);
            _border.CollisionCategories = Category.Cat4;
            _border.CollidesWith = Category.All;

            _ship1 = new Ship(World);
            _ship1.Colour = Color.Cyan;
            _ship1.MaximumSpeed *= 0.5f;
            _ship1.Body.CollisionCategories = Category.Cat10;
            _steering1 = new RotationPreferencedSteering(_ship1);
            _steering1.PotentialCollisionDetector = new PotentialCollisionDetector(_ship1.Body, Category.Cat3, Category.Cat2);
            _ship1Ai = new SimpleAvoidAi(_ship1, _steering1);

            _ship2 = new Ship(World);
            _ship2.Colour = Color.Magenta;
            _ship2.MaximumSpeed *= 0.5f;
            _ship2.Body.CollisionCategories = Category.Cat10;
            _steering2 = new ThrustPreferencedSteering(_ship2);
            _steering2.PotentialCollisionDetector = new PotentialCollisionDetector(_ship2.Body, Category.Cat3, Category.Cat2);
            _ship2Ai = new SimpleAvoidAi(_ship2, _steering2);

            var width = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width - 100);
            var height = ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height - 100);

            for (int i = 0; i < 10; i++)
            {
                var obstacle = new Obstacle(World, (float)_random.NextDouble() + 0.25f);
                obstacle.Body.Position = new Vector2(
                    width / 2 - _random.Next((int)width),
                    height / 2 - _random.Next((int)height));

                obstacle.Body.CollisionCategories = Category.Cat2;

                obstacle.LoadContent(this);
                _obstacles.Add(obstacle);
            }

            ResetShips();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            
            foreach (var o in _obstacles)
            {
                o.Draw(ScreenManager.SpriteBatch);
            }

            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("ship1 - rotational x:{0:0.00} y:{1:0.00} ai:{2}", _ship1.Position.X, _ship1.Position.Y, _ship1Ai._aiState),
                new Vector2(2, 2),
                _ship1.Colour);
            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.Fonts.DetailsFont,
                string.Format("ship2 - thrust     x:{0:0.00} y:{1:0.00} ai:{2}", _ship2.Position.X, _ship2.Position.Y, _ship2Ai._aiState),
                new Vector2(2, 22),
                _ship2.Colour);

            ScreenManager.SpriteBatch.End();

            _border.Draw();

            ScreenManager.LineBatch.Begin(Camera.SimProjection, Camera.SimView);

            _ship1.Draw(ScreenManager.LineBatch);
            _ship2.Draw(ScreenManager.LineBatch);

            _ship1Ai.Draw(ScreenManager.LineBatch);
            _ship2Ai.Draw(ScreenManager.LineBatch);

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

    public class SimpleAvoidAi
    {
        Ship _ship;

        Steering _steering;

        Vector2 _avoidVector;

        public AiState _aiState;

        public SimpleAvoidAi(Ship ship, Steering steering)
        {
            _ship = ship;
            _steering = steering;
            _aiState = AiState.SeekTarget;
        }

        public void Update(float elapsedTime, Vector2 target, IEnumerable<IObstacle> obstacles)
        {
            var steeringComponents = _steering.SteerToAvoidObstacles(obstacles, 1.0f, elapsedTime);

            if (steeringComponents.Equals(SteeringComponents.NoSteering))
            {
                if (_aiState == AiState.Avoid && (_ship.Position - _avoidVector).Length() > 0.5f)
                {
                    steeringComponents = _steering.Seek(_avoidVector, elapsedTime);
                }
                else
                {
                    _aiState = AiState.SeekTarget;
                    steeringComponents = _steering.Seek(target, elapsedTime);
                }
            }
            else
            {
                _aiState = AiState.Avoid;
                _avoidVector = steeringComponents.SteeringTarget;
            }

            _ship.ApplySteering(steeringComponents);
        }

        public void Draw(LineBatch lineBatch)
        {
            lineBatch.DrawLine(_ship.Position, _avoidVector, Color.Red);
        }
    }

    public enum AiState
    {
        SeekTarget,

        Avoid
    }
}