using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StarCheckersWindows
{
    public class Image
    {
        public string Path { get; set; }
        public float Alpha { get; set; }
        public string Text { get; set; }
        public string FontName { get; set; }
		public bool IsCorrectionWhileScalingNeeded { get; set;}

        public bool IsActive { get; set; }

        [XmlIgnore]
        public Texture2D Texture { get; set; }
        [XmlIgnore]
        public Vector2 Position;
//        [XmlIgnore]
        public Vector2 Scale;
        [XmlIgnore]
        public Rectangle SourceRectange { get; set; }

        [XmlIgnore]
        public Dictionary<string, ImageEffect> EffectList { get; set; }
        public string Effects { get; set; }

        public FadeEffect FadeEffect;
        public SpriteSheetEffect SpriteSheetEffect;

        protected Vector2 origin;
        protected ContentManager content;
        protected RenderTarget2D RenderTarget;
        protected SpriteFont font;

        public Image()
        {
            Path = Effects = Text = String.Empty;
			IsCorrectionWhileScalingNeeded = false;
            FontName = "Fonts/Orbitron";
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Alpha = 1.0f;
            SourceRectange = Rectangle.Empty;
            EffectList = new Dictionary<string, ImageEffect>();
        }

        protected virtual void SetEffect<T>(ref T effect)
        {
            if (effect == null)
                effect = (T)Activator.CreateInstance(typeof(T));
            else
            {
                (effect as ImageEffect).IsActive = true;
                var obj = this;
                (effect as ImageEffect).LoadContent(ref obj);
            }

            EffectList.Add(effect.GetType().ToString().Replace("StarCheckersWindows.", ""), (effect as ImageEffect));

        }

        public virtual void ActivateEffect(string effect)
        {
            if (EffectList.ContainsKey(effect))
            {
                EffectList[effect].IsActive = true;
                var obj = this;
                EffectList[effect].LoadContent(ref obj);
            }
        }

        public void StoreEffects()
        {
            Effects = (EffectList.Any(e => e.Value.IsActive))
                ? EffectList.Where(e => e.Value.IsActive).Select(e => e.Key).Aggregate((e1, e2) => e1 + ":" + e2)
                : string.Empty;
        }

        public void RestoreEffects()
        {
            EffectList.Keys.ToList().ForEach(DeactivateEffect);

            Effects.Split(':').ToList().ForEach(ActivateEffect);
        }

        public virtual void DeactivateEffect(string effect)
        {
            if (EffectList.ContainsKey(effect))
            {
                EffectList[effect].IsActive = false;
                EffectList[effect].UnloadContent();
            }
        }

        public void LoadContent()
        {
            content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");

            if (Path != String.Empty)
                Texture = content.Load<Texture2D>(Path);

            font = content.Load<SpriteFont>(FontName);

            Vector2 dimensions = Vector2.Zero;

            if (Texture != null)
                dimensions.X += Texture.Width;
            dimensions.X += font.MeasureString(Text).X;

            if (Texture != null)
                dimensions.Y = Math.Max(Texture.Height, font.MeasureString(Text).Y);
            else
                dimensions.Y = font.MeasureString(Text).Y;

            if (SourceRectange == Rectangle.Empty)
                SourceRectange = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            RenderTarget = new RenderTarget2D(ScreenManager.Instance.GraphicsDevice, (int)dimensions.X, (int)dimensions.Y);
            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(RenderTarget);
            ScreenManager.Instance.GraphicsDevice.Clear(Color.Transparent);

            ScreenManager.Instance.SpriteBatch.Begin();
            if (Texture != null)
                ScreenManager.Instance.SpriteBatch.Draw(Texture, Vector2.Zero, Color.White);
            ScreenManager.Instance.SpriteBatch.DrawString(font, Text, Vector2.Zero, Color.White);
            ScreenManager.Instance.SpriteBatch.End();

            Texture = RenderTarget;
            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(null);

            SetEffect<FadeEffect>(ref FadeEffect);
            SetEffect<SpriteSheetEffect>(ref SpriteSheetEffect);

            if (Effects != String.Empty)
            {
                string[] split = Effects.Split(':');
                split.ToList().ForEach(ActivateEffect);
            }

        }

        public void UnloadContent()
        {
            content.Unload();
            EffectList.ToList().ForEach(e => DeactivateEffect(e.Key));
        }

        public void Update(GameTime gameTime)
        {
            EffectList.Values.Where(e => e.IsActive).ToList().ForEach(e => e.Update(gameTime));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            origin = new Vector2(SourceRectange.Width / 2.0f, SourceRectange.Height / 2.0f);
            spriteBatch.Begin();
			spriteBatch.Draw(Texture, Position + origin + ((IsCorrectionWhileScalingNeeded) ? (origin * (Scale - Vector2.One)) : Vector2.Zero), SourceRectange,
                Color.White * Alpha, 0.0f, origin, Scale, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
    }
}
