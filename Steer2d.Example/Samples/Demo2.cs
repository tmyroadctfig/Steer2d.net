//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using DemoBaseXNA.ScreenSystem;
//using DemoBaseXNA.DrawingSystem;
//using FarseerGames.FarseerPhysics.Dynamics;
//using FarseerGames.FarseerPhysics;
//using Microsoft.Xna.Framework;
//using DemoBaseXNA;
//using Microsoft.Xna.Framework.Graphics;
//using FarseerGames.FarseerPhysics.Factories;
//using Microsoft.Xna.Framework.Input;
//using Steer2d.Example.Entities;
//using Steer2d.Utility;

//namespace Steer2d.Example.Demos
//{
//    public class Demo2 : GameScreen
//    {
//        private SpriteFont _spriteFont;
//        private LineBrush _lineBrush;
//        private Ship _playerShip;
//        private Ship _otherShip;

//        public override void Initialize()
//        {
//            PhysicsSimulator = new PhysicsSimulator(new Vector2(0, 0));
//            PhysicsSimulatorView = new PhysicsSimulatorView(PhysicsSimulator);
            
//            base.Initialize();
//        }

//        public override void LoadContent()
//        {
//            //load texture that will visually represent the physics body
//            _lineBrush = new LineBrush(2, Color.Black);
//            _lineBrush.Load(ScreenManager.GraphicsDevice);

//            _spriteFont = ScreenManager.ContentManager.Load<SpriteFont>(@"Content\Fonts\diagnosticFont");
            
//            _playerShip = new Ship(PhysicsSimulator);
//            _playerShip.Position = ScreenManager.ScreenCenter;

//            _otherShip = new Ship(PhysicsSimulator);
//            _otherShip.Position = ScreenManager.ScreenCenter + new Vector2(50, 0);
//            _otherShip.Body.IsQuadraticDragEnabled = true;
//            _otherShip.MaximumThrust = 50;

//            base.LoadContent();
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            
//            _playerShip.Draw(ScreenManager.SpriteBatch, _lineBrush);
//            _otherShip.Draw(ScreenManager.SpriteBatch, _lineBrush);

//            ScreenManager.SpriteBatch.DrawString(_spriteFont, _angleInfo, _otherShip.Position + new Vector2(20, 20), Color.Black);

//            ScreenManager.SpriteBatch.End();

//            base.Draw(gameTime);
//        }

//        public override void HandleInput(InputState input, GameTime gameTime)
//        {
//            if (firstRun)
//            {
//                ScreenManager.AddScreen(new PauseScreen(GetTitle(), GetDetails()));
//                firstRun = false;
//            }

//            if (input.PauseGame)
//            {
//                ScreenManager.AddScreen(new PauseScreen(GetTitle(), GetDetails()));
//            }

//            if (input.CurrentGamePadState.IsConnected)
//            {
//                HandleGamePadInput(input);
//            }
//            else
//            {
//                HandleKeyboardInput(input, gameTime);
//            }

//            base.HandleInput(input, gameTime);
//        }

//        private void HandleGamePadInput(InputState input)
//        {
//            Vector2 force = 500 * input.CurrentGamePadState.ThumbSticks.Left;
//            force.Y = -force.Y;
//            force = Vector2.Transform(force, Matrix.CreateRotationZ(_playerShip.Body.Rotation));
//            _playerShip.Body.ApplyForce(force);

//            float rotation = -1000 * input.CurrentGamePadState.Triggers.Left;
//            _playerShip.Body.ApplyTorque(rotation);

//            rotation = 1000 * input.CurrentGamePadState.Triggers.Right;
//            _playerShip.Body.ApplyTorque(rotation);
//        }

//        private void HandleKeyboardInput(InputState input, GameTime gameTime)
//        {
//            const float forceAmount = 5000;
//            Vector2 force = Vector2.Zero;
//            force.Y = -force.Y;

