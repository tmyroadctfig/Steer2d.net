using System;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.SamplesFramework
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FarseerPhysicsGame : Game
    {
        private GraphicsDeviceManager _graphics;

        public FarseerPhysicsGame()
        {
            Window.Title = "Farseer Samples Framework";
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferMultiSampling = true;
#if WINDOWS || XBOX
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            ConvertUnits.SetDisplayUnitToSimUnitRatio(24f);
            IsFixedTimeStep = true;
#elif WINDOWS_PHONE
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
            ConvertUnits.SetDisplayUnitToSimUnitRatio(16f);
            IsFixedTimeStep = false;
#endif
#if WINDOWS
            _graphics.IsFullScreen = false;
#elif XBOX || WINDOWS_PHONE
            _graphics.IsFullScreen = true;
#endif

            Content.RootDirectory = "Content";

            //new-up components and add to Game.Components
            ScreenManager = new ScreenManager(this);
            Components.Add(ScreenManager);

            FrameRateCounter frameRateCounter = new FrameRateCounter(ScreenManager);
            frameRateCounter.DrawOrder = 101;
            Components.Add(frameRateCounter);
        }

        public ScreenManager ScreenManager { get; set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var demo1 = new Demo1();
            var demo2 = new Demo2();
            var demo3 = new Demo3();
            var demo4 = new Demo4();
            var demo5 = new Demo5();
            var demo6 = new Demo6();

            MenuScreen menuScreen = new MenuScreen("Farseer Samples");

            menuScreen.AddMenuItem("Simple Samples", EntryType.Separator, null);
            menuScreen.AddMenuItem(demo1.GetTitle(), EntryType.Screen, demo1);
            menuScreen.AddMenuItem(demo2.GetTitle(), EntryType.Screen, demo2);
            menuScreen.AddMenuItem(demo3.GetTitle(), EntryType.Screen, demo3);
            menuScreen.AddMenuItem(demo4.GetTitle(), EntryType.Screen, demo4);
            menuScreen.AddMenuItem(demo5.GetTitle(), EntryType.Screen, demo5);
            menuScreen.AddMenuItem(demo6.GetTitle(), EntryType.Screen, demo6);

            menuScreen.AddMenuItem("", EntryType.Separator, null);
            menuScreen.AddMenuItem("Exit", EntryType.ExitItem, null);

            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(menuScreen);
            ScreenManager.AddScreen(new LogoScreen(TimeSpan.FromSeconds(3.0)));
        }
    }
}