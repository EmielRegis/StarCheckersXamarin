using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StarCheckersWindows
{
    public class GameScreen
    {
        protected ContentManager content;
        [XmlIgnore]
        public Type Type { get; set; }
        public string XmlPath { get; set; }

        public GameScreen()
        {
            Type = GetType();
            XmlPath = "Load/" + Type.ToString().Replace("StarCheckersWindows.", "") + ".xml";
        }

        public virtual void LoadContent()
        {
            this.content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
        }

        public virtual void UnloadContent()
        {
            content.Unload();
        }

        public virtual void Update(GameTime gameTime)
        {
            InputManager.Instance.Update();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}