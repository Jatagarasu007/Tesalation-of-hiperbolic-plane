using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace PoincareDiskModelConsolApp.Helpers
{
    public class MathHelper
    {
        public MathHelper()
        {

        }

        // Inversion circle is Poencare circle (Fundamental circle defined as x^2 + y^2 = 1)
        public Point CircleInversion(Point point)
        {
            StraightLine p = new StraightLine(new Point(0, 0), point);

            StraightLine v = new StraightLine(point, -1 / p.k);

            List<Point> intersections = FindIntersectionBetweenStraightLineAndCircle(new Circle(new Point(0, 0), 1), v);

            StraightLine q = new StraightLine(intersections[0], new Point(0, 0));
            StraightLine s = new StraightLine(intersections[0], -1/q.k);

            return FindStraightLineIntersection(p, s);
        }

        public Point FindMidPoint(Point a, Point b)
        {
            Point midPoint = new Point();

            midPoint.x = ConvertToZero((b.x + a.x) / 2);
            midPoint.y = ConvertToZero((b.y + a.y) / 2);

            return midPoint;
        }

        public Point FindStraightLineIntersection(StraightLine p, StraightLine q)
        {
            Point intersectionPoint = new Point();

            if (p.k == q.k || ( double.IsInfinity(q.k)  && double.IsInfinity(p.k))) 
            {
                throw new Exception("Method: FindStraightLineIntersection . Message: Straight lines are paralell");
            }

            if (double.IsInfinity(p.k)) 
            {
                intersectionPoint.x = ConvertToZero(p.x);
                intersectionPoint.y = ConvertToZero(q.k * p.x + q.n);
                return intersectionPoint;
            }

            if (double.IsInfinity(q.k))
            {
                intersectionPoint.x = ConvertToZero(q.x);
                intersectionPoint.y = ConvertToZero(p.k * q.x + p.n);
                return intersectionPoint;
            }

            intersectionPoint.x = ConvertToZero((q.n - p.n) / (p.k - q.k));
            intersectionPoint.y = ConvertToZero(q.k * intersectionPoint.x + q.n);
            

            return intersectionPoint;
        }

        // O - center of circle
        public List<float> GenerateCircle(Point o, double radius, List<float> color, double lowerAngleBoudary = 0, double upperAngleBoudary = 360)
        {
            int elementCounter = 0;

            double angle = lowerAngleBoudary;

            // angle in radians
            double angleRadians = 0;

            List<float> hiperbolicStraightLine = new List<float>();

            while (angle <= upperAngleBoudary)
            {
                angleRadians = ConvertToRadians(angle);

                // x - cordinate
                hiperbolicStraightLine.Add((float)(radius * Math.Cos(angleRadians) + o.x));
                // y - cordinate
                hiperbolicStraightLine.Add((float)(radius * Math.Sin(angleRadians) + o.y));
                // z - cordinate
                hiperbolicStraightLine.Add(0);

                // Add red color for each point
                //OpenGL
                hiperbolicStraightLine.AddRange(color);
                angle = angle + 0.5f;
                elementCounter = elementCounter + 7;

            }

            return hiperbolicStraightLine;

        }

        // Remained to included also when straight line contians orgin too
        public List<float> GenerateHiperbolicStraightLine(Point pointA, Point pointB, List<float> color)
        {
            // Center of inversion circle
            Point o = new Point();
            // Radius of inversion circle
            double radius;


            if (IsColinear(pointA, pointB, new Point(0, 0)) == true)
            {
                // This is when you want to generate hiperbolic straight line through center
                List<Point> intersection = FindIntersectionBetweenStraightLineAndCircle(new Circle(new Point(0, 0), 1), new StraightLine(pointA, pointB));

                if (intersection[0].x == intersection[1].x)
                {
                    double[] domain = { Math.Min(intersection[0].y, intersection[1].y), Math.Max(intersection[0].y, intersection[1].y) };
                    return GenerateStraightLine(new StraightLine(pointA, pointB), domain, color, 0.001);
                }
                else
                {
                    double[] domain = { Math.Min(intersection[0].x, intersection[1].x), Math.Max(intersection[0].x, intersection[1].x) };
                    return GenerateStraightLine(new StraightLine(pointA, pointB), domain, color, 0.001);
                }

            }
            else
            {
                Point pointA_inversе = CircleInversion(pointA);
                Point midPointM = FindMidPoint(pointA, pointB);
                Point midPointN = FindMidPoint(pointA_inversе, pointA);

                StraightLine s = new StraightLine(pointA, pointB);
                StraightLine d = new StraightLine(midPointM, -1 / s.k);

                StraightLine p = new StraightLine(pointA, pointA_inversе);
                StraightLine q = new StraightLine(midPointN, -1 / p.k);

                o = FindStraightLineIntersection(d, q);
                radius = FindDistance(pointA, o);

                List<Point> points = FindIntersectionBetweenTwoCircles(new Circle(new Point(0, 0), 1), new Circle(o, radius));

                double minAngle = Math.Min(FindAngle(points[0], o, radius), FindAngle(points[1], o, radius));
                double maxAngle = Math.Max(FindAngle(points[0], o, radius), FindAngle(points[1], o, radius));

                // angle between 2 points must be 180 deegres or less because arch lenght must be lesss then half of circumanse of inversion circle
                if (maxAngle * minAngle < 0 && maxAngle - minAngle >= 180)
                {
                    var auxiliarAngle = maxAngle;
                    maxAngle = 360 + minAngle;
                    minAngle = auxiliarAngle;

                    return GenerateCircle(o, radius, color, minAngle, maxAngle);
                }
                

                return GenerateCircle(o, radius, color, minAngle, maxAngle);
            }

            
        }

        public List<float> GenerateHiperbolicSegment(Point pointA, Point pointB, List<float> color)
        {
            // Center of inversion circle
            Point o = new Point();
            // Radius of inversion circle
            double radius;

            if (IsColinear(pointA, pointB, new Point(0, 0)) == true)
            {
                // This is when you want to generate hiperbolic straight line through center
                List<Point> intersection = FindIntersectionBetweenStraightLineAndCircle(new Circle(new Point(0, 0), 1), new StraightLine(pointA, pointB));

                // If points belongs to line paralel with y axis
                // (pointA.x - pointB.x) < 0.00001
                if (pointB.x == pointA.x)
                {
                    double[] domain = { Math.Min(pointA.y, pointB.y), Math.Max(pointA.y, pointB.y) };
                    return GenerateStraightLine(new StraightLine(pointA, pointB), domain, color, 0.001);
                }
                else 
                {
                    double[] domain = { Math.Min(pointA.x, pointB.x), Math.Max(pointA.x, pointB.x) };
                    return GenerateStraightLine(new StraightLine(pointA, pointB), domain, color, 0.001);
                }
                
            }
            else 
            {
                Point pointA_inversе = CircleInversion(pointA);
                Point midPointM = FindMidPoint(pointA, pointB);
                Point midPointN = FindMidPoint(pointA_inversе, pointA);

                StraightLine s = new StraightLine(pointA, pointB);
                StraightLine d = new StraightLine(midPointM, -1 / s.k);

                StraightLine p = new StraightLine(pointA, pointA_inversе);
                StraightLine q = new StraightLine(midPointN, -1 / p.k);

                o = FindStraightLineIntersection(d, q);
                radius = FindDistance(pointA, o);

                List<Point> points = FindIntersectionBetweenTwoCircles(new Circle(new Point(0, 0), 1), new Circle(o, radius));

                double minAngle = Math.Min(FindAngle(pointA, o, radius), FindAngle(pointB, o, radius));
                double maxAngle = Math.Max(FindAngle(pointA, o, radius), FindAngle(pointB, o, radius));


                // angle between 2 points must be 180 deegres or less because arch lenght must be less then half of circumstance of inversion circle
                if (maxAngle * minAngle < 0 && maxAngle - minAngle >= 180) 
                {
                    var auxiliarAngle = maxAngle;
                    maxAngle =  360 + minAngle;
                    minAngle = auxiliarAngle;

                    return GenerateCircle(o, radius, color, minAngle, maxAngle);
                }

                return GenerateCircle(o, radius, color, minAngle, maxAngle);
            }
          
        }


        // Added color points in order to be usable in  OpenGL

        // Find distance between two points
        public double FindDistance(Point a, Point b)
        {
            double distance = -1;

            distance = Math.Sqrt(Math.Pow(b.x - a.x, 2) + Math.Pow(b.y - a.y, 2));

            return distance;
        }


        // angle defined in degrees
        public double ConvertToRadians(double angle)
        {
            return (angle * (Math.PI)) / 180;
        }

        // angle defined in radians
        public double ConvertToDegrees(double angle)
        {
            return (angle * 180) / (Math.PI);
        }

        // Find angle of point on circle in degrees
        public double FindAngle(Point circlePoint, Point circleCenter, double radius)
        {
            double normalisedX_cordinate = (circlePoint.x - circleCenter.x) / radius;
            double normalisedY_cordinate = (circlePoint.y - circleCenter.y) / radius;

            return ConvertToDegrees(Math.Atan2(normalisedY_cordinate, normalisedX_cordinate));
        }

        // Supported case when intersection have 2 points
        public List<Point> FindIntersectionBetweenTwoCircles(Circle k_1, Circle k_2)
        {
            Point intersectionPointA = new Point();
            Point intersectionPointB = new Point();
            Point k_1Center = k_1.center;
            Point k_2Center = k_2.center;

            double x1;
            double x2;
            double y1;
            double y2;

            // Distance between 2 circle centers
            double distance = Math.Sqrt(Math.Pow(k_2Center.x - k_1Center.x, 2) + Math.Pow(k_2Center.y - k_1Center.y, 2));
            double teta = Math.Sqrt((-distance + k_1.radius + k_2.radius) * (distance - k_1.radius + k_2.radius) * (distance + k_1.radius - k_2.radius) * (distance + k_1.radius + k_2.radius)) / 4;
            // Just ti improve performance and readability
            double helperVariable1 = (Math.Pow(k_1.radius, 2) - Math.Pow(k_2.radius, 2)) / (2 * Math.Pow(distance, 2));
            double helperVariable2 = (2 * teta) / Math.Pow(distance, 2);

            x1 = (k_1Center.x + k_2Center.x) / 2 + (k_2Center.x - k_1Center.x) * helperVariable1 + (k_1Center.y - k_2Center.y) * helperVariable2;
            x2 = (k_1Center.x + k_2Center.x) / 2 + (k_2Center.x - k_1Center.x) * helperVariable1 - (k_1Center.y - k_2Center.y) * helperVariable2;
            y1 = (k_1Center.y + k_2Center.y) / 2 + (k_2Center.y - k_1Center.y) * helperVariable1 - (k_1Center.x - k_2Center.x) * helperVariable2;
            y2 = (k_1Center.y + k_2Center.y) / 2 + (k_2Center.y - k_1Center.y) * helperVariable1 + (k_1Center.x - k_2Center.x) * helperVariable2;


            intersectionPointA.x = x1;
            intersectionPointA.y = y1;
            intersectionPointB.x = x2;
            intersectionPointB.y = y2;

            List<Point> intersectionPoints = new List<Point>();

            intersectionPoints.Add(intersectionPointA);
            intersectionPoints.Add(intersectionPointB);

            return intersectionPoints;
        }

        public List<Point> FindIntersectionBetweenStraightLineAndCircle(Circle k, StraightLine p, double sensitivity = 0.01)
        {
            // Regarding tesalation it should not happen that D <= 0 

            List<Point> intersectionPoints = new List<Point>();

            // Case when p is parallel with y axis
            if (double.IsInfinity(p.k)) 
            {
                double x1 = ConvertToZero(p.x);
                double y1 = ConvertToZero(Math.Sqrt((1 - Math.Pow(p.x, 2))));
                double x2 = ConvertToZero(p.x);
                double y2 = ConvertToZero(-Math.Sqrt((1 - Math.Pow(p.x, 2)))); 

                intersectionPoints.Add(new Point(x1, y1));
                intersectionPoints.Add(new Point(x2, y2));

                return intersectionPoints;
            }

            Point kCenter = k.center;
            // ax^2 + bx + c = 0
            double a = 1 + Math.Pow(p.k, 2);
            double b = 2 * (p.k * (p.n - kCenter.y) - kCenter.x);
            double c = Math.Pow(kCenter.x, 2) + Math.Pow(kCenter.y, 2) + Math.Pow(p.n, 2) - Math.Pow(k.radius, 2) - (2 * p.n * kCenter.y);
            double D = Math.Sqrt(Math.Pow(b, 2) - 4 * a * c);

            if (D == 0)
            {
                double x = ConvertToZero(-b / (2 * a));
                double y = ConvertToZero(p.k * x + p.n);

                intersectionPoints.Add(new Point(x, y));
            }
            else if (D > 0)
            {
                double x1 = ConvertToZero((-b + D) / (2 * a));
                double y1 = ConvertToZero(p.k * x1 + p.n);
                double x2 = ConvertToZero((-b - D) / (2 * a));
                double y2 = ConvertToZero(p.k * x2 + p.n);

                intersectionPoints.Add(new Point(x1, y1));
                intersectionPoints.Add(new Point(x2, y2));
            }

            // For D < 0  there are 2 solutions that belongs to C (set of complex numbers)

            return intersectionPoints;
        }

        // Sensitivity - define how much can be deviation from 0 in equation
        public bool CheckIfPointBelongsToCircle(Point point, Circle circle, double sensitivity)
        {
            Point circleCenter = circle.center;
            double circleRadius = circle.radius;

            double equation = Math.Pow((point.x - circleCenter.x), 2) + Math.Pow((point.y - circleCenter.y), 2) - Math.Pow(circleRadius, 2);

            return equation < sensitivity ? true : false;
        }

        public bool IsColinear(Point pointA, Point pointB, Point pointC)
        {
            if (pointA.x == pointB.x && pointB.x == pointC.x) 
            {
                return true;
            }

            if (pointA.y == pointB.y && pointB.y == pointC.y)
            {
                return true;
            }

            double leftEquation = (pointB.y - pointA.y) / (pointB.x - pointA.x);
            double rightEquation = (pointC.y - pointA.y) / (pointC.x - pointA.x);

            if (ConvertToZero(leftEquation - rightEquation) == 0)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        // Domain - sets of point that belong to R 
        // distanceD - distance between points on domain
        public List<float> GenerateStraightLine(StraightLine p, double[] domain, List<float> color, double distanceD = 0.001)
        {
            List<float> straightLine = new List<float>();
            double a = domain[0];
            double b = domain[1];
            double y;
            
            while(a <= b) 
            {
                if (double.IsInfinity(p.k))
                {
                    y = p.k * a + p.n;
                    straightLine.Add((float)p.x);
                    straightLine.Add((float)a);
                    straightLine.Add(0);
                    straightLine.AddRange(color);
                    a = a + distanceD;
                }
                else 
                {
                    y = p.k * a + p.n;
                    straightLine.Add((float)a);
                    straightLine.Add((float)y);
                    straightLine.Add(0);
                    straightLine.AddRange(color);
                    a = a + distanceD;
                }
            }

            return straightLine;
        }

        public Point cMultiplication(Point z1, Point z2) 
        {
            Point z = new Point();
            z.x = z1.x * z2.x - z1.y * z2.y;
            z.y = z1.x * z2.y + z1.y * z2.x;

            return z;        
        }

        public Point cAddition(Point z1, Point z2)
        {
            Point z = new Point();
            z.x = z1.x + z2.x;
            z.y = z1.y + z2.y;

            return z;
        }

        public Point cSubstraction(Point z1, Point z2)
        {
            Point z = new Point();
            z.x = z1.x - z2.x;
            z.y = z1.y - z2.y;

            return z;
        }

        public Point cDerivation(Point z1, Point z2)
        {
            Point z = new Point();

            z.x = (z1.x * z2.x + z1.y * z2.y) / Math.Pow(cModuo(z2), 2);
            z.y = (z1.y * z2.x - z1.x * z2.y) / Math.Pow(cModuo(z2), 2);


            return z;
        }


        public double cModuo(Point z)
        {
            return Math.Sqrt(Math.Pow(z.x, 2) + Math.Pow(z.y, 2));
        }

        public Point cConjugate(Point z) 
        {
            Point v = new Point();
            v.x = z.x;
            v.y = -z.y;

            return v;
        }

        // alpha is a point against who you are doing rotation
        public Point hRotation(Point z, Point alpha, double angle) 
        {
            Point rotatedPoint = new Point();

            Point beta = GetComplexNumber(1, angle);
            Point expression1 = cSubstraction(z, alpha);
            Point expression2 = cSubstraction(cMultiplication(z, cConjugate(alpha)), new Point(1, 0));

            rotatedPoint = cMultiplication(beta, cDerivation(expression1, expression2));

            rotatedPoint.x = ConvertToZero(rotatedPoint.x);
            rotatedPoint.y = ConvertToZero(rotatedPoint.y);

            return rotatedPoint;
        }

        // alpha (A) is vector OA (O is orgin) for which translation is done
        public Point hTranslation(Point z, Point alpha)
        {
            Point translatedPoint = new Point();

            Point expression1 = cSubstraction(z, alpha);
            Point expression2 = cSubstraction(cMultiplication(z, cConjugate(alpha)), new Point(1, 0));

            translatedPoint = cDerivation(expression1, expression2);

            translatedPoint.x = ConvertToZero(translatedPoint.x);
            translatedPoint.y = ConvertToZero(translatedPoint.y);


            return translatedPoint;
        }

        public Point GetComplexNumber(double radius, double angle) 
        {
            Point z = new Point();

            z.x = radius * Math.Cos(ConvertToRadians(angle));
            z.y = radius * Math.Sin(ConvertToRadians(angle));

            return z;
        }

        // p - number of sides of polygon
        // q - number of polyigons that meet at single vertex
        // Points added in context of OpenGL (added aditional z = 0 cordinate, added color)
        public void tGetInitialRegularPolygon(ref List<PolygonSide> tesalation, int p, int q, int iteration) 
        {
            double s = Math.Sqrt((1 - Math.Tan(Math.PI/ p) * Math.Tan(Math.PI / q)) /(1 + Math.Tan(Math.PI / p) * Math.Tan(Math.PI / q)));

            //s = 0.5;
            List<Point> points= new List<Point>();
            List<PolygonSide> polygonSides = new List<PolygonSide>();

            Point orgin = new Point(0, 0);
            Point newPoint = new Point();

            int counter = 0;

            while (counter < p ) 
            {
                newPoint = hRotation(new Point(s,0), orgin, (counter + 1) * ( 360/p));
                points.Add(newPoint);
                counter = counter + 1;
            }

            counter = 0;
            // points.Count == p
            while (counter < p) 
            {
                PolygonSide polygonSide = new PolygonSide();

                if (counter == 0) 
                {
                    polygonSide.closestA = points[points.Count - 1];
                    polygonSide.A = points[counter];
                    polygonSide.B = points[counter + 1];
                    polygonSide.closestB = points[counter + 2];
                    polygonSides.Add(polygonSide);
                    counter = counter + 1;
                    continue;
                }

                // p - 1 - beacuse counter starts  from 0
                // Check if it is last side od polygon (point)
                if (counter == p - 1)
                {
                    polygonSide.closestA = points[counter - 1];
                    polygonSide.A = points[counter];
                    polygonSide.B = points[0];
                    polygonSide.closestB = points[1];
                    polygonSides.Add(polygonSide);
                    counter = counter + 1;
                    continue;
                }

                // p - 2 - beacuse counter starts  from 0
                // Check if it is second lastest side od polygon (point)
                if (counter == p - 2)
                {
                    polygonSide.closestA = points[counter - 1];
                    polygonSide.A = points[counter ];
                    polygonSide.B = points[counter + 1];
                    polygonSide.closestB = points[0];
                    polygonSides.Add(polygonSide);
                    counter = counter + 1;
                    continue;
                }

                polygonSide.closestA = points[counter - 1];
                polygonSide.A = points[counter];
                polygonSide.B = points[counter + 1];
                polygonSide.closestB = points[counter + 2];
                polygonSides.Add(polygonSide);
                counter = counter + 1;
            }

            tesalation.AddRange(polygonSides);

            foreach (PolygonSide side in polygonSides)
            {
                GetPolygon(ref tesalation, side, p, q, iteration);
            }
        }

        public List<float> tTransformTesalationSetOpenGL(List<PolygonSide> tesalationPolygonSides, List<float> color) 
        {
            // Contains cordiante of points and colors
            List<float> openGLvertex = new List<float>();

            foreach (PolygonSide polygonSide in  tesalationPolygonSides) 
            {
                var hipSegemnt = GenerateHiperbolicSegment(polygonSide.A, polygonSide.B, color);
                openGLvertex.AddRange(hipSegemnt);
            }

            return openGLvertex;
        }


        public double ConvertToZero(double value) 
        {
            if (Math.Abs(value) < 0.000000001) 
            {
                return 0;
            }

            return value;
        }

        public void GetPolygon(ref List<PolygonSide>  tesalation,  PolygonSide s, int p, int q, int iteration) 
        {

            if (iteration == 0) 
            {
                return;
            }

            Point o = new Point(0, 0);

            double angle = ConvertToDegrees((2 * Math.PI) / q);
            List<Point> polygonVertex = new List<Point>();
            List<PolygonSide> polygonSides = new List<PolygonSide>();

            // By default sensitivity
            List<Point> sortedPoints = DetermineRotationCenter(s, angle);
            Point rotatedPoint = new Point();
            int counter = 0;

            Point rotationCenter = sortedPoints[0];
            Point rotationPoint = sortedPoints[1];

            polygonVertex.Add(rotationPoint);
            polygonVertex.Add(rotationCenter);

            // We need all points but p - 1 sides because we need to generate for PolygonSide
            // Two  polyigon vertex are knows so we need  p - 2 more
            while (counter < p - 2) 
            {
                rotatedPoint = hTranslation( hRotation(rotationPoint , rotationCenter, angle), rotationCenter);

                // Check if point of polyigon belongs to poencare disk
                // New Polygon otherwise would be parcial in poencare disk

                if (FindDistance(rotatedPoint, o) > 1)
                {
                    return;
                }

                polygonVertex.Add(rotatedPoint);
                rotationPoint = rotationCenter;
                rotationCenter = rotatedPoint;
                counter = counter + 1;
            }

            counter = 1;

            // Skip first side because we dont need it for a process
            while (counter < p)
            {
                PolygonSide polygonSide = new PolygonSide();


                // p - 1 - beacuse counter starts  from 0
                // Check if it is last side od polygon (point)
                if (counter == p - 1)
                {
                    polygonSide.closestA = polygonVertex[counter - 1];
                    polygonSide.A = polygonVertex[counter];
                    polygonSide.B = polygonVertex[0];
                    polygonSide.closestB = polygonVertex[1];
                    polygonSides.Add(polygonSide);
                    counter = counter + 1;
                    continue;
                }

                // p - 2 - beacuse counter starts  from 0
                // Check if it is second lastest side od polygon (point)
                if (counter == p - 2)
                {
                    polygonSide.closestA = polygonVertex[counter - 1];
                    polygonSide.A = polygonVertex[counter];
                    polygonSide.B = polygonVertex[counter + 1];
                    polygonSide.closestB = polygonVertex[0];
                    polygonSides.Add(polygonSide);
                    counter = counter + 1;
                    continue;
                }

                polygonSide.closestA = polygonVertex[counter - 1];
                polygonSide.A = polygonVertex[counter];
                polygonSide.B = polygonVertex[counter + 1];
                polygonSide.closestB = polygonVertex[counter + 2];
                polygonSides.Add(polygonSide);
                counter = counter + 1;
            }

            tesalation.AddRange(polygonSides);


            foreach(PolygonSide side in polygonSides)
            {
                GetPolygon(ref tesalation, side, p, q, iteration - 1);
            }
        }

        public List<Point> DetermineRotationCenter(PolygonSide s, double angle, double sensitivity = 0.00001) 
        {
            List<Point> sortedPoints = new List<Point>();
            Point newPoint = hTranslation(hRotation(s.B, s.A, angle), s.A); 

            if (ComparePoints(newPoint, s.closestA, sensitivity))
            {
                // rotation center
                sortedPoints.Add(s.B);
                // rotation point
                sortedPoints.Add(s.A);
                return sortedPoints;
            }
            else 
            {
                // rotation center
                sortedPoints.Add(s.A);
                // rotation point
                sortedPoints.Add(s.B);
                return sortedPoints;
            }
            
        
        }

        // You can use FindDistance function also
        // sensitivity = 0.00001
        public bool ComparePoints(Point pointA, Point pointB, double sensitivity = 0.00001) 
        {
            // Or you distance between points, you need to find which is less prone to errors
            return Math.Abs((pointA.x - pointB.x)) < sensitivity && Math.Abs((pointA.y - pointB.y)) < sensitivity ? true : false;
            //return FindDistance(pointA, pointB) < sensitivity ? true : false;
        }


    }
}
