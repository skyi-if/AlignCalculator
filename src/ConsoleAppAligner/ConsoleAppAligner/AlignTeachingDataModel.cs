using IntelligentFactory.IFMath;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;


namespace IntelligentFactory
{

    public enum CameraAlignDirection
    {
        R_0,
        R_90,
        R_180,
        R_270
    }

    public class Point4D: ViewModelBase
    {
        double x;
        double y;
        double z;
        double t;

        public double X { get => x; set => SetValue<double>(ref x , value); }
        public double Y { get => y; set => SetValue<double>(ref y , value); }
        public double Z { get => z; set => SetValue<double>(ref z , value); }
        public double T { get => t; set => SetValue<double>(ref t , value); }
    }
    public class TeachingControlViewModelData : ViewModelBase
    {


        ObservableCollection<TeachingImageMark> camCalMarks = new ObservableCollection<TeachingImageMark>();
        ObservableCollection<TeachingImageMark> suttleXMarks = new ObservableCollection<TeachingImageMark>();
        ObservableCollection<TeachingImageMark> suttleYMarks = new ObservableCollection<TeachingImageMark>();
        ObservableCollection<TeachingImageMark> suttleTMarks = new ObservableCollection<TeachingImageMark>();

        TeachingImageMark posTeachingMark = new TeachingImageMark();
        TeachingImageMark foundMark = new TeachingImageMark();

        CalibrationAxis camCalAxis = CalibrationAxis.X;
        CameraAlignDirection camAlignDirection = CameraAlignDirection.R_90;
        bool isTopView = true;

        double camRefDistance = 1;
        double camResultMMPerPixel = 0.016;
        double shtXResultRatio = 1.0; // PLC/cam
        double shtYResultRatio = 1.0; // PLC/cam

        double shtXMovingDist = 2;
        double shtYMovingDist = 2;
        double shtTMovingAngle = 1;

        Point shtTMarkPosInImage = new Point();
        Point shtTMarkPosInRotateCoord = new Point();
        Point shtTImageOrgInRotateCoord = new Point();
        double stddevAboutRotateCoord = 0;

        string recipePath = "";

        bool updateRotateCoord = false;

        bool updateMarkPos = false;


        Point recipeAlignPos = new Point(); //physical pixel
        Point recipeAlignPosImagePixel = new Point();
        Point recipeAlignRotatePos = new Point();




        [XmlArray]
        public ObservableCollection<TeachingImageMark> CamCalMarks { get => camCalMarks; set => camCalMarks = value; }
        [XmlArray]
        public ObservableCollection<TeachingImageMark> ShuttleXMarks { get => suttleXMarks; set => suttleXMarks = value; }
        [XmlArray]
        public ObservableCollection<TeachingImageMark> ShuttleYMarks { get => suttleYMarks; set => suttleYMarks = value; }
        [XmlArray]
        public ObservableCollection<TeachingImageMark> ShuttleTMarks { get => suttleTMarks; set => suttleTMarks = value; }
        [XmlElement]
        public TeachingImageMark PosTeachingMark { get => posTeachingMark; set => SetValue<TeachingImageMark>(ref posTeachingMark, value); }
        [XmlElement]
        public TeachingImageMark RecipeFoundMark { get => foundMark; set => SetValue<TeachingImageMark>(ref foundMark, value); }
        [XmlElement]
        public CalibrationAxis CamCalAxis { get => camCalAxis; set => SetValue<CalibrationAxis>(ref camCalAxis, value); }
        [XmlAttribute]
        public double CamRefDistance { get => camRefDistance; set => SetValue<double>(ref camRefDistance, value); }
        [XmlAttribute]
        public double CamResultMMPerPixel { get => camResultMMPerPixel; set => SetValue<double>(ref camResultMMPerPixel, value); }
        [XmlAttribute]
        public double ShtXResultRatio { get => shtXResultRatio; set => SetValue<double>(ref shtXResultRatio, value); }
        [XmlAttribute]
        public double ShtYResultRatio { get => shtYResultRatio; set => SetValue<double>(ref shtYResultRatio, value); }
        [XmlAttribute]
        public double ShtXMovingDist { get => shtXMovingDist; set => SetValue<double>(ref shtXMovingDist, value); }
        [XmlAttribute]
        public double ShtYMovingDist { get => shtYMovingDist; set => SetValue<double>(ref shtYMovingDist, value); }
        [XmlAttribute]
        public double ShtTMovingAngle { get => shtTMovingAngle; set => SetValue<double>(ref shtTMovingAngle, value); }
        [XmlElement]
        public Point ShtTMarkPosInRotateCoord
        {
            get => shtTMarkPosInRotateCoord; set => SetValue<Point>(ref shtTMarkPosInRotateCoord, value,
            "ShtTMarkPosInRotateCoord", "ShtTMarkPosInRotateCoordString");
        }
        [XmlElement]
        public Point ShtTImageOrgInRotateCoord
        {
            get => shtTImageOrgInRotateCoord; set => SetValue<Point>(ref shtTImageOrgInRotateCoord, value,
            "ShtTImageOrgInRotateCoord", "ShtTImageOrgInRotateCoordString");
        }
        [XmlAttribute]
        public double StddevAboutRotateCoord { get => stddevAboutRotateCoord; set => SetValue<double>(ref stddevAboutRotateCoord, value); }

