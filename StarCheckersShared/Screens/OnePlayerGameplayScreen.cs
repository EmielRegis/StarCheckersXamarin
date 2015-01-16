using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StarCheckersWindows.Source.Figures;

namespace StarCheckersWindows
{
    public class OnePlayerGameplayScreen : GameplayScreen
    {
        private IList<Figure> remFigs = new List<Figure>();
        private IList<Point> destPts = new List<Point>();
 
        public OnePlayerGameplayScreen()
        {

        }

        public OnePlayerGameplayScreen(string whiteFiguresTemplate, string blackFiguresTemplate)
            : base(whiteFiguresTemplate, blackFiguresTemplate)
        {

        }

        protected override void PreparePlayerFigure(Figure f)
        {
            f.MovingEnded += (x, y) =>
            {
                if (f.YPosition == 0 && f.GetType() == typeof (Man))
                {
                    playerFigures.Remove(f);
                    f = new King(f.XPosition, f.YPosition, f.FigureColor);
                    f.Image = (f.FigureColor == FigureColor.White) ? whiteKingImage : blackKingImage;
                    PreparePlayerFigure(f);
                    playerFigures.Add(f);
                }

                isPlayerTurn = false;
                isEnemyTurn = true;
            };

            f.AttackingEnded += (fig, x, y) =>
            {
                fig.IsAlive = false;
                enemyFigures.Remove(fig);

                if (f.GeneratePossibleAttacks(playerFigures, enemyFigures).Any())
                {
                    isRecursiveCaptureInProgress = true;
                    f.IsSelected = true;
                    selectedFigure = f;
                    possibleAttacks = f.GeneratePossibleAttacks(playerFigures, enemyFigures, isRecursive: true);
                }
                else
                {
                    if (f.YPosition == 0 && f.GetType() == typeof (Man))
                    {
                        playerFigures.Remove(f);
                        f = new King(f.XPosition, f.YPosition, f.FigureColor);
                        f.Image = (f.FigureColor == FigureColor.White) ? whiteKingImage : blackKingImage;
                        PreparePlayerFigure(f);
                        playerFigures.Add(f);
                    }
                    isRecursiveCaptureInProgress = false;
                    isPlayerTurn = false;
                    isEnemyTurn = true;
                }
            };
        }

        protected override void PrepareEnemyFigure(Figure f)
        {
            f.MovingEnded += (x, y) =>
            {
                if (f.YPosition == 7 && f.GetType() == typeof(Man))
                {
                    enemyFigures.Remove(f);
                    f = new King(f.XPosition, f.YPosition, f.FigureColor);
                    f.Image = (f.FigureColor == FigureColor.White) ? whiteKingImage : blackKingImage;
                    PrepareEnemyFigure(f);
                    enemyFigures.Add(f);
                }
                f.IsSelected = false;
                isPlayerTurn = true;
                isEnemyTurn = false;
            };

            f.AttackingEnded += (fig, x, y) =>
            {
                fig.IsAlive = false;
                playerFigures.RemoveAll(rf => rf.XPosition == fig.XPosition && rf.YPosition == fig.YPosition);
                remFigs.RemoveAt(0);
                destPts.RemoveAt(0);

                if (remFigs.Any())
                {
                    isRecursiveCaptureInProgress = true;
                    f.IsSelected = true;
                    selectedFigure = f;
                    selectedFigure.Attack(remFigs.First(), destPts.First().X, destPts.First().Y);
                    selectedFigure.IsSelected = false;
                    selectedFigure = null;
                    isEnemyTurn = false;
                }
                else
                {
                    if (f.YPosition == 7 && f.GetType() == typeof(Man))
                    {
                        enemyFigures.Remove(f);
                        f = new King(f.XPosition, f.YPosition, f.FigureColor);
                        f.Image = (f.FigureColor == FigureColor.White) ? whiteKingImage : blackKingImage;
                        PrepareEnemyFigure(f);
                        enemyFigures.Add(f);
                    }
                    f.IsSelected = false;
                    isRecursiveCaptureInProgress = false;
                    isPlayerTurn = true;
                    isEnemyTurn = false;
                }
            };
        }

