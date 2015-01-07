using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StarCheckersWindows
{
    public class Chessboard
    {
        public Image BlackFieldImage { get; set; }
        public Image WhiteFieldImage { get; set; }


        public void ReloadChessboard(string blackFieldImagePath, string whiteFieldImagePath)
        {
            BlackFieldImage.Path = blackFieldImagePath;
            WhiteFieldImage.Path = whiteFieldImagePath;

            LoadContent();
        }

        public void LoadContent()
        {
            BlackFieldImage.LoadContent();
            WhiteFieldImage.LoadContent();
			BlackFieldImage.IsCorrectionWhileScalingNeeded = true;
			WhiteFieldImage.IsCorrectionWhileScalingNeeded = true;
        }

        public void UnloadContent()
        {
            BlackFieldImage.UnloadContent();
            BlackFieldImage.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (WhiteFieldImage == null || BlackFieldImage == null)
                return;

            float dim = ((int) Math.Min(ScreenManager.Instance.Dimensions.X, ScreenManager.Instance.Dimensions.Y))/8;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Image field = ((x + y)%2 == 0) ? WhiteFieldImage : BlackFieldImage;
                    field.Position.X = (float) x*dim;
                    field.Position.Y = (float) y*dim;
                    field.Scale.X = dim / field.SourceRectange.Width;
                    field.Scale.Y = dim / field.SourceRectange.Height;
                    
                    field.Draw(spriteBatch);
                }
            }
        }
    }
}
