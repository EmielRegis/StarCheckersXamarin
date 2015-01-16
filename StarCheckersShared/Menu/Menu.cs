using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarCheckersWindows
{
    public class Menu
    {
        public event EventHandler OnMenuChange;

        public string Axis { get; set; }
        public string Effects { get; set; }
        [XmlElement("Item")] public List<MenuItem> Items;

        public int ItemNumber { get; set; }

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnMenuChange(this, null);
            }
        }

        protected string id;

        public Menu()
        {
            id = String.Empty;
            ItemNumber = 0;
            Effects = String.Empty;
            Axis = "Y";
            Items = new List<MenuItem>();
        }

        public void Transition(float alpha)
        {
            Items.ForEach(i => 
            {
                i.Image.IsActive = true;
                i.Image.Alpha = alpha;
                i.Image.FadeEffect.Increase = (alpha == 0.0f);
            });
        }

        protected void AlignMenuItems()
        {
            Vector2 dimensions = Vector2.Zero;
            Items.ForEach(i => dimensions += new Vector2(i.Image.SourceRectange.Width, i.Image.SourceRectange.Height));

            dimensions = new Vector2((ScreenManager.Instance.Dimensions.X - dimensions.X)/2.0f,
                (ScreenManager.Instance.Dimensions.Y - dimensions.Y)/2.0f);

            Items.ForEach(i => { 
                i.Image.Position = (Axis == "X") 
                    ? new Vector2(dimensions.X, (ScreenManager.Instance.Dimensions.Y - i.Image.SourceRectange.Height) / 2.0f) 
                    : new Vector2((ScreenManager.Instance.Dimensions.X - i.Image.SourceRectange.Width) / 2.0f, dimensions.Y);

                dimensions += new Vector2(i.Image.SourceRectange.Width, i.Image.SourceRectange.Height);
            });
        }

        public void LoadContent()
        {
            List<string> effects = Effects.Split(':').ToList();
            Items.ForEach(i =>{
                i.Image.LoadContent();
                effects.ForEach(e => i.Image.ActivateEffect(e));
            });
            AlignMenuItems();
        }

        public void UnloadContent()
        {
            Items.ForEach(i => i.Image.UnloadContent());
        }

        public void Update(GameTime gameTime)
        {
            if (Axis == "X")
            {
                if (InputManager.Instance.KeyPressed(Keys.Right))
                    ItemNumber ++;
                else if (InputManager.Instance.KeyPressed(Keys.Left))
                    ItemNumber --;
            }
            else
            {
                if (InputManager.Instance.KeyPressed(Keys.Down))
                    ItemNumber++;
                else if (InputManager.Instance.KeyPressed(Keys.Up))
                    ItemNumber--;

                if (ItemNumber < 0)
                    ItemNumber = Items.Count - 1;
                else if (ItemNumber > Items.Count - 1)
                    ItemNumber = 0;  

                Items.ForEach(i => {
                    i.Image.IsActive = (Items.IndexOf(i) == ItemNumber);
                    i.Image.Update(gameTime);
                });
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Items.ForEach(i => i.Image.Draw(spriteBatch));
        }


    }
}
