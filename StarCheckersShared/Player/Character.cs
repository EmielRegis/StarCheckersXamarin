using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarCheckersWindows
{

    public class Character
    {
        public Image Image { get; set; }
        public Vector2 Velocity;
        public float MoveSpeed { get; set; }

        public Character()
        {
            Velocity = Vector2.Zero;
        }

        public void LoadContent()
        {
            Image.LoadContent();
        }

        public void UnloadContent()
        {
            Image.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            Image.IsActive = true;

            if (Velocity.X == 0.0f)
            {
                if (InputManager.Instance.KeyDown(Keys.Down))
                {
                    Velocity.Y = MoveSpeed * ((float)gameTime.ElapsedGameTime.TotalSeconds);
                    Image.SpriteSheetEffect.CurrentFrame.Y = 0;
                }
                else if (InputManager.Instance.KeyDown(Keys.Up))
                {
                    Velocity.Y = -MoveSpeed * ((float)gameTime.ElapsedGameTime.TotalSeconds);
                    Image.SpriteSheetEffect.CurrentFrame.Y = 3;
                }
                else
                    Velocity.Y = 0.0f;
            }

            if (Velocity.Y == 0.0f)
            {
                if (InputManager.Instance.KeyDown(Keys.Left))
                {
                    Velocity.X = -MoveSpeed * ((float)gameTime.ElapsedGameTime.TotalSeconds);
                    Image.SpriteSheetEffect.CurrentFrame.Y = 1;
                }
                else if (InputManager.Instance.KeyDown(Keys.Right))
                {
                    Velocity.X = MoveSpeed * ((float)gameTime.ElapsedGameTime.TotalSeconds);
                    Image.SpriteSheetEffect.CurrentFrame.Y = 2;
                }
                else
                    Velocity.X = 0.0f;
            }

            if(Velocity.X == 0.0f && Velocity.Y == 0.0f)
                Image.IsActive = false;

            Image.Update(gameTime);
            Image.Position += Velocity;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
    }
}
