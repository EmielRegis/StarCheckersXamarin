using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StarCheckersWindows.Source.Figures;

namespace StarCheckersWindows
{
    public class King : Figure
    {
        public King()
        {

        }

        public King(int xPosition, int yPosition)
            : base(xPosition, yPosition)
        {

        }

        public King(int xPosition, int yPosition, FigureColor figureColor)
            : base(xPosition, yPosition, figureColor)
        {

        }

        public override IEnumerable<Point> GeneratePossibleMoves(IList<Figure> playerFigures, IList<Figure> enemyFigures, bool isReversed = false)
        {
            IList<Point> moves = new List<Point>();

            int x = 1, y = 1;
            while (XPosition - x >= 0 && YPosition - y >= 0)
            {
                if (playerFigures.All(f => (f.XPosition != XPosition - x) || (f.YPosition != YPosition - y)) &&
                    enemyFigures.All(f => (f.XPosition != XPosition - x) || (f.YPosition != YPosition - y)))
                    moves.Add(new Point(XPosition - x, YPosition - y));
                else break;

                x++;
                y++;
            }

            x = y = 1;
            while (XPosition + x <= 7 && YPosition - y >= 0)
            {
                if (playerFigures.All(f => (f.XPosition != XPosition + x) || (f.YPosition != YPosition - y)) &&
                    enemyFigures.All(f => (f.XPosition != XPosition + x) || (f.YPosition != YPosition - y)))
                    moves.Add(new Point(XPosition + x, YPosition - y));
                else break;

                x++;
                y++;
            }

            x = y = 1;
            while (XPosition - x >= 0 && YPosition + y <= 7)
            {
                if (playerFigures.All(f => (f.XPosition != XPosition - x) || (f.YPosition != YPosition + y)) &&
                    enemyFigures.All(f => (f.XPosition != XPosition - x) || (f.YPosition != YPosition + y)))
                    moves.Add(new Point(XPosition - x, YPosition + y));
                else break;

                x++;
                y++;
            }

            x = y = 1;
            while (XPosition + x <= 7 && YPosition + y <= 7)
            {
                if (playerFigures.All(f => (f.XPosition != XPosition + x) || (f.YPosition != YPosition + y)) &&
                    enemyFigures.All(f => (f.XPosition != XPosition + x) || (f.YPosition != YPosition + y)))
                    moves.Add(new Point(XPosition + x, YPosition + y));
                else break;

                x++;
                y++;
            }

            return moves;
        }

