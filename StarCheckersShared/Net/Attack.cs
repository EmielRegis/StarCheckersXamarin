using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StarCheckersWindows
{
    public class Attack
    {
        public bool IsMultiple { get; set;}
        public Point ActPos { get; set;}
        public List<Point> DestPos { get; set;}
        public List<Point> RemFigsPos { get; set;}

        public Attack()
        {

        }
    }
}

