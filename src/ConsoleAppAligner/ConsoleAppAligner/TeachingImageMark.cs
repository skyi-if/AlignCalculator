using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace IntelligentFactory
{
    public class Point
    {
        double x;
        double y;

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }

        public Point() { }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    public enum CalibrationAxis
    {
        None,
        X,
        Y,
        Z,
        T,
    }

    public class TeachingMotor: ViewModelBase
    {
        enum AxisType
        {
            None,
            X,
            Y,
            Z,
            T
        }

        string deviceName = "";
        bool reverse = false;
        AxisType imageAxisType = AxisType.None;
        AxisType realAxisType = AxisType.None;

        double currentPos = 0;

        public string DeviceName { get => deviceName; set =>SetValue<string>(ref deviceName , value); }
        public bool Reverse { get => reverse; set => SetValue<bool>(ref reverse , value); }
        private AxisType ImageAxisType { get => imageAxisType; set => SetValue<AxisType>(ref imageAxisType , value); }
        private AxisType RealAxisType { get => realAxisType; set => SetValue<AxisType>(ref realAxisType, value); }
        public double CurrentPos { get => currentPos; set => SetValue<double>(ref currentPos , value); }
    }

    public class TeachingImageMark: ViewModelBase
    {
        int id;
        Point controlPoint = new Point();
        Point imagePixelPoint = new Point();
        Point physicalImagePixelPoint = new Point(); 

        string cameraId = "";

        double posCamX = 0;
        double posCamY = 0;
        double posCamZ = 0;

        double shuttleX = 0;
        double shuttleY = 0;
        double shuttleZ = 0;
        double shuttleT = 0;



        public TeachingImageMark()
        {

        }

        //public TeachingImageMark(CameraAlignDirection camDirection, bool isTopView, TargetMarkEventArgs targetMarkEventArgs)
        //{
        //    if(targetMarkEventArgs != null)
        //    {
        //        Id = targetMarkEventArgs.Id;
        //        controlPoint = targetMarkEventArgs.ControlPoint;
        //        imagePixelPoint = targetMarkEventArgs.ImagePixelPoint;
        //        physicalImagePixelPoint = GetPhysicalImagePixel(camDirection, isTopView, imagePixelPoint);
        //    }
            
        //}

        public TeachingImageMark(CameraAlignDirection camDirection, bool isTopView, Point imagePixelPoint)
        {
            Id = -1;
            controlPoint = imagePixelPoint;
            this.imagePixelPoint = imagePixelPoint;
            physicalImagePixelPoint = GetPhysicalImagePixel(camDirection, isTopView, imagePixelPoint);
            
        }


        public static Point GetPhysicalImagePixel(CameraAlignDirection camDirection, bool isTopView, Point point)
        {
            Point phyPixel = new Point( point.X, point.Y);

            if(!isTopView)
            {
                phyPixel = GetXAxisSymmetry(phyPixel);
            }

            if(camDirection == CameraAlignDirection.R_0)
            {
                phyPixel = new Point( point.X, -1 * point.Y);
            }
            else if(camDirection == CameraAlignDirection.R_90)
            {
                phyPixel = new Point(point.Y, point.X);
            }
            else if(camDirection == CameraAlignDirection.R_180)
            {
                phyPixel = new Point(-1 * point.X, point.Y);
            }
            else if(camDirection == CameraAlignDirection.R_270)
            {
                phyPixel = new Point(-1 * point.Y, -1 * point.X);
            }

            return phyPixel;
        }

        public static Point GetXAxisSymmetry(Point src)
        {
            return new Point(src.X * -1.0, src.Y);
        }


        public int Id { get => id; set =>SetValue<int>(ref id , value); }
        public Point ControlPoint { get => controlPoint; set => SetValue<Point>(ref controlPoint, value); }
        public Point ImagePixelPoint { get => imagePixelPoint; set => SetValue<Point>(ref imagePixelPoint, value); }
        public Point PhysicalImagePixelPoint { get => physicalImagePixelPoint; set => SetValue<Point>(ref physicalImagePixelPoint, value); }
        public string CameraId { get => cameraId; set => SetValue<string>(ref cameraId, value); }
        public double PosCamX { get => posCamX; set => SetValue<double>(ref posCamX, value); }
        public double PosCamY { get => posCamY; set => SetValue<double>(ref posCamY, value); }
        public double PosCamZ { get => posCamZ; set => SetValue<double>(ref posCamZ, value); }
        public double ShuttleX { get => shuttleX; set => SetValue<double>(ref shuttleX, value); }
        public double ShuttleY { get => shuttleY; set => SetValue<double>(ref shuttleY, value); }
        public double ShuttleZ { get => shuttleZ; set => SetValue<double>(ref shuttleZ, value); }
        public double ShuttleT { get => shuttleT; set => SetValue<double>(ref shuttleT ,value); }
    }
}
