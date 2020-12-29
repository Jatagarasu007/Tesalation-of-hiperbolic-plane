using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PoincareDiskModelConsolApp.Helpers
{
    public class StraightLine
    {
        // These 3 attributes uniquely define straight line 
        public double x;
        public double y;
        // y - y1 = k*(x - x1)
        // y = kx + n;
        public double k;
        public double n;


        public StraightLine(Point point, double k)
        {
            this.x = point.x;
            this.y = point.y;
            this.k = k;

            if (double.IsInfinity(this.k))
            {
                this.n = -this.x;
            }
            else 
            {
                this.n = this.y - this.k * this.x;
            }
            
        }

        public StraightLine(Point pointA, Point pointB)
        {
            this.x = pointA.x;
            this.y = pointA.y;
            StraightLineParameters(pointA, pointB);
        }

        // Coefficient of direction and height on y axis
        private void StraightLineParameters(Point pointA, Point pointB) 
        {
            // Paralel with y axis
            if (pointA.x == pointB.x) 
            {
                // k is undefined but in code we will interprete as 
                k = double.PositiveInfinity;
                n = -pointA.x;
                return;
            }

            if (pointA.y == pointB.y)
            {
                k = 0;
                n = pointA.y;
                return;
            }

            this.k = (pointB.y - pointA.y) / (pointB.x - pointA.x);

            /*
            // Because of calcluating errors
            if (k < 0.00000000001) 
            {
                k = 0;
            }
            */

            this.n = pointA.y - k * pointA.x;

        }

    }
}
