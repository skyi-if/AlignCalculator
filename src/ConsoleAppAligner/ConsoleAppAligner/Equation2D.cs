using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentFactory.IFMath
{
    public class Equation2D
    {
        public enum Pattern2Dim
        {            
            Ellipse,
            Parabola,
            Hyperbola,
        }

        // predictable equation.
        // cy0 * (y -y1)^0 + cy1 * (y - y1) + cy2 * (y-y1)^2 = cx0 * (x - x1)^0 + cx1 * (x - x1) + cx2 * (x-x1)^2
        List<double> coeffXs = new List<double>() { 0, 0, 0, 0 };
        List<double> coeffYs = new List<double>() { 0, 1 };

        List<double> pointXs = new List<double>();
        List<double> pointYs = new List<double>();

        public List<double> CoeffXs { get => coeffXs; set => coeffXs = value; }
        public List<double> CoeffYs { get => coeffYs; set => coeffYs = value; }
        public List<double> PointXs { get => pointXs; set => pointXs = value; }
        public List<double> PointYs { get => pointYs; set => pointYs = value; }

        public Equation2D()
        {

        }

        public Equation2D(int xdim, int ydim, List<double> coeffXs, List<double> coeffYs, double shiftX, double shiftY)
        {
            throw new NotImplementedException();
        }

        public Equation2D(double slopX, double yoffset)
        {
            coeffXs[0] = yoffset;
            coeffXs[1] = slopX;
        }

        public Equation2D(double pt1x, double pt1y, double pt2x, double pt2y)
        {
            // line with 2 point.
            // (y- y1)/(y2-y1) = (x - x1)/(x2 - x1)
            // y - y1 = (y2 - y1) * (x - x1) / (x2 - x1)
            // y - y1 = (y2 - y1) * (x) / (x2 - x1) - (y2 - y1) * ( x1) / (x2 - x1)

            coeffYs[0] = -1.0 * pt1y;
            coeffXs[0] = (pt1y - pt2y) / (pt2x - pt1x);
            coeffXs[1] = (pt2y - pt1y) / (pt2x - pt1x);


        }

        public Equation2D(Line2D line2DInfo)
        {
            // make line
            if(coeffXs.Count < 2)
            {
                throw new NotImplementedException();
            }

            if (line2DInfo.Vx != 0)
            {
                coeffXs[1] = line2DInfo.Vy / line2DInfo.Vx;
                coeffXs[0] = line2DInfo.Y1 - coeffXs[1] * line2DInfo.X1;
            }
            else
            {
                //coeffYs[1] = 0;
                coeffXs[1] = double.PositiveInfinity;
                coeffXs[0] = -1.0 * line2DInfo.X1;
            }

            //coeffXs[1] = double.PositiveInfinity;
            //coeffXs[0] = -1.0 * line2DInfo.X1;

        }

        public Equation2D(double slopX2, double slopeX1, double yoffset)
        {
            // make parabolic

        }


        public Equation2D(Pattern2Dim pattern, double pt1x, double pt1y, double pt2x, double pt2y, double pt3x, double pt3y)
        {
            throw new NotImplementedException();

        }

        //public void InitializeFromCv2(Line2D line2DInfo )
        //{

        //}

        public bool IsOn(double x, double y, double tolerance)
        {
            throw new NotImplementedException();
            //return false;
        }

        public double GetLineAngleDegree(double baseAngle = 0)
        {
            if (IsInfinityLine())
                return 90;

            double d = Math.Atan(coeffXs[1]) * 180.0 / Math.PI;
            if (Math.Abs(d - baseAngle) > 90)
            {
                if (d > baseAngle)
                    d -= 180;
                else
                    d += 180;
            }

            return d;

        }

        public List<double> GetY(double x)
        {
            if(coeffYs.Count > 3)
            {
                throw new NotImplementedException();
            }

            if (IsInfinityLine())
            {
                return new List<double>() { 0, 1000000 };
            }

            double y = 0;
            for(int i = 0; i < coeffXs.Count; i++)
            {
                if(coeffXs[i] != 0)
                    y += coeffXs[i] * Math.Pow(x, i);
            }

            if(coeffYs.Count < 2)
            {
                throw new NotImplementedException();
            }
            else if(coeffYs.Count == 2 || (coeffYs.Count==3 && coeffYs[2] == 0)) // linear equation.
            {   
                {
                    y = y / coeffYs[1] - coeffYs[0];
                    return new List<double>() { y };
                }

            }
            else if(coeffYs.Count == 3)
            {
                double y0 = coeffYs[0] - y;
                double offset = Math.Sqrt(Math.Pow(coeffYs[1], 2) - 4.0 * coeffYs[2] * y0);
                return new List<double>()
                {
                    (-1.0 * coeffYs[1] + offset)/ (2.0 * coeffYs[2]),
                    (-1.0 * coeffYs[1] - offset)/ (2.0 * coeffYs[2]),
                };
            }
            else
            {
                throw new NotImplementedException();
            }

            return new List<double>();
        }

        public bool IsInfinityLine()
        {
            return double.IsInfinity(coeffXs[1]);
        }

        public List<Tuple<double, double>> Get2PointFromBound(double x, double y, double width, double height)
        {
            List<Tuple<double, double>> points = new List<Tuple<double, double>>();
            if (IsInfinityLine())
            {
                points.Add(new Tuple<double, double>(-1.0 * coeffXs[0], y));
                points.Add(new Tuple<double, double>(-1.0 * coeffXs[0], y + height));
            }
            else
            {
                points.Add(new Tuple<double, double>(x, GetY(x)[0]));
                points.Add(new Tuple<double, double>(width + x, GetY(width + x)[0]));
            }

            return points;
            //throw new NotImplementedException();
        }


        public List<Point> Get2PointListFromBound(double x, double y, double width, double height)
        {
            List<Point> points = new List<Point>();
            if (IsInfinityLine())
            {
                points.Add(new Point(-1.0 * coeffXs[0], y));
                points.Add(new Point(-1.0 * coeffXs[0], y + height));
            }
            else if( Math.Abs(Math.Abs(GetLineAngleDegree()) - 90) < 45)
            {
                points.Add(new Point(GetX(y), y));
                points.Add(new Point(GetX(y + height), y + height));
            }
            else
            {
                //double firstY = GetY(x)[0];
                
                points.Add(new Point(x, GetY(x)[0]));
                points.Add(new Point(width + x, GetY(width + x)[0]));
            }

            return points;
        }

        public double GetDistanceTo(double x, double y)
        {
            if(IsInfinityLine())
            {
                return Math.Abs(x - CoeffXs[0]);
            }

            double d = Math.Abs(coeffXs[1] * x - y + coeffXs[0] ) / Math.Sqrt(coeffXs[1] * coeffXs[1] + 1);

            return d;
        }



        //public List<Tuple<double, double>> Get2PointFromXBound(double x, double width)
        //{
        //    if(I)

        //}




        public double GetX(double y)
        {
            double x = 0;
            //y = coeffXs[1] * x + coeffXs[0];

            x = (y - coeffXs[0]) / coeffXs[1];

            return x;


            //throw new NotImplementedException();
            //return 0;
        }


        public (double, double, double) GetMatrixABC()
        {
            // return c1 = a1 * x + b1 * y

            double a, b, c;

            if(IsInfinityLine())
            {
                b = 0;
                a = -1;
            }
            else
            {
                b = 1;
                a = -1 * CoeffXs[1];
            }

            c = CoeffXs[0];
            



            return (a, b, c);


        }




    }

    public class Equation3D
    {

    }
}
