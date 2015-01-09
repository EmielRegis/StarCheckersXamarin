using Microsoft.Xna.Framework;

namespace StarCheckersWindows
{
    public class FadeEffect : ImageEffect
    {
        public float FadeSpeed { get; set; }
        public bool Increase { get; set; }

        public FadeEffect()
        {
            Increase = false;
            FadeSpeed = 1;
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
                if (!Increase)
                    Image.Alpha -= FadeSpeed*(float) gameTime.ElapsedGameTime.TotalSeconds;
                else
                    Image.Alpha += FadeSpeed*(float) gameTime.ElapsedGameTime.TotalSeconds;

                if (Image.Alpha < 0.0f)
                {
                    Increase = true;
                    Image.Alpha = 0.0f;
                }
                else if (Image.Alpha > 1.0f)
                {
                    Increase = false;
                    Image.Alpha = 1.0f;
                }
            }
            else
                Image.Alpha = 1.0f;
        }
    }
}
