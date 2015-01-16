using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StarCheckersWindows.Source.Figures;

namespace StarCheckersWindows
{
    public class Man : Figure
    {
        public Man()
        {

        }

        public Man(int xPosition, int yPosition)
            : base(xPosition, yPosition)
        {

        }

        public Man(int xPosition, int yPosition, FigureColor figureColor)
            : base(xPosition, yPosition, figureColor)
        {

        }

        public override IEnumerable<Point> GeneratePossibleMoves(IList<Figure> playerFigures, IList<Figure> enemyFigures, bool isReversed = false)
        {
            IList<Point> moves = new List<Point>();

            if (!isReversed)
            {
                if (XPosition - 1 >= 0 && YPosition - 1 >= 0 &&
                playerFigures.All(f => (f.XPosition != XPosition - 1) || (f.YPosition != YPosition - 1)) &&
                enemyFigures.All(f => (f.XPosition != XPosition - 1) || (f.YPosition != YPosition - 1)))
                    moves.Add(new Point(XPosition - 1, YPosition - 1));

                if (XPosition + 1 <= 7 && YPosition - 1 >= 0 &&
                playerFigures.All(f => (f.XPosition != XPosition + 1) || (f.YPosition != YPosition - 1)) &&
                enemyFigures.All(f => (f.XPosition != XPosition + 1) || (f.YPosition != YPosition - 1)))
                    moves.Add(new Point(XPosition + 1, YPosition - 1));
            }
            else
            {
                if (XPosition - 1 >= 0 && YPosition + 1 <= 7 &&
                playerFigures.All(f => (f.XPosition != XPosition - 1) || (f.YPosition != YPosition + 1)) &&
                enemyFigures.All(f => (f.XPosition != XPosition - 1) || (f.YPosition != YPosition + 1)))
                    moves.Add(new Point(XPosition - 1, YPosition + 1));

                if (XPosition + 1 <= 7 && YPosition + 1 <= 7 &&
                playerFigures.All(f => (f.XPosition != XPosition + 1) || (f.YPosition != YPosition + 1)) &&
                enemyFigures.All(f => (f.XPosition != XPosition + 1) || (f.YPosition != YPosition + 1)))
                    moves.Add(new Point(XPosition + 1, YPosition + 1));
            }

            return moves;
        }

        public override IEnumerable<Tuple<Point, Figure>> GeneratePossibleAttacks(IList<Figure> playerFigures, IList<Figure> enemyFigures, bool isReversed = false, bool isRecursive = false)
        {
            IList<Tuple<Point, Figure>> moves = new List<Tuple<Point, Figure>>();

            if (XPosition - 2 >= 0 && YPosition - 2 >= 0 &&
                playerFigures.All(f => (f.XPosition != XPosition - 2) || (f.YPosition != YPosition - 2)) &&
                enemyFigures.All(f => (f.XPosition != XPosition - 2) || (f.YPosition != YPosition - 2)) &&
                enemyFigures.Any(f => (f.XPosition == XPosition - 1) && (f.YPosition == YPosition - 1)))
                    moves.Add(new Tuple<Point, Figure>(new Point( XPosition - 2, YPosition - 2), 
                    enemyFigures.First(f => (f.XPosition == XPosition - 1) && (f.YPosition == YPosition - 1))));

            if (XPosition - 2 >= 0 && YPosition + 2 <= 7 &&
                playerFigures.All(f => (f.XPosition != XPosition - 2) || (f.YPosition != YPosition + 2)) &&
                enemyFigures.All(f => (f.XPosition != XPosition - 2) || (f.YPosition != YPosition + 2)) &&
                enemyFigures.Any(f => (f.XPosition == XPosition - 1) && (f.YPosition == YPosition + 1)))
                    moves.Add(new Tuple<Point, Figure>(new Point(XPosition - 2, YPosition + 2),
                    enemyFigures.First(f => (f.XPosition == XPosition - 1) && (f.YPosition == YPosition + 1))));

            if (XPosition + 2 <= 7 && YPosition - 2 >= 0 &&
                playerFigures.All(f => (f.XPosition != XPosition + 2) || (f.YPosition != YPosition - 2)) &&
                enemyFigures.All(f => (f.XPosition != XPosition + 2) || (f.YPosition != YPosition - 2)) &&
                enemyFigures.Any(f => (f.XPosition == XPosition + 1) && (f.YPosition == YPosition - 1)))
                    moves.Add(new Tuple<Point, Figure>(new Point(XPosition + 2, YPosition - 2),
                    enemyFigures.First(f => (f.XPosition == XPosition + 1) && (f.YPosition == YPosition - 1))));

            if (XPosition + 2 <= 7 && YPosition + 2 <= 7 &&
                playerFigures.All(f => (f.XPosition != XPosition + 2) || (f.YPosition != YPosition + 2)) &&
                enemyFigures.All(f => (f.XPosition != XPosition + 2) || (f.YPosition != YPosition + 2)) &&
                enemyFigures.Any(f => (f.XPosition == XPosition + 1) && (f.YPosition == YPosition + 1)))
                    moves.Add(new Tuple<Point, Figure>(new Point(XPosition + 2, YPosition + 2),
                    enemyFigures.First(f => (f.XPosition == XPosition + 1) && (f.YPosition == YPosition + 1))));

            return moves;
        }

        public override Figure Copy()
        {
            return new Man(XPosition, YPosition, FigureColor);
        }

        public override int Value()
        {
            return 1;
        }
    }
}
