using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Runtime.Remoting.Messaging;


namespace StarCheckersWindows
{

    public sealed class ScreenManager
    {
        private static ScreenManager instance;
        [XmlIgnore]
        public Vector2 Dimensions { get;  set; }
        [XmlIgnore]
        public ContentManager Content { get; private set; }
        [XmlIgnore]
        public GraphicsDevice GraphicsDevice { get; set; }
        [XmlIgnore]
        public SpriteBatch SpriteBatch { get; set; }
        public Image Image { get; set; }

        private Action applicationExitAction;

        [XmlIgnore]
        public bool IsTransitioning { get; private set; }

        public bool IsLandscape
        {
            get { return (Dimensions.X > Dimensions.Y); } 
        }

        public Image CursorImage { get; set; }

        private GameScreen currentScreen;
        private GameScreen newScreen;

        private XmlManager<GameScreen> xmlGameScreenManager; 

        private ScreenManager()
        {
            Dimensions = new Vector2(960, 640);

            xmlGameScreenManager = new XmlManager<GameScreen>();

//            currentScreen = new SplashScreen();
			currentScreen = new TitleScreen();

			xmlGameScreenManager.Type = currentScreen.Type;
			currentScreen = xmlGameScreenManager.Load(currentScreen.XmlPath);

        }

        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    XmlManager<ScreenManager> xml = new XmlManager<ScreenManager>();
                    xml.Type = typeof (ScreenManager);
                    instance = xml.Load("Load/ScreenManager.xml");
                }

                return instance;
            }
        }

        public void ChangeScreens(string screenName)
        {
            newScreen = (GameScreen) Activator.CreateInstance(Type.GetType("StarCheckersWindows." + screenName));
            Image.IsActive = true;
            Image.FadeEffect.Increase = true;
            Image.Alpha = 0.0f;
            IsTransitioning = true;
        }

        public bool CanExit()
        {
            return currentScreen.Type == typeof(TitleScreen);
        }

        private void Transition(GameTime gameTime)
        {
            if (IsTransitioning)
            {
                Image.Update(gameTime);

                if (Image.Alpha == 1.0f)
                {
                    currentScreen.UnloadContent();
                    currentScreen = newScreen;
                    xmlGameScreenManager.Type = currentScreen.Type;
                    if(File.Exists(currentScreen.XmlPath))
                        currentScreen = xmlGameScreenManager.Load(currentScreen.XmlPath);
                    currentScreen.LoadContent();
                }
                else if (Image.Alpha == 0.0f)
                {
                    Image.IsActive = false;
                    IsTransitioning = false;
                }
            }
        }

        public void LoadContent(ContentManager content)
        {
            Content = new ContentManager(content.ServiceProvider, "Content");
            currentScreen.LoadContent();
            Image.LoadContent();
            CursorImage = new Image {Path = "Cursors/Cursor"};
            CursorImage.LoadContent();
        }

        public void UnloadContent()
        {
            currentScreen.UnloadContent();
            Image.UnloadContent();
            CursorImage.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
            Transition(gameTime);

			#if ANDROID

			#else
            CursorImage.Position.X = Mouse.GetState().X;
            CursorImage.Position.Y = Mouse.GetState().Y;

			if(InputManager.Instance.MouseLeftButtonPressed())
				InputManager.Instance.SetTouch (Mouse.GetState ().X, Mouse.GetState ().Y); 
			#endif
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
            if(IsTransitioning)
                Image.Draw(spriteBatch);

            #if ANDROID

            #else
            if(CursorImage != null)
                CursorImage.Draw(SpriteBatch);
            #endif
        }

        public void OnApplicationExit()
        {
            if (applicationExitAction != null)
                applicationExitAction();

            ChangeScreens("TitleScreen");
        }

        public void AddOnExitApplicationAction(Action onExitAction)
        {
            this.applicationExitAction += onExitAction;
        }
    }
}