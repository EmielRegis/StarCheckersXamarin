using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.Xna.Framework;

namespace StarCheckersWindows
{
    public class BasicAI : IAlphaBetaMinMaxAlgorithm, IFigureEvaluator
    {
        public int AlphaBetaMinMaxAlgorithm(int depth, int maxDepth, int alpha, int beta, bool isReversed, IList<Figure> playerFigures, IList<Figure> enemyFigures, IFigureEvaluator figureEvaluator, out Figure movedFigure, out IList<Figure> removedFigures, out IList<Point> destinationPoints, bool isRecursive = false)
        {        
            removedFigures = new List<Figure>();
            destinationPoints = new List<Point>();
            movedFigure = null;

            if (!playerFigures.Any())
                return -enemyFigures.Select(f => figureEvaluator.EvaluateFigure(f)).Aggregate((a, b) => a + b);
            if (!enemyFigures.Any())
                return playerFigures.Select(f => figureEvaluator.EvaluateFigure(f)).Aggregate((a, b) => a + b);
            if (depth == maxDepth)
                return (playerFigures.Select(f => figureEvaluator.EvaluateFigure(f)).Aggregate((a, b) => a + b)
                     - enemyFigures.Select(f => figureEvaluator.EvaluateFigure(f)).Aggregate((a, b) => a + b));               

            var possibleAttacks = playerFigures
                                 .SelectMany(f => f.GeneratePossibleAttacks(playerFigures, enemyFigures, isReversed, isRecursive).
                                     Select(a => new {figure = f, attack = a})).ToList();

            if (possibleAttacks.Any())
            {
                foreach (var x in possibleAttacks)
                {
                    List<Figure> pFigs = playerFigures.Select(pf => pf.Copy()).ToList();
                    List<Figure> eFigs = enemyFigures.Select(ef => ef.Copy()).ToList();
                    IList<Figure> rFigs = new List<Figure>();
                    IList<Point> dPts = new List<Point>();

                    var selFig = pFigs.First(sf => sf.XPosition == x.figure.XPosition && sf.YPosition == x.figure.YPosition);
                    selFig.XPosition = x.attack.Item1.X;
                    selFig.YPosition = x.attack.Item1.Y;

                    var remFig = x.attack.Item2;
                    eFigs.RemoveAll(rf => rf.XPosition == remFig.XPosition && rf.YPosition == remFig.YPosition);

                    Figure f;
                    IList<Figure> lf;
                    IList<Point> lp;
                    int tempEvaluation;
                    if (selFig.GeneratePossibleAttacks(pFigs, eFigs, isReversed, true).Any())
                    {
                        tempEvaluation = AlphaBetaMinMaxAlgorithm(depth + 1, maxDepth, alpha, beta, isReversed, pFigs,
                            eFigs,
                            figureEvaluator, out f, out lf, out lp);

                        rFigs = lf;
                        dPts = lp;
                    }
                    else
                    {
                        if ((isReversed && selFig.YPosition == 7) ^ (!isReversed && selFig.YPosition == 0) && selFig.GetType() == typeof(Man))
                        {
                            pFigs.Remove(selFig);
                            selFig = new King(selFig.XPosition, selFig.YPosition, selFig.FigureColor);
                            pFigs.Add(selFig);
                        }

                        tempEvaluation = -AlphaBetaMinMaxAlgorithm(depth + 1, maxDepth, -beta, -alpha, !isReversed, eFigs, pFigs,
                                         figureEvaluator, out f, out lf, out lp);
                        
                    }
                        
                    if(tempEvaluation > alpha)
                    {
                        alpha = tempEvaluation;
                       
                        movedFigure = x.figure;
                        removedFigures = rFigs;
                        destinationPoints = dPts;
                        removedFigures.Insert(0 ,remFig);
                        destinationPoints.Insert(0, x.attack.Item1);

                        if (alpha >= beta) break;
                    }
                    
                }
            }
            else
            {
                var possibleMoves = playerFigures
                                 .SelectMany(f => f.GeneratePossibleMoves(playerFigures, enemyFigures, isReversed).
                                     Select(m => new { figure = f, move = m })).ToList();

                if (possibleMoves.Any())
                {
                    foreach (var x in possibleMoves)
                    {
                         var pFigs = playerFigures.Select(pf => pf.Copy()).ToList();
                        var eFigs = enemyFigures.Select(ef => ef.Copy()).ToList();

                       var selFig = pFigs.First(sf => sf.XPosition == x.figure.XPosition && sf.YPosition == x.figure.YPosition);
                        selFig.XPosition = x.move.X;
                        selFig.YPosition = x.move.Y;

                        if ((isReversed && selFig.YPosition == 7) ^ (!isReversed && selFig.YPosition == 0) && selFig.GetType() == typeof(Man))
                        {
                            pFigs.Remove(selFig);
                            selFig = new King(selFig.XPosition, selFig.YPosition, selFig.FigureColor);
                            pFigs.Add(selFig);
                        }

                        Figure f;
                        IList<Figure> lf;
                        IList<Point> lp;
                        int tempEvaluation = -AlphaBetaMinMaxAlgorithm(depth + 1, maxDepth, -beta, -alpha, !isReversed, eFigs, pFigs,
                                             figureEvaluator, out f, out lf, out lp);

                        if(tempEvaluation > alpha)
                        {
                            alpha = tempEvaluation;

                            movedFigure = x.figure;
                            destinationPoints = new List<Point>{x.move};

                            if (alpha >= beta) break;
                        }
                    }
                }
            }
            return alpha;
        }

        public int EvaluateFigure(Figure figure, bool isReversed = false)
        {
            return figure.Value();
        }
    }
}