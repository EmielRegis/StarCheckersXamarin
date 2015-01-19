using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StarCheckersWindows.Source.Figures;

namespace StarCheckersWindows
{
    class TwoPlayersGameplayScreen : GameplayScreen
    {
        public TwoPlayersGameplayScreen()
        {
            
        }
        public TwoPlayersGameplayScreen(string whiteFiguresTemplate, string blackFiguresTemplate) : base(whiteFiguresTemplate, blackFiguresTemplate)
        {
            
        }

        protected override void PreparePlayerFigure(Figure f)
        {
            f.MovingEnded += (x, y) =>
            {
                if (f.YPosition == 0 && f.GetType() == typeof(Man))
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
                    if (f.YPosition == 0 && f.GetType() == typeof(Man))
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
                isPlayerTurn = true;
                isEnemyTurn = false;
            };

            f.AttackingEnded += (fig, x, y) =>
            {
                fig.IsAlive = false;
                playerFigures.Remove(fig);

                if (f.GeneratePossibleAttacks(enemyFigures, playerFigures, true).Any())
                {
                    isRecursiveCaptureInProgress = true;
                    f.IsSelected = true;
                    selectedFigure = f;
                    possibleAttacks = f.GeneratePossibleAttacks(enemyFigures, playerFigures, true, true);
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
                    isRecursiveCaptureInProgress = false;
                    isPlayerTurn = true;
                    isEnemyTurn = false;
                }
            };
        }

        protected override void ChessboardClickAction()
        {
            float dim = (Math.Min(ScreenManager.Instance.Dimensions.X, ScreenManager.Instance.Dimensions.Y)) / 8.0f;

            IList<Figure> activeFigures = (isPlayerTurn) ? playerFigures : enemyFigures;
            IList<Figure> nonActiveFigures = (isPlayerTurn) ? enemyFigures : playerFigures;

            if (selectedFigure != null)
            {
                if (possibleAttacks.Any(pt =>
                    pt.Item1.X * dim < InputManager.Instance.MouseOrTouchX && pt.Item1.Y * dim < InputManager.Instance.MouseOrTouchY &&
                    pt.Item1.X * dim + dim > InputManager.Instance.MouseOrTouchX && pt.Item1.Y * dim + dim > InputManager.Instance.MouseOrTouchY))
                {
                    Tuple<Point, Figure> dest = possibleAttacks.First(p =>
                        p.Item1.X * dim < InputManager.Instance.MouseOrTouchX && p.Item1.Y * dim < InputManager.Instance.MouseOrTouchY &&
                        p.Item1.X * dim + dim > InputManager.Instance.MouseOrTouchX && p.Item1.Y * dim + dim > InputManager.Instance.MouseOrTouchY);

                    selectedFigure.Attack(dest.Item2, dest.Item1.X, dest.Item1.Y);
                    selectedFigure.IsSelected = false;
                    selectedFigure = null;
                }
                else if (possibleMoves.Any(pt =>
                    pt.X * dim < InputManager.Instance.MouseOrTouchX && pt.Y * dim < InputManager.Instance.MouseOrTouchY &&
                    pt.X * dim + dim > InputManager.Instance.MouseOrTouchX && pt.Y * dim + dim > InputManager.Instance.MouseOrTouchY) &&
                            !activeFigures.Any(af => af.GeneratePossibleAttacks(activeFigures, nonActiveFigures, (enemyFigures == activeFigures)).Any()))
                {
                    Point dest = possibleMoves.First(p =>
                        p.X * dim < InputManager.Instance.MouseOrTouchX && p.Y * dim < InputManager.Instance.MouseOrTouchY &&
                        p.X * dim + dim > InputManager.Instance.MouseOrTouchX && p.Y * dim + dim > InputManager.Instance.MouseOrTouchY);

                    selectedFigure.Move(dest.X, dest.Y);
                    selectedFigure.IsSelected = false;
                    selectedFigure = null;
                }
            }

            if (!isRecursiveCaptureInProgress)
            {
                if (isPlayerTurn)
                {
                    selectedFigure = playerFigures.FirstOrDefault(f => f.IsSelected);
                    if (selectedFigure != null)
                    {
                        possibleMoves = selectedFigure.GeneratePossibleMoves(playerFigures, enemyFigures);
                        possibleAttacks = selectedFigure.GeneratePossibleAttacks(playerFigures, enemyFigures);
                    }
                }

                if (isEnemyTurn)
                {
                    selectedFigure = enemyFigures.FirstOrDefault(f => f.IsSelected);
                    if (selectedFigure != null)
                    {
                        possibleMoves = selectedFigure.GeneratePossibleMoves(enemyFigures, playerFigures, true);
                        possibleAttacks = selectedFigure.GeneratePossibleAttacks(enemyFigures, playerFigures, true);
                    }
                }
            }
        }
    }
}