        [XmlIgnore]
        public string ShtTMarkPosInRotateCoordString { get => shtTMarkPosInRotateCoord.ToString(); }
        [XmlIgnore]
        public string ShtTImageOrgInRotateCoordString { get => ShtTImageOrgInRotateCoord.ToString(); }
        public string RecipePath { get => recipePath; set => SetValue<string>(ref recipePath, value); }
        public Point RecipeAlignPosImagePixel { get => recipeAlignPosImagePixel; set => SetValue<Point>(ref recipeAlignPosImagePixel, value); }
        public Point RecipeAlignPos { get => recipeAlignPos; set => SetValue<Point>(ref recipeAlignPos, value); }
        public bool UpdateRotateCoord { get => updateRotateCoord; set => updateRotateCoord = value; }
        public Point ShtTMarkPosInImage { get => shtTMarkPosInImage; set => SetValue<Point>(ref shtTMarkPosInImage, value); }
        public Point RecipeAlignRotatePos { get => recipeAlignRotatePos; set => SetValue<Point>(ref recipeAlignRotatePos, value); }
        public bool UpdateRecipeMarkPos { get => updateMarkPos; set => updateMarkPos = value; }
        [XmlAttribute]
        public CameraAlignDirection CamAlignDirection { get => camAlignDirection; set => SetValue<CameraAlignDirection>(ref camAlignDirection, value); }
        [XmlAttribute]
        public bool IsTopView { get => isTopView; set => SetValue<bool>(ref isTopView, value); }

        public Point GetRotatePosition(Point imagePixelPoint)
        {

            double dx = imagePixelPoint.X - ShtTMarkPosInImage.X;
            double dy = imagePixelPoint.Y - ShtTMarkPosInImage.Y;

            // change to physical pos.
            dx = dx * CamResultMMPerPixel;
            dy = dy * CamResultMMPerPixel;


            return new Point(ShtTMarkPosInRotateCoord.X + dx,
                ShtTMarkPosInRotateCoord.Y + dy);
        }

        void AddRange(ObservableCollection<TeachingImageMark> tgtList, ObservableCollection<TeachingImageMark> srcList)
        {
            tgtList.Clear();
            foreach (TeachingImageMark mark in srcList)
            {
                tgtList.Add(mark);
            }
        }

        public void CopyFrom(TeachingControlViewModelData src)
        {
            if (src == null)
                throw new NullReferenceException();

            AddRange(CamCalMarks, src.CamCalMarks);
            AddRange(ShuttleXMarks, src.ShuttleXMarks);
            AddRange(ShuttleYMarks, src.ShuttleYMarks);
            AddRange(ShuttleTMarks, src.ShuttleTMarks);
            PosTeachingMark = src.PosTeachingMark;
            RecipeFoundMark = src.RecipeFoundMark;
            CamCalAxis = src.CamCalAxis;
            CamRefDistance = src.CamRefDistance;
            CamResultMMPerPixel = src.CamResultMMPerPixel;
            ShtXResultRatio = src.ShtXResultRatio;
            ShtYResultRatio = src.ShtYResultRatio;
            ShtXMovingDist = src.ShtXMovingDist;
            ShtYMovingDist = src.ShtYMovingDist;
            ShtTMovingAngle = src.ShtTMovingAngle;
            ShtTMarkPosInRotateCoord = src.ShtTMarkPosInRotateCoord;
            ShtTImageOrgInRotateCoord = src.ShtTImageOrgInRotateCoord;
            StddevAboutRotateCoord = src.StddevAboutRotateCoord;
            RecipePath = src.RecipePath;
            RecipeAlignPos = src.RecipeAlignPos;
            UpdateRotateCoord = src.UpdateRotateCoord;
            ShtTMarkPosInImage = src.ShtTMarkPosInImage;
            RecipeAlignRotatePos = src.RecipeAlignRotatePos;
            UpdateRecipeMarkPos = src.UpdateRecipeMarkPos;
            CamAlignDirection = src.CamAlignDirection;



        }