//            if (input.CurrentKeyboardState.IsKeyDown(Keys.Up)) { force += new Vector2(0, -forceAmount); }
//            if (input.CurrentKeyboardState.IsKeyDown(Keys.S)) { force += new Vector2(0, forceAmount); }
//            if (input.CurrentKeyboardState.IsKeyDown(Keys.Down)) { force += new Vector2(0, forceAmount); }
//            if (input.CurrentKeyboardState.IsKeyDown(Keys.W)) { force += new Vector2(0, -forceAmount); }

//            force = Vector2.Transform(force, Matrix.CreateRotationZ(_playerShip.Body.Rotation));
//            _playerShip.Thrust = force;
//            _playerShip.Body.ApplyForce(force);

//            //const float torqueAmount = 1000;
//            //float torque = 0;

//            //if (input.CurrentKeyboardState.IsKeyDown(Keys.Left)) { torque -= torqueAmount; }
//            //if (input.CurrentKeyboardState.IsKeyDown(Keys.Right)) { torque += torqueAmount; }

//            //_playerShip.Body.ApplyTorque(torque);

//            if (input.CurrentKeyboardState.IsKeyDown(Keys.Left) ||
//                input.CurrentKeyboardState.IsKeyDown(Keys.Right))
//            {
//                _playerShip.Body.AngularVelocity = 0;
//                _playerShip.Body.ClearTorque();

//                int rotateAmount = 0;

//                if (input.CurrentKeyboardState.IsKeyDown(Keys.Left)) { rotateAmount -= 180; }
//                if (input.CurrentKeyboardState.IsKeyDown(Keys.Right)) { rotateAmount += 180; }

//                _playerShip.Body.Rotation += MathHelper.ToRadians((float)(rotateAmount * gameTime.ElapsedGameTime.TotalSeconds));
//            }



//            Vector2 steeringForce = Steering.Pursue(_otherShip, _playerShip);

//            if (!float.IsNaN(steeringForce.X) &&
//                !float.IsNaN(steeringForce.Y) && 
//                steeringForce.Length() != 0)
//            {
//                if (steeringForce.Length() > 1.001f)
//                {
//                    steeringForce.Normalize();
//                }

//                // Apply rotation
//                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
//                var direction = Vector2.Transform(new Vector2(0, -1), Matrix.CreateRotationZ(_otherShip.Body.Rotation)); //new Vector2((float)Math.Sin(_otherShip.Body.Rotation), -(float)Math.Cos(_otherShip.Body.Rotation));

//                float angle = VectorUtils.FindAngleBetweenTwoVectors(direction, steeringForce);
//                _angleInfo = string.Format("{0}deg", MathHelper.ToDegrees(angle));

//                if (!float.IsNaN(angle))
//                {
//                    //angle = MathHelper.Clamp(angle, -elapsedTime * 4, elapsedTime * 4);
//                    _otherShip.Body.Rotation -= angle;

//                    _otherShip.Body.AngularVelocity = 0;
//                    _otherShip.Body.ClearTorque();
//                }

//                // Apply thrust
//                float thrustMagnitude = Vector2.Dot(direction, steeringForce);
//                var thrust = direction;
//                thrust.Normalize();
//                thrust *= thrustMagnitude * 5000;
//                _otherShip.Thrust = thrust;

//                //_otherShip.Body.LinearVelocity = Vector2.Zero;
//                _otherShip.Body.LinearVelocity += thrust * elapsedTime;
//            }

//            _playerShip.LimitVelocity();
//            _otherShip.LimitVelocity();
//        }

//        string _angleInfo = "";

//        public static string GetTitle()
//        {
//            return "Demo2: A Single Pursuer";
//        }

//        public static string GetDetails()
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.AppendLine("This demo shows a single body with no geometry");
//            sb.AppendLine("attached. Note that it does not collide with the borders.");
//            sb.AppendLine(string.Empty);
//            sb.AppendLine("GamePad:");
//            sb.AppendLine("  -Rotate: left and right triggers");
//            sb.AppendLine("  -Move: left thumbstick");
//            sb.AppendLine(string.Empty);
//            sb.AppendLine("Keyboard:");
//            sb.AppendLine("  -Rotate: left and right arrows");
//            sb.AppendLine("  -Move: A,S,D,W");
//            return sb.ToString();
//        }
//    }
//}
