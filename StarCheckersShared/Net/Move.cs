using System;
using Microsoft.Xna.Framework;


namespace StarCheckersWindows
{
    public class Move
    {
        public Point ActPos { get; set;}
        public Point DestPos { get; set;}

        public Move(Point actualPosition, Point destinationPosition)
        {
            ActPos = actualPosition;
            DestPos = destinationPosition;
        }
    }
}