        public void SetTeachingPosition(Point pointImagePixel)
        {
            PosTeachingMark = new TeachingImageMark(CamAlignDirection,
                IsTopView,
                pointImagePixel);
        }


        public void SetAlignPosition(Point pointImagePixel)
        {
            Point physicalImagePixel = TeachingImageMark.GetPhysicalImagePixel(CamAlignDirection,
                IsTopView,
                pointImagePixel);
            RecipeAlignPosImagePixel = pointImagePixel;
            RecipeAlignPos =
                physicalImagePixel;

            //Vm.ShtTMarkPosInImage = Vm.ShuttleTMarks[0].ImagePixelPoint;
            //Vm.ShtTMarkPosInRotateCoord = new Point(posX, posY);
            //Vm.UpdateRotateCoord = true;

            if (UpdateRotateCoord)
            {
                // get offset from MarkPos.
                double dx = RecipeAlignPos.X - ShtTMarkPosInImage.X;
                double dy = RecipeAlignPos.Y - ShtTMarkPosInImage.Y;

                // change to physical pos.
                dx = dx * CamResultMMPerPixel;
                dy = dy * CamResultMMPerPixel;


                RecipeAlignRotatePos = new Point(ShtTMarkPosInRotateCoord.X +
                    dx,
                    ShtTMarkPosInRotateCoord.Y + dy);


                UpdateRecipeMarkPos = true;

            }
        }

    }


    public class AlignTeachingDataModel: ViewModelBase
    {
        Point4D alignRCoord = new Point4D();

        bool rotateDirectionInverse = true;
        double markDistanceMM = -1;

        List<TeachingControlViewModelData> cameraTeaching = new List<TeachingControlViewModelData>();

        [XmlElement]
        public Point4D AlignRCoord { get => alignRCoord; set =>SetValue<Point4D>(ref alignRCoord, value); }
        [XmlArray]
        public List<TeachingControlViewModelData> CameraTeaching { get => cameraTeaching; 
            set =>SetValue<List<TeachingControlViewModelData>>(ref cameraTeaching , value); }
        [XmlAttribute]
        public bool RotateDirectionInverse { get => rotateDirectionInverse; set =>SetValue<bool>(ref rotateDirectionInverse, value); }
        [XmlAttribute]
        public double MarkDistanceMM { get => markDistanceMM; set => SetValue<double>(ref markDistanceMM, value); }

        public void SaveToFile(string path)
        {
            MySerialize.ToXmlFile<AlignTeachingDataModel>(path, this);
        }


        public void Initialize(int camCount)
        {
            for(int i = 0; i< camCount; i++)
            {
                CameraTeaching.Add(new TeachingControlViewModelData());
            }

        }

