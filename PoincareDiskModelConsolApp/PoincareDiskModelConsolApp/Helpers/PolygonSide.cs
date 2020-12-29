using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoincareDiskModelConsolApp.Helpers
{
    public class PolygonSide
    {
        public Point A;
        public Point B;
        public Point closestA;
        public Point closestB;

        public  PolygonSide(Point A, Point B, Point closestA, Point closestB)
        {
            this.A = A;
            this.B = B;
            this.closestA = closestA;
            this.closestB = closestB;
        }

        public PolygonSide()
        {

        }


    }
}
