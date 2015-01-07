using Microsoft.Xna.Framework;

namespace StarCheckersWindows
{
    public class SpriteSheetEffect : ImageEffect
    {
        public int FrameCounter { get; set; }
        public int SwitchFrame { get; set; }

        public int FrameWidth
        {
            get { return (Image.Texture != null) ? Image.Texture.Width/ (int) AmountOfFrames.X : 0; }
        }
        public int FrameHeight
        {
            get { return (Image.Texture != null) ? Image.Texture.Height / (int)AmountOfFrames.Y : 0; }
        }

        public Vector2 CurrentFrame;
        public Vector2 AmountOfFrames;

        public SpriteSheetEffect()
        {
            AmountOfFrames = new Vector2(3, 4);
            CurrentFrame = new Vector2(1, 0);
            SwitchFrame = 100;
            FrameCounter = 0; 
        }


        public override void LoadContent(ref Image image)
        {
            base.LoadContent(ref image);


        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Image.IsActive)
            {
                FrameCounter += (int) gameTime.ElapsedGameTime.TotalMilliseconds;

                if (FrameCounter >= SwitchFrame)
                {
                    FrameCounter = 0;
                    CurrentFrame.X ++;

                    if (CurrentFrame.X*FrameWidth >= Image.Texture.Width)
                        CurrentFrame.X = 0;
                }
            }
            else 
                CurrentFrame.X = 1;

            Image.SourceRectange = new Rectangle((int) CurrentFrame.X * FrameWidth, (int)CurrentFrame.Y * FrameHeight, FrameWidth, FrameHeight);
        }
    }
}
