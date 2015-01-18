#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Net;
using System.Threading;
using System;
using System.Diagnostics;

#endregion

namespace StarCheckersWindows
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Thread.Sleep(2000);
            SyncClient client = new SyncClient(IPAddress.Parse("192.168.0.11"), 8888);
//            SyncClient client = new SyncClient(IPAddress.Parse("25.122.152.24"), 8888);
            client.StartClient();
            Console.WriteLine(Process.GetCurrentProcess().Id);

//            client.SendAndReceiveMessage("start");
            client.ReceiveMessage();

            client.SendAndReceiveMessage(Process.GetCurrentProcess().Id +"initial message");
            client.SendAndReceiveMessage("other message");
            string ans = client.SendAndReceiveMessage("another message");
            if (ans != "yet antoher message")
                client.SendAndReceiveMessage("yet antoher message");
            else 
                client.SendMessage("yet antoher message");
            client.SendMessage("end");
            client.StopClient();

//            using  (var netManager = new NetworkManager(IPAddress.Parse("25.122.152.24"), 8888))
//            {
//                netManager.SendReceiveMessage(NetworkMessageType.OK, true);
//            }


            #if ANDROID
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.IsFullScreen = true;
            #else
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 640;
            graphics.IsFullScreen = false;
            #endif

            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.Instance.GraphicsDevice = GraphicsDevice;
            ScreenManager.Instance.SpriteBatch = spriteBatch;
            #if ANDROID
            ScreenManager.Instance.Dimensions = new Vector2 (TouchPanel.DisplayWidth, TouchPanel.DisplayHeight);
            this.Window.Touch += (sender, e) =>  
            {
            ScreenManager.Instance.CursorImage.Position.X =  (int) e.Event.GetX();
            ScreenManager.Instance.CursorImage.Position.Y =  (int) e.Event.GetY();

            InputManager.Instance.SetTouch((int)e.Event.GetX(), (int)e.Event.GetY());
            };
            #else
            ScreenManager.Instance.Dimensions = new Vector2 (960, 640);
            #endif
            ScreenManager.Instance.LoadContent(Content);


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            ScreenManager.Instance.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ScreenManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            ScreenManager.Instance.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