        protected override void InsideUpdateAction()
        {
            if (isEnemyTurn)
            {
                Figure f;
                var ai = new BasicAI();
                int result = ai.AlphaBetaMinMaxAlgorithm(0, 5, int.MinValue, int.MaxValue, true, enemyFigures, playerFigures, ai, out f,
                    out remFigs, out destPts);

                selectedFigure = f;

                if (remFigs != null && remFigs.Any())
                {
                    selectedFigure.Attack(remFigs.First(), destPts.First().X, destPts.First().Y);
                    selectedFigure.IsSelected = false;
                    selectedFigure = null;
                    isEnemyTurn = false;
                }
                else
                {
                    selectedFigure.Move(destPts.First().X, destPts.First().Y);
                    selectedFigure.IsSelected = false;
                    selectedFigure = null;
                    isEnemyTurn = false;
                }
            }
        }

        protected override void ChessboardClickAction()
        {
            

            if (isPlayerTurn)
            {
                float dim = (Math.Min(ScreenManager.Instance.Dimensions.X, ScreenManager.Instance.Dimensions.Y))/8.0f;

                IList<Figure> activeFigures = playerFigures;
                IList<Figure> nonActiveFigures = enemyFigures;

                if (selectedFigure != null)
                {
					if (possibleAttacks.Any(pt =>
						pt.Item1.X*dim < InputManager.Instance.MouseOrTouchX && pt.Item1.Y*dim < InputManager.Instance.MouseOrTouchY &&
						pt.Item1.X*dim + dim > InputManager.Instance.MouseOrTouchX && pt.Item1.Y*dim + dim > InputManager.Instance.MouseOrTouchY))
					{
						Tuple<Point, Figure> dest = possibleAttacks.First(p =>
							p.Item1.X*dim < InputManager.Instance.MouseOrTouchX && p.Item1.Y*dim < InputManager.Instance.MouseOrTouchY &&
							p.Item1.X*dim + dim > InputManager.Instance.MouseOrTouchX && p.Item1.Y*dim + dim > InputManager.Instance.MouseOrTouchY);

						selectedFigure.Attack(dest.Item2, dest.Item1.X, dest.Item1.Y);
						selectedFigure.IsSelected = false;
						selectedFigure = null;
					}
					else if (possibleMoves.Any(pt =>
						pt.X*dim < InputManager.Instance.MouseOrTouchX && pt.Y*dim < InputManager.Instance.MouseOrTouchY &&
						pt.X*dim + dim > InputManager.Instance.MouseOrTouchX && pt.Y*dim + dim > InputManager.Instance.MouseOrTouchY) &&
						!activeFigures.Any(
							af =>
							af.GeneratePossibleAttacks(activeFigures, nonActiveFigures,
								(enemyFigures == activeFigures)).Any()))
					{
						Point dest = possibleMoves.First(p =>
							p.X*dim < InputManager.Instance.MouseOrTouchX && p.Y*dim < InputManager.Instance.MouseOrTouchY &&
							p.X*dim + dim > InputManager.Instance.MouseOrTouchX && p.Y*dim + dim > InputManager.Instance.MouseOrTouchY);

						selectedFigure.Move(dest.X, dest.Y);
						selectedFigure.IsSelected = false;
						selectedFigure = null;
					}
                }

                if (!isRecursiveCaptureInProgress)
                {
                    selectedFigure = playerFigures.FirstOrDefault(f => f.IsSelected);
                    if (selectedFigure != null)
                    {
                        possibleMoves = selectedFigure.GeneratePossibleMoves(playerFigures, enemyFigures);
                        possibleAttacks = selectedFigure.GeneratePossibleAttacks(playerFigures, enemyFigures);
                    }    
                }
            }
        }
    }
}
