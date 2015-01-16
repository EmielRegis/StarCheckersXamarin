using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StarCheckersWindows.Source.Figures;

namespace StarCheckersWindows
{
    public abstract class GameplayScreen : GameScreen
    {
//        GameplayManager
        public event Action<Figure> OnPreparePlayerFigure;
        public event Action<Figure> OnPrepareEnemyFigure;
        public event Action OnChessboardClick;
        public event Action<FigureColor> OnEndgame;

        public bool IsRecursiveCaptureAllowed = true;
        public bool IsCapturingNecessary = true;

        public Func<bool> EndgameCondition; 
//        end of GameplayManager

        protected Character character;
        protected Chessboard chessboard;

        protected List<Figure> whiteFigures;
        protected List<Figure> blackFigures;
        protected List<Figure> playerFigures;
        protected List<Figure> enemyFigures;

        protected Figure selectedFigure;
        protected IEnumerable<Point> possibleMoves;
        protected IEnumerable<Tuple<Point, Figure>> possibleAttacks;

        protected Image whiteKingImage, blackKingImage, whiteManImage, blackManImage;

        protected bool isPlayerTurn = true;
        protected bool isEnemyTurn = false;     
        protected bool isRecursiveCaptureInProgress = false;

        protected abstract void PreparePlayerFigure(Figure f);

        protected abstract void PrepareEnemyFigure(Figure f);

        protected abstract void ChessboardClickAction();

        protected virtual void InsideUpdateAction()
        {
            
        }

        protected GameplayScreen()
        {
            character = new XmlManager<Character>().Load("Load/Gameplay/Character.xml");

            XmlManager<Chessboard> xml = new XmlManager<Chessboard> { Type = typeof(Chessboard) };
            chessboard = xml.Load("Load/Gameplay/Chessboard.xml");

            whiteManImage = new Image { Path = "Themes/MainThemeWhite/Figures/ManImage" };
            blackManImage = new Image { Path = "Themes/MainThemeBlack/Figures/ManImage" };
            whiteKingImage = new Image { Path = "Themes/MainThemeWhite/Figures/KingImage" };
            blackKingImage = new Image { Path = "Themes/MainThemeBlack/Figures/KingImage" };
        }

        protected GameplayScreen(string whiteFiguresTemplate, string blackFiguresTemplate)
        {
            character = new XmlManager<Character>().Load("Load/Gameplay/Character.xml");

            chessboard = new Chessboard
            {
                BlackFieldImage = {Path = "Themes/" + blackFiguresTemplate + "/Chessboard/BlackFieldImage"},
                WhiteFieldImage = {Path = "Themes/" + whiteFiguresTemplate + "/Chessboard/WhiteFieldImage"}
            };

            whiteManImage = new Image { Path = "Themes/" + whiteFiguresTemplate + "/Figures/ManImage" };
            blackManImage = new Image { Path = "Themes/" + blackFiguresTemplate + "/Figures/ManImage" };
            whiteKingImage = new Image { Path = "Themes/" + whiteFiguresTemplate + "/Figures/KingImage" };
            blackKingImage = new Image { Path = "Themes/" + blackFiguresTemplate + "/Figures/KingImage" };
        }

        public override void LoadContent()
        {                        
            base.LoadContent();
            
            character.LoadContent(); 
        
            chessboard.LoadContent();
           
            whiteManImage.LoadContent();
            blackManImage.LoadContent();
            whiteKingImage.LoadContent();
            blackKingImage.LoadContent();

            enemyFigures = new List<Figure>
            {
                new Man(1, 0), new Man(3, 0), new Man(5, 0), new Man(7, 0), 
                new Man(0, 1), new Man(2, 1), new Man(4, 1), new Man(6, 1),
                new Man(1, 2), new Man(3, 2), new Man(5, 2), new Man(7, 2)
            };

            playerFigures = new List<Figure>
            {
                new Man(0, 5), new Man(2, 5), new Man(4, 5), new Man(6, 5), 
                new Man(1, 6), new Man(3, 6), new Man(5, 6), new Man(7, 6),
                new Man(0, 7), new Man(2, 7), new Man(4, 7), new Man(6, 7)
            };

            whiteFigures = playerFigures;
            blackFigures = enemyFigures;

            whiteFigures.ForEach(f =>
            {
                f.FigureColor = FigureColor.White;
                f.Image = whiteManImage;
            });

            blackFigures.ForEach(f =>
            {
                f.FigureColor = FigureColor.Black;
                f.Image = blackManImage;
            });

            playerFigures.ForEach(f =>
            {
                PreparePlayerFigure(f);
                if(OnPreparePlayerFigure != null) OnPreparePlayerFigure(f);
            });

            enemyFigures.ForEach(f =>
            {
                PrepareEnemyFigure(f);
                if (OnPrepareEnemyFigure != null) OnPrepareEnemyFigure(f);
            });

            isPlayerTurn = true;
            EndgameCondition = () => !(playerFigures.Any() && enemyFigures.Any());
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            chessboard.UnloadContent();
            character.UnloadContent();

            playerFigures.ForEach(f => f.UnloadContent());
            enemyFigures.ForEach(f => f.UnloadContent());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            chessboard.Update(gameTime);
            character.Update(gameTime);

			if (InputManager.Instance.MouseLeftButtonPressed() || InputManager.Instance.IsTouch)
            {
                ChessboardClickAction();
                if (OnChessboardClick != null) OnChessboardClick();
				InputManager.Instance.HandleTouch ();
            }

            InsideUpdateAction();

            playerFigures.ForEach(f => f.Update(gameTime));
            enemyFigures.ForEach(f => f.Update(gameTime));

            if (EndgameCondition())
            {
                if (!playerFigures.Any())
                {
                    
                }
                else if (!enemyFigures.Any())
                {
                    
                }
            }
        }

        public sealed override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            chessboard.Draw(spriteBatch);
//            character.Draw(spriteBatch);

            whiteFigures.ForEach(f => f.Draw(spriteBatch));
            blackFigures.ForEach(f => f.Draw(spriteBatch));

            if (selectedFigure != null)
            {
                float dim = (Math.Min(ScreenManager.Instance.Dimensions.X, ScreenManager.Instance.Dimensions.Y)) / 8.0f;

                PrimitiveSpritePainter.DrawRectangle(spriteBatch, new Rectangle((int)(selectedFigure.DrawingPosition.X * dim), (int)(selectedFigure.DrawingPosition.Y * dim), (int)dim, (int)dim), Color.Yellow, 3);

                if (possibleAttacks.Any())
                    possibleAttacks.ToList().ForEach(p => PrimitiveSpritePainter.DrawRectangle(spriteBatch, new Rectangle((int)(p.Item1.X * dim), (int)(p.Item1.Y * dim), (int)dim, (int)dim), Color.Red, 4));
                else
                    possibleMoves.ToList().ForEach(p => PrimitiveSpritePainter.DrawRectangle(spriteBatch, new Rectangle((int)(p.X * dim), (int)(p.Y * dim), (int)dim, (int)dim), Color.LimeGreen, 4));
            }
        }
    }
}