        public bool Calculate2PointAlign(int index0, int index1)
        {
            Point pa1 = CameraTeaching[index0].GetRotatePosition(
                CameraTeaching[index0].PosTeachingMark.PhysicalImagePixelPoint);
            Point pa2 = CameraTeaching[index1].GetRotatePosition(
                CameraTeaching[index1].PosTeachingMark.PhysicalImagePixelPoint);

            Point pb1 = CameraTeaching[index0].RecipeAlignRotatePos;
            Point pb2 = CameraTeaching[index1].RecipeAlignRotatePos;

            bool setTemporary90Rot = false;
            if((CameraTeaching[index0].CamAlignDirection == CameraAlignDirection.R_90
                 || CameraTeaching[index0].CamAlignDirection == CameraAlignDirection.R_270)
                && (
                CameraTeaching[index1].CamAlignDirection == CameraAlignDirection.R_90
                 || CameraTeaching[index1].CamAlignDirection == CameraAlignDirection.R_270
                )
                )
            {
                setTemporary90Rot = true;
            }


            double angleDirection = 1.0;
            //if (RotateDirectionInverse)
            {
                angleDirection = -1.0;
            }

            double originalT = GetTheta(setTemporary90Rot, pa1, pa2, pb1, pb2);


            AlignRCoord.T = angleDirection * originalT;

            if(Math.Abs(AlignRCoord.T) >= 5)
            {
                Console.WriteLine($"Calculated align angle is too large: {AlignRCoord.T}");
                return false;
            }


            Point r1 = GetRotatePoint(pb1, AlignRCoord.T);
            Point r2 = GetRotatePoint(pb2, AlignRCoord.T);


            //VTS.Logger.Write()

            Console.WriteLine($"r1:{r1.ToString()}, dTP:{r1.X - pa1.X:f2}, {r1.Y - pa1.Y:f2}");
            Console.WriteLine($"r2:{r2.ToString()}, dTP:{r2.X - pa2.X:f2}, {r2.Y - pa2.Y:f2}");

            Point deltaP = new Point(((r1.X - pa1.X) + (r2.X - pa2.X))/2.0
                ,((r1.Y - pa1.Y) + (r2.Y - pa2.Y))/2.0
                );

            AlignRCoord.X = deltaP.X;
            AlignRCoord.Y = deltaP.Y;

            if (!RotateDirectionInverse )
            {                
                AlignRCoord.Y *= -1.0;
            }

            return true;
        }


        public void SetTeachingPoint(int camIdx, Point imagePixelPoint)
        {
            CameraTeaching[camIdx].SetTeachingPosition(imagePixelPoint);
        }

        public void SetAlignPoint(int camIdx, Point imagePixelPoint)
        {
            // set and change physical point, rotate point.
            CameraTeaching[camIdx].SetAlignPosition(imagePixelPoint);
        }

        public Point4D AutoRun2PointAlign(Point cam1ImagePixel, Point cam2ImagePixel
            , Point cam1TeachingPixel, Point cam2TeachingPixel, string saveRootPath = null
            )
        {
            try
            {
                SetTeachingPoint(0, cam1TeachingPixel);
                SetTeachingPoint(1, cam2TeachingPixel);

                SetAlignPoint(0, cam1ImagePixel);
                SetAlignPoint(1, cam2ImagePixel);

                if(Calculate2PointAlign(0, 1))
                    return new Point4D() { X = AlignRCoord.X, Y = AlignRCoord.Y, T = AlignRCoord.T };

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
            
        }


        public static Point GetRotatePoint(Point point, double dT)
        {
            double radT = dT * Math.PI / 180.0;
            return new Point(Math.Cos(radT) * point.X - Math.Sin(radT) * point.Y,
                Math.Sin(radT) * point.X + Math.Cos(radT) * point.Y
                );
        }

        public static double ChangePLCRotateCoord(double T)
        {
            return -1.0 * T;
        }

        public static double GetTheta(bool setTemp90Rot, Point pa1, Point pa2, Point pb1, Point pb2)
        {

            return (GetAngle(setTemp90Rot, pb1, pb2) - 
                GetAngle(setTemp90Rot, pa1, pa2));

        }

        public static double GetAngle(bool setTemp90Rot, Point start, Point end)
        {

            //if(setTemp90Rot)
            //{
            //    start = GetRotatePoint(start, 90);
            //    end = GetRotatePoint(end, 90);
            //}

            double dy = end.Y - start.Y;
            double dx = end.X - start.X;
            double dydx = dy / dx;
            double angle = Math.Atan(dydx) * (180.0 / Math.PI);

            
            if (double.IsNaN(angle))
            {
                return 90;
            }

            //if (setTemp90Rot)
            //{
            //    angle -= 90;
            //}

            if (dx < 0.0)
            {
                angle += 180.0;
            }
            else
            {
                if (dy < 0.0) angle += 360.0;
            }
            return angle;
        }

        public static double GetAngleAtan()
        {

            return 0;
        }


    }
}
