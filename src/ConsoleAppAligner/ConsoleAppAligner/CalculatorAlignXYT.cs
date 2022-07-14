using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentFactory.IFMath
{
    public class CalculatorAlignXYT
    {
        public static (double x0, double y0) FindOriginalCoordinatePosition(double dX, double dY, double deltaTheta)
        {
            double x0 = 0, y0 = 0;
            double dRad_2 = deltaTheta * Math.PI / 360.0;


            // first equation.
            // dX = -2 * (y0 * cos(dT/2) + x0* sin(dT/2))* sin(dT/2);
            // dX = -2 * y0 * cos(dT/2) * sin(dT/2) -2 * x0*sin(dT/2) *sin(dT/2);
            // dX = -2 * x0*sin(dT/2) *sin(dT/2) -2 * y0 * cos(dT/2) * sin(dT/2);
            // second equation
            // dY = 2 * (x0 * cos(dT/2) - y0 * sin(dT/2)) * sin(dT/2);
            // dY = 2 * x0 *cos(dT/2) * sin(dT/2) - 2 * y0 * sin(dT/2) * sin(dT/2);

            double sinDT_2 = Math.Sin(dRad_2);
            double cosDT_2 = Math.Cos(dRad_2);

            double[,] dEqParams = new double[2,2];

            dEqParams[0, 0] = -2.0 * sinDT_2 * sinDT_2; //-2 * x0*sin(dT/2) *sin(dT/2)
            dEqParams[0, 1] = -2.0 * cosDT_2 * sinDT_2; //-2 * y0 * cos(dT/2) * sin(dT/2)

            dEqParams[1, 0] = 2.0 * cosDT_2 * sinDT_2; // 2 * x0 *cos(dT/2) * sin(dT/2)
            dEqParams[1,1] = -2.0 * sinDT_2 * sinDT_2; // - 2 * y0 * sin(dT/2) * sin(dT/2)

            double[] dVals = new double[2];
            dVals[0] = dX;
            dVals[1] = dY;

            OpenCvSharp.Mat matEq = new OpenCvSharp.Mat(2, 2, OpenCvSharp.MatType.CV_64FC1, dEqParams);
            OpenCvSharp.Mat matVal = new OpenCvSharp.Mat(2, 1, OpenCvSharp.MatType.CV_64FC1, dVals);

            OpenCvSharp.Mat matResult = new OpenCvSharp.Mat();

            OpenCvSharp.Cv2.Solve(matEq, matVal, matResult);

            x0 = matResult.At<double>(0);// .Ro.Col[]
            y0 = matResult.At<double>(1);

            return (x0, y0);
        }

        public static void TestFindOrgCoordinate()
        {
            var (x0, y0) = IntelligentFactory.IFMath.CalculatorAlignXYT.FindOriginalCoordinatePosition((2.0 - 1.414), 1.414, 45.0);

            Console.WriteLine($"org.X = {x0}, org.Y = {y0}");

        }

 


    }
}
