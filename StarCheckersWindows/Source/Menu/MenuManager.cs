using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarCheckersWindows
{
    public  class MenuManager
    {
        private Menu menu;
        private bool isTransitioning;
        

        public MenuManager()
        {
            menu = new Menu();
            menu.OnMenuChange += menu_OnMenuChange;
        }

        public void Transition(GameTime gameTime)
        {
            if(isTransitioning)
            {
                menu.Items.ForEach(i =>
                {
                    i.Image.Update(gameTime);
                    float first = menu.Items[0].Image.Alpha;
                    float last = menu.Items[menu.Items.Count - 1].Image.Alpha;

                    if (first == 0.0f && last == 0.0f)
                        menu.Id = menu.Items[menu.ItemNumber].LinkId;
                    else if (first == 1.0f && last == 1.0f)
                    {
                        isTransitioning = false;
                        menu.Items.ForEach(ii => ii.Image.RestoreEffects());
                    }
                });
            }
        }

        void menu_OnMenuChange(object sender, EventArgs e)
        {
            XmlManager<Menu> xmlmenManager = new XmlManager<Menu>();
            menu.UnloadContent();
            menu = xmlmenManager.Load(menu.Id);
            menu.LoadContent();

            menu.OnMenuChange += menu_OnMenuChange;
            menu.Transition(0.0f);

            menu.Items.ForEach(i =>
                    {
                        i.Image.StoreEffects();
                        i.Image.ActivateEffect("FadeEffect");
                    });
        }

        public void LoadContent(string menuPath)
        {
            if (menuPath != String.Empty)
                menu.Id = menuPath;
        }

        public void UnloadContent()
        {
            menu.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if(!isTransitioning)
                menu.Update(gameTime);

            if (InputManager.Instance.KeyPressed(Keys.Enter) && !isTransitioning)
            {
                if (menu.Items[menu.ItemNumber].LinkType == "Screen")
                    ScreenManager.Instance.ChangeScreens(menu.Items[menu.ItemNumber].LinkId);
                else
                {
                    isTransitioning = true;
                    menu.Transition(1.0f);    
                    menu.Items.ForEach(i =>
                    {
                        i.Image.StoreEffects();
                        i.Image.ActivateEffect("FadeEffect");
                    });
                }
            }
            Transition(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            menu.Draw(spriteBatch);
        }
    }
}
