using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoincareDiskModelConsolApp.Helpers
{
    public class Circle
    {

        public Point center;
        public double radius;

        //General format: x^2 + y^2 + Ax + By + C = 0
        public double A;
        public double B;
        public double C;

        public Circle(Point center, double radius)
        {
            this.center = center;
            this.radius = radius;
            SetCoefficient(center, radius);
        }

        // Set coefficient for circle equation when it is represented in general format 
        private void SetCoefficient(Point center, double radius) 
        {
            A = -2 * center.x;
            B = -2 * center.y;
            C = Math.Pow(center.x , 2) + Math.Pow(center.y, 2)- Math.Pow(radius, 2);
        }
    }
}
