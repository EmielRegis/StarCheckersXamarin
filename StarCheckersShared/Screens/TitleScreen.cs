using System.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StarCheckersWindows
{
    public class TitleScreen : GameScreen
    {
        public Image Image;
        private MenuManager menuManager;

        public TitleScreen()
        {
            menuManager = new MenuManager();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            menuManager.LoadContent("Load/Menus/TitleMenu.xml");
            Image.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            menuManager.UnloadContent();
            Image.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            menuManager.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Image.Draw(spriteBatch);
            menuManager.Draw(spriteBatch);
        }
    }
}