        public override IEnumerable<Tuple<Point, Figure>> GeneratePossibleAttacks(IList<Figure> playerFigures, IList<Figure> enemyFigures, bool isReversed = false, bool isRecursive = false)
        {
            IList<Tuple<Point, Figure>> moves = new List<Tuple<Point, Figure>>();
            if (isRecursive)
            {
                if (XPosition - 2 >= 0 && YPosition - 2 >= 0 &&
               playerFigures.All(f => (f.XPosition != XPosition - 2) || (f.YPosition != YPosition - 2)) &&
               enemyFigures.All(f => (f.XPosition != XPosition - 2) || (f.YPosition != YPosition - 2)) &&
               enemyFigures.Any(f => (f.XPosition == XPosition - 1) && (f.YPosition == YPosition - 1)))
                    moves.Add(new Tuple<Point, Figure>(new Point(XPosition - 2, YPosition - 2),
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
            }
            else
            {
                int x = 2, y = 2;
                while (XPosition - x >= 0 && YPosition - y >= 0)
                {
                    if (playerFigures.All(f => (f.XPosition != XPosition - x) || (f.YPosition != YPosition - y)) &&
                                enemyFigures.All(f => (f.XPosition != XPosition - x) || (f.YPosition != YPosition - y)) &&
                                enemyFigures.Any(f => (f.XPosition == XPosition - x + 1) && (f.YPosition == YPosition - y + 1)))
                        moves.Add(new Tuple<Point, Figure>(new Point(XPosition - x, YPosition - y),
                        enemyFigures.First(f => (f.XPosition == XPosition - x + 1) && (f.YPosition == YPosition - y + 1))));

                    if ((enemyFigures.Any(f => f.XPosition == XPosition - x + 1 && f.YPosition == YPosition - y + 1) &&
                        enemyFigures.Any(f => f.XPosition == XPosition - x && f.YPosition == YPosition - y)) ||
                        (playerFigures.Any(f => f.XPosition == XPosition - x + 1 && f.YPosition == YPosition - y + 1) ||
                        playerFigures.Any(f => f.XPosition == XPosition - x && f.YPosition == YPosition - y)))
                        break;

                    x++;
                    y++;
                }

                x = y = 2;
                while (XPosition - x >= 0 && YPosition + y <= 7)
                {
                    if (playerFigures.All(f => (f.XPosition != XPosition - x) || (f.YPosition != YPosition + y)) &&
                    enemyFigures.All(f => (f.XPosition != XPosition - x) || (f.YPosition != YPosition + y)) &&
                    enemyFigures.Any(f => (f.XPosition == XPosition - x + 1) && (f.YPosition == YPosition + y - 1)))
                        moves.Add(new Tuple<Point, Figure>(new Point(XPosition - x, YPosition + y),
                        enemyFigures.First(f => (f.XPosition == XPosition - x + 1) && (f.YPosition == YPosition + y - 1))));

                    if ((enemyFigures.Any(f => f.XPosition == XPosition - x + 1 && f.YPosition == YPosition + y - 1) &&
                        enemyFigures.Any(f => f.XPosition == XPosition - x && f.YPosition == YPosition + y)) ||
                        (playerFigures.Any(f => f.XPosition == XPosition - x + 1 && f.YPosition == YPosition + y - 1) ||
                        playerFigures.Any(f => f.XPosition == XPosition - x && f.YPosition == YPosition + y)))
                        break;

                    x++;
                    y++;
                }

                x = y = 2;
                while (XPosition + x <= 7 && YPosition - y >= 0)
                {
                    if (playerFigures.All(f => (f.XPosition != XPosition + x) || (f.YPosition != YPosition - y)) &&
                    enemyFigures.All(f => (f.XPosition != XPosition + x) || (f.YPosition != YPosition - y)) &&
                    enemyFigures.Any(f => (f.XPosition == XPosition + x - 1) && (f.YPosition == YPosition - y + 1)))
                        moves.Add(new Tuple<Point, Figure>(new Point(XPosition + x, YPosition - y),
                        enemyFigures.First(f => (f.XPosition == XPosition + x - 1) && (f.YPosition == YPosition - y + 1))));

                    if ((enemyFigures.Any(f => f.XPosition == XPosition + x - 1 && f.YPosition == YPosition - y + 1) &&
                        enemyFigures.Any(f => f.XPosition == XPosition + x && f.YPosition == YPosition - y)) ||
                        (playerFigures.Any(f => f.XPosition == XPosition + x - 1 && f.YPosition == YPosition - y + 1) ||
                        playerFigures.Any(f => f.XPosition == XPosition + x && f.YPosition == YPosition - y)))
                        break;

                    x++;
                    y++;
                }

                x = y = 2;
                while (XPosition + x <= 7 && YPosition + y <= 7)
                {
                    if (playerFigures.All(f => (f.XPosition != XPosition + x) || (f.YPosition != YPosition + y)) &&
                    enemyFigures.All(f => (f.XPosition != XPosition + x) || (f.YPosition != YPosition + y)) &&
                    enemyFigures.Any(f => (f.XPosition == XPosition + x - 1) && (f.YPosition == YPosition + y - 1)))
                        moves.Add(new Tuple<Point, Figure>(new Point(XPosition + x, YPosition + y),
                        enemyFigures.First(f => (f.XPosition == XPosition + x - 1) && (f.YPosition == YPosition + y - 1))));

                    if ((enemyFigures.Any(f => f.XPosition == XPosition + x - 1 && f.YPosition == YPosition + y - 1) &&
                        enemyFigures.Any(f => f.XPosition == XPosition + x && f.YPosition == YPosition + y)) ||
                        (playerFigures.Any(f => f.XPosition == XPosition + x - 1 && f.YPosition == YPosition + y - 1) ||
                        playerFigures.Any(f => f.XPosition == XPosition + x && f.YPosition == YPosition + y)))
                        break;

                    x++;
                    y++;
                }       
            }
                

            return moves;
        }

        public override Figure Copy()
        {
            return new King(XPosition, YPosition, FigureColor);
        }

        public override int Value()
        {
            return 10;
        }
    }
}
