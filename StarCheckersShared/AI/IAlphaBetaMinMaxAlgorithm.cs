using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StarCheckersWindows
{
    public interface IAlphaBetaMinMaxAlgorithm
    {
        int AlphaBetaMinMaxAlgorithm(int depth, int maxDepth, int alpha, int beta, bool isReversed, IList<Figure> playerFigures, IList<Figure> enemyFigures, IFigureEvaluator figureEvaluator, out Figure movedFigure, out IList<Figure> removedFigures, out IList<Point> destinationPoints, bool isRecursive = false);
    }
}
