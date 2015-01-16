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
                            pt.Item1.X * dim < Mouse.GetState().X && pt.Item1.Y * dim < Mouse.GetState().Y &&
                            pt.Item1.X * dim + dim > Mouse.GetState().X && pt.Item1.Y * dim + dim > Mouse.GetState().Y))
                {
                    Tuple<Point, Figure> dest = possibleAttacks.First(p =>
                                p.Item1.X * dim < Mouse.GetState().X && p.Item1.Y * dim < Mouse.GetState().Y &&
                                p.Item1.X * dim + dim > Mouse.GetState().X && p.Item1.Y * dim + dim > Mouse.GetState().Y);

                    selectedFigure.Attack(dest.Item2, dest.Item1.X, dest.Item1.Y);
                    selectedFigure.IsSelected = false;
                    selectedFigure = null;
                }
                else if (possibleMoves.Any(pt =>
                            pt.X * dim < Mouse.GetState().X && pt.Y * dim < Mouse.GetState().Y &&
                            pt.X * dim + dim > Mouse.GetState().X && pt.Y * dim + dim > Mouse.GetState().Y) &&
                            !activeFigures.Any(af => af.GeneratePossibleAttacks(activeFigures, nonActiveFigures, (enemyFigures == activeFigures)).Any()))
                {
                    Point dest = possibleMoves.First(p =>
                                p.X * dim < Mouse.GetState().X && p.Y * dim < Mouse.GetState().Y &&
                                p.X * dim + dim > Mouse.GetState().X && p.Y * dim + dim > Mouse.GetState().Y);

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
