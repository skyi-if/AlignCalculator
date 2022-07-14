using IntelligentFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppAligner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //        var (x0, y0) = IntelligentFactory.IFMath.CalculatorAlignXYT.FindOriginalCoordinatePosition(
            //-1 + 1.414/2, 1.414/2, 45);

            double[] deltaTList = new double[]
            {
                0
                ,0.04
                ,0.08
                ,-0.04
                ,0.06

            };

            Point[] inputListCam1 = new Point[] {
                new Point(1299 , 972),
                new Point(1220  ,1014),
                new Point(1143  ,1059),
                new Point(1374  ,929),
                new Point(1125 ,1101),
            };

            Point[] inputListCam2 = new Point[] {
                new Point(1291  ,971),
                new Point(1221  ,928),
                new Point(1150  ,883),
                new Point(1363  ,1016),
                new Point(1131  ,967),
            };

            //double[] deltaTList = new double[]
            //{
            //                0
            //                ,0.04
            //                ,0.08
            //                ,-0.04
            //                ,-0.08

            //};

            //Point[] inputListCam1 = new Point[] {
            //    new Point(1296 , 972),
            //    new Point(1224  ,1017),
            //    new Point(1143  ,1059),
            //    new Point(1368  ,932),
            //    new Point(1439 ,888),
            //};

            //Point[] inputListCam2 = new Point[] {
            //    new Point(1297  ,974),
            //    new Point(1227  ,929),
            //    new Point(1150  ,883),
            //    new Point(1370  ,1018),
            //    new Point(1442  ,1063),
            //};

            Console.WriteLine("회전원점이 구해지는지 시험하기");
            Console.WriteLine("[LeftCam] origin point at the view of image coordinate =========== ");

            for(int i=1;i<deltaTList.Length-1;i++)
            {
                Point orgWithImgCoord = GetOriginPointInImageCoordinate(inputListCam1[i - 1], inputListCam1[i], deltaTList[i] - deltaTList[i-1]);
                Console.WriteLine($"index={i}, deltaT={deltaTList[i] - deltaTList[i - 1]}, org={orgWithImgCoord.X}, {orgWithImgCoord.Y}");
            }

            for (int i = 1; i < deltaTList.Length - 1; i++)
            {
                Point orgWithImgCoord = GetOriginPointInImageCoordinate(inputListCam1[0], inputListCam1[i], deltaTList[i] - deltaTList[0]);
                Console.WriteLine($"index0=try{i}, deltaT={deltaTList[i] - deltaTList[0]}, org={orgWithImgCoord.X}, {orgWithImgCoord.Y}");
            }

            Console.WriteLine("\r\n[RightCam] origin point at the view of image coordinate =========== ");
            for (int i = 1; i < deltaTList.Length - 1; i++)
            {
                Point orgWithImgCoord = GetOriginPointInImageCoordinate(inputListCam2[i - 1], inputListCam2[i], deltaTList[i] - deltaTList[i - 1]);
                Console.WriteLine($"index={i}, deltaT={deltaTList[i] - deltaTList[i - 1]}, org={orgWithImgCoord.X}, {orgWithImgCoord.Y}");
            }

            for (int i = 1; i < deltaTList.Length - 1; i++)
            {
                Point orgWithImgCoord = GetOriginPointInImageCoordinate(inputListCam2[0], inputListCam2[i], deltaTList[i] - deltaTList[0]);
                Console.WriteLine($"index0=try{i}, deltaT={deltaTList[i] - deltaTList[0]}, org={orgWithImgCoord.X}, {orgWithImgCoord.Y}");
            }


            Console.WriteLine("\r\n\r\n");
            Console.WriteLine(">>>> [Calibration data test for saving] <<<<");
            double camResolution = 0.0073;

            // 신뢰성이 있어 보이는 calibration은 index2->index3 이동과, index0->index2 이동.
            Console.WriteLine("Create calibration data");
            // 캘리브레이션으로 기억해야 하는 데이터.
            // cam1 = cam1CalImgPoint, cam1CalRotatePoint
            Point cam1CalImgPoint = inputListCam1[0];
            Point cam1CalRotatePoint = GetReferencePointWithKnownDeltaTheta(inputListCam1[0], inputListCam1[2], deltaTList[2] - deltaTList[0], camResolution);

            // 캘리브레이션으로 기억해야 하는 데이터.
            // cam2 = cam2CalImgPoint, cam2CalRotatePoint
            Point cam2CalImgPoint = inputListCam2[0];
            Point cam2CalRotatePoint = GetReferencePointWithKnownDeltaTheta(inputListCam2[0], inputListCam2[2], deltaTList[2] - deltaTList[0], camResolution);

            Console.WriteLine($"CalData, Cam1: ImageCoord({cam1CalImgPoint.X},{cam1CalImgPoint.Y}), RotateCoord({cam1CalRotatePoint.X:F2},{cam1CalRotatePoint.Y:F2})");
            Console.WriteLine($"CalData, Cam2: ImageCoord({cam2CalImgPoint.X},{cam2CalImgPoint.Y}), RotateCoord({cam2CalRotatePoint.X:F2},{cam2CalRotatePoint.Y:F2})");

            Console.WriteLine("\r\n");


            Console.WriteLine("[align test simulation] about point-4");

            Point alignTestCam1 = inputListCam1[4];
            Point alignTestCam2 = inputListCam2[4];

            //Point alignTestCam1 = new Point(1301, 1350);//  inputListCam1[4];
            //Point alignTestCam2 = new Point(1301, 1350); //inputListCam2[4];
            //Point alignTestCam1 = new Point(1644, 1409);//  inputListCam1[4];
            //Point alignTestCam2 = new Point(1643, 1740); //inputListCam2[4];
            Console.WriteLine($"Found Mark in image: cam1=({alignTestCam1.X},{alignTestCam1.Y}) cam2=({alignTestCam2.X},{alignTestCam2.Y})\r\n");

            // exam 1
            // teaching point = point 0
            Console.WriteLine("Exam 01 =========================== >>");
            Point teachingCam1 = inputListCam1[0];
            Point teachingCam2 = inputListCam2[0];

            //Point teachingCam1 = new Point(1301, 1350);
            //Point teachingCam2 = new Point(1301, 1350);

            Point4D alignVal = CalculateAlignValue(
                teachingCam1, alignTestCam1, cam1CalImgPoint, cam1CalRotatePoint,
                teachingCam2, alignTestCam2, cam2CalImgPoint, cam2CalRotatePoint, 
                camResolution);

            Point4D alignVal2 = CalculateAlignValueWithCamRotated(
    teachingCam1, alignTestCam1, cam1CalImgPoint, cam1CalRotatePoint,
    teachingCam2, alignTestCam2, cam2CalImgPoint, cam2CalRotatePoint,
    camResolution);



            Console.WriteLine($"Align target ({teachingCam1.X},{teachingCam1.Y}),({teachingCam2.X},{teachingCam2.Y})\r\nexpected: x:??, y:??, theta:??");
            Console.WriteLine($"result    x:{alignVal.X:F3}, y:{alignVal.Y:F3}, theta:{alignVal.T:F3}");
            Console.WriteLine($"result2    x:{alignVal2.X:F3}, y:{alignVal2.Y:F3}, theta:{alignVal2.T:F3}");


            // exam 2
            // teaching point = point 1
            Console.WriteLine("\r\nExam 02 =========================== >>");
            teachingCam1 = inputListCam1[1];
            teachingCam2 = inputListCam2[1];

            alignVal = CalculateAlignValue(
                teachingCam1, alignTestCam1, cam1CalImgPoint, cam1CalRotatePoint,
                teachingCam2, alignTestCam2, cam2CalImgPoint, cam2CalRotatePoint,
                camResolution);
            alignVal2 = CalculateAlignValueWithCamRotated(
                teachingCam1, alignTestCam1, cam1CalImgPoint, cam1CalRotatePoint,
                teachingCam2, alignTestCam2, cam2CalImgPoint, cam2CalRotatePoint,
                camResolution);

            Console.WriteLine($"Align Exam(index4 -> index1): expected: x:0.4, y:0.4, theta:0.02");
            Console.WriteLine($"result    x:{alignVal.X:F3}, y:{alignVal.Y:F3}, theta:{alignVal.T:F3}");
            Console.WriteLine($"result2    x:{alignVal2.X:F3}, y:{alignVal2.Y:F3}, theta:{alignVal2.T:F3}");
            Console.WriteLine("<<====================================\r\n");

            // exam 3
            // teaching point = point 3
            Console.WriteLine("Exam 03 =========================== >>");
            teachingCam1 = inputListCam1[3];
            teachingCam2 = inputListCam2[3];

            alignVal = CalculateAlignValue(
                teachingCam1, alignTestCam1, cam1CalImgPoint, cam1CalRotatePoint,
                teachingCam2, alignTestCam2, cam2CalImgPoint, cam2CalRotatePoint,
                camResolution);

            alignVal2 = CalculateAlignValueWithCamRotated(
                teachingCam1, alignTestCam1, cam1CalImgPoint, cam1CalRotatePoint,
                teachingCam2, alignTestCam2, cam2CalImgPoint, cam2CalRotatePoint,
                camResolution);
            Console.WriteLine($"Align Exam(index4 -> index1): expected: x:0.4, y:0.4, theta:0.12");
            Console.WriteLine($"result    x:{alignVal.X:F3}, y:{alignVal.Y:F3}, theta:{alignVal.T:F3}");
            Console.WriteLine($"result2    x:{alignVal2.X:F3}, y:{alignVal2.Y:F3}, theta:{alignVal2.T:F3}");
            Console.WriteLine("<<====================================\r\n");



            // exam 04
            Console.WriteLine("\r\n");
            Console.WriteLine("Exam 04 =========================== >>");

           deltaTList = new double[]
            {
                            0
                            ,0.04
                            ,0.08
                            ,-0.04
                            ,-0.08

            };

            inputListCam1 = new Point[] {
                new Point(1296 , 972),
                new Point(1224  ,1017),
                new Point(1143  ,1059),
                new Point(1368  ,932),
                new Point(1439 ,888),
            };

            inputListCam2 = new Point[] {
                new Point(1297  ,974),
                new Point(1227  ,929),
                new Point(1150  ,883),
                new Point(1370  ,1018),
                new Point(1442  ,1063),
            };

            Console.WriteLine("Create New calibration data");
            // 캘리브레이션으로 기억해야 하는 데이터.
            // cam1 = cam1CalImgPoint, cam1CalRotatePoint
            cam1CalImgPoint = inputListCam1[0];
            cam1CalRotatePoint = GetReferencePointWithKnownDeltaTheta(inputListCam1[0], inputListCam1[2], deltaTList[2] - deltaTList[0], camResolution);

            // 캘리브레이션으로 기억해야 하는 데이터.
            // cam2 = cam2CalImgPoint, cam2CalRotatePoint
            cam2CalImgPoint = inputListCam2[0];
            cam2CalRotatePoint = GetReferencePointWithKnownDeltaTheta(inputListCam2[0], inputListCam2[2], deltaTList[2] - deltaTList[0], camResolution);

            Console.WriteLine($"CalData, Cam1: ImageCoord({cam1CalImgPoint.X},{cam1CalImgPoint.Y}), RotateCoord({cam1CalRotatePoint.X:F2},{cam1CalRotatePoint.Y:F2})");
            Console.WriteLine($"CalData, Cam2: ImageCoord({cam2CalImgPoint.X},{cam2CalImgPoint.Y}), RotateCoord({cam2CalRotatePoint.X:F2},{cam2CalRotatePoint.Y:F2})");

            Console.WriteLine("\r\n");

            //Point alignTestCam1 = inputListCam1[4];
            //Point alignTestCam2 = inputListCam2[4];

            alignTestCam1 = new Point(1301, 1350);//  inputListCam1[4];
            alignTestCam2 = new Point(1301, 1350); //inputListCam2[4];
            //Point alignTestCam1 = new Point(1644, 1409);//  inputListCam1[4];
            //Point alignTestCam2 = new Point(1643, 1740); //inputListCam2[4];
            Console.WriteLine($"Found Mark in image: cam1=({alignTestCam1.X},{alignTestCam1.Y}) cam2=({alignTestCam2.X},{alignTestCam2.Y})\r\n");

            // exam 4
            // teaching point = point 0

            teachingCam1 = inputListCam1[0];
            teachingCam2 = inputListCam2[0];

            //teachingCam1 = new Point(1301, 1350);
            //teachingCam2 = new Point(1301, 1350);

            alignVal = CalculateAlignValue(
                teachingCam1, alignTestCam1, cam1CalImgPoint, cam1CalRotatePoint,
                teachingCam2, alignTestCam2, cam2CalImgPoint, cam2CalRotatePoint,
                camResolution);

            alignVal2 = CalculateAlignValueWithCamRotated(
                teachingCam1, alignTestCam1, cam1CalImgPoint, cam1CalRotatePoint,
                teachingCam2, alignTestCam2, cam2CalImgPoint, cam2CalRotatePoint,
                camResolution);
            Console.WriteLine($"Align target ({teachingCam1.X},{teachingCam1.Y}),({teachingCam2.X},{teachingCam2.Y})\r\nexpected: x:??, y:??, theta:??");
            Console.WriteLine($"result    x:{alignVal.X:F3}, y:{alignVal.Y:F3}, theta:{alignVal.T:F3}");
            Console.WriteLine($"result2    x:{alignVal2.X:F3}, y:{alignVal2.Y:F3}, theta:{alignVal2.T:F3}");

            Console.ReadLine();
        }

        void HowToCalibrationAndToUseSample()
        {
            double cameraResolutionMMPerPixel = 1;
            Point T이동후Point = new Point();
            Point T이동전Point = new Point();

            /// 이동전좌표와 계산된좌표가 회전좌표계 변환에 사용됩니다.
            /// 카메라 1,2에 대해서 이 내용을 각각 저장해서 사용하면 됩니다.
            Point 이동전Point의회전좌표계위치 = GetReferencePointWithKnownDeltaTheta(T이동전Point, T이동후Point, 1, cameraResolutionMMPerPixel);


            //Point 이미지의어떤좌표1 =new Point();
            //Point 어떤좌표1의회전좌표위치 = ChangeToRotateCoordinatePoint(이미지의어떤좌표1, T이동전Point, 이동전Point의회전좌표계위치, cameraResolutionMMPerPixel);


            // align 하기.
            /// 아래는 1회 align을 계산하는 예제 입니다.
            /// 이 align은 카메라의 직진도가 정확해야 합니다.
            Point cam1티칭 = new Point();
            Point cam2티칭 = new Point();
            Point cam1현재이미지마크 = new Point();
            Point cam2현재이미지마크 = new Point();

            Point cam1회전좌표이미지기준위치 = new Point();
            Point cam1회전좌표계산결과 = new Point();

            Point cam2회전좌표이미지기준위치 = new Point();
            Point cam2회전좌표계산결과 = new Point();


            Point4D alignVal = CalculateAlignValue(cam1티칭, cam1현재이미지마크, cam1회전좌표이미지기준위치, cam1회전좌표계산결과,
                cam2티칭, cam2현재이미지마크, cam2회전좌표이미지기준위치, cam2회전좌표계산결과, cameraResolutionMMPerPixel);

        }

        /// <summary>
        /// Theta를1번이동한두점에서회전좌표계구하기
        /// </summary>
        /// <param name="T이동전Point"></param>
        /// <param name="T이동후Point"></param>
        /// <param name="T이동후360"></param>
        /// <param name="T이동전360"></param>
        /// <param name="cameraResolutionMMPerPixel"></param>
        /// <returns></returns>
        static Point GetReferencePointWithKnownDeltaTheta(Point T이동전Point, Point T이동후Point, double deltaT, double cameraResolutionMMPerPixel)
        {

            double dx = (T이동후Point.X - T이동전Point.X) *
                cameraResolutionMMPerPixel;
            double dy = (T이동후Point.Y - T이동전Point.Y) *
                cameraResolutionMMPerPixel;


            var (x0, y0) = IntelligentFactory.IFMath.CalculatorAlignXYT.FindOriginalCoordinatePosition(
                dx, dy, deltaT);

            return new Point(x0, y0);

        }

        static Point GetOriginPointInImageCoordinate(Point T이동전Point, Point T이동후Point, double deltaT)
        {
            double dx = (T이동후Point.X - T이동전Point.X) ;
            double dy = (T이동후Point.Y - T이동전Point.Y) ;

            var (x0, y0) = IntelligentFactory.IFMath.CalculatorAlignXYT.FindOriginalCoordinatePosition(
                dx, dy, deltaT);

            Point point = new Point(T이동전Point.X - 1 * x0, T이동전Point.Y -1 * y0);

            return point;
        }

        // 이미지어떤좌표를회전좌표로변환하기
        static Point ChangeToRotateCoordinatePoint(Point 어떤좌표1, Point T이동전Point, Point 이동전Point의회전좌표계위치, double cameraResolutionMMPerPixel)
        {
            double dx = 어떤좌표1.X - T이동전Point.X;
            double dy = 어떤좌표1.Y - T이동전Point.Y;

            // change to physical pos.
            dx = dx * cameraResolutionMMPerPixel;
            dy = dy * cameraResolutionMMPerPixel;


            return new Point(이동전Point의회전좌표계위치.X + dx,
                이동전Point의회전좌표계위치.Y + dy);
        }

        static Point ChangeToRotateCoordinatePoint2(Point 어떤좌표1, Point T이동전Point, Point 이동전Point의회전좌표계위치, double radTCamCoordFromRotatedCoord, double cameraResolutionMMPerPixel)
        {
            double dx = 어떤좌표1.X - T이동전Point.X;
            double dy = 어떤좌표1.Y - T이동전Point.Y;

            // change to physical pos.
            dx = dx * cameraResolutionMMPerPixel;
            dy = dy * cameraResolutionMMPerPixel;

            Point roatedDelta = GetVirtualRotatedPoint(new Point(dx, dy), radTCamCoordFromRotatedCoord * 180.0 / Math.PI);


            return new Point(이동전Point의회전좌표계위치.X + roatedDelta.X,
                이동전Point의회전좌표계위치.Y + roatedDelta.Y);
        }


        static Point4D CalculateAlignValueWithCamRotated(Point cam1TeachingImage, Point cam1NewMarkPos, Point cam1RotateRefImagePoint, Point cam1RotateRefRotatedPoint,
            Point cam2TeachingImage, Point cam2NewMarkPos, Point cam2RotateRefImagePoint, Point cam2RotateRefRotatedPoint,
            double cameraResolutionMMPerPixel
            )
        {

            //////////////////////////
            // 변경점 1. -> 이건 cal 할때 미리 계산 가능.
            // 물류좌표계와 회전좌표계에 x축의 회전 각도가 있을 경우에...
            // xy 이동 좌표를 회전 변환을 주어 다시 계산해야 함.

            // step 1-1. get camera Theta about roated coordinate
            //   1-1-1. get calpoint Theta in cam coordinate
            double distanceBetweenCalPoints = Math.Sqrt(Math.Pow(cam2RotateRefRotatedPoint.X - cam1RotateRefRotatedPoint.X, 2) +
                                                        Math.Pow(cam2RotateRefRotatedPoint.Y - cam1RotateRefRotatedPoint.Y, 2));

            double radBetweenCalPoints = Math.Asin((cam2RotateRefRotatedPoint.Y - cam1RotateRefRotatedPoint.Y) / distanceBetweenCalPoints);


            //   1-1-2. get camera coord Theta in rotated coordinate
            Point r1Cal = GetVirtualRotatedPoint(cam1RotateRefRotatedPoint, radBetweenCalPoints * 180.0 / Math.PI);
            Point r2Cal = GetVirtualRotatedPoint(cam2RotateRefRotatedPoint, radBetweenCalPoints * 180.0 / Math.PI);

            double distanceForCamCoordThera = Math.Sqrt(Math.Pow(r2Cal.X - r1Cal.X, 2) +
                                            Math.Pow(r2Cal.Y - r1Cal.Y, 2));

            double radForCamCoordThera = Math.Asin((r2Cal.Y - r1Cal.Y) / distanceForCamCoordThera); // 이 값이 cam coordinate와 rotate coordinate 사이의 theta.

            // end of 변경점 1.
            /////////////////////////////////////////////////


            Point4D AlignRCoord = new Point4D();
            Point cam1TeachingRotated = ChangeToRotateCoordinatePoint2(cam1TeachingImage, cam1RotateRefImagePoint, cam1RotateRefRotatedPoint, radForCamCoordThera, cameraResolutionMMPerPixel);
            Point cam2TeachingRotated = ChangeToRotateCoordinatePoint2(cam2TeachingImage, cam2RotateRefImagePoint, cam2RotateRefRotatedPoint, radForCamCoordThera, cameraResolutionMMPerPixel);

            Point cam1NewMarkRotated = ChangeToRotateCoordinatePoint2(cam1NewMarkPos, cam1RotateRefImagePoint, cam1RotateRefRotatedPoint, radForCamCoordThera, cameraResolutionMMPerPixel);
            Point cam2NewMarkRotated = ChangeToRotateCoordinatePoint2(cam2NewMarkPos, cam2RotateRefImagePoint, cam2RotateRefRotatedPoint, radForCamCoordThera, cameraResolutionMMPerPixel);

            double originalT = GetTheta(false, cam1TeachingRotated, cam2TeachingRotated, cam1NewMarkRotated, cam2NewMarkRotated);
            double angleDirection = -1.0;
            AlignRCoord.T = angleDirection * originalT;


            Point r1 = GetVirtualRotatedPoint(cam1NewMarkRotated, AlignRCoord.T);
            Point r2 = GetVirtualRotatedPoint(cam2NewMarkRotated, AlignRCoord.T);
            Console.WriteLine($"  r1:({r1.X:F2},{r1.Y:F2}), dTP:{r1.X - cam1TeachingRotated.X:f2}, {r1.Y - cam1TeachingRotated.Y:f2}");
            Console.WriteLine($"  r2:({r2.X:F2},{r2.Y:F2}), dTP:{r2.X - cam2TeachingRotated.X:f2}, {r2.Y - cam2TeachingRotated.Y:f2}");

            Point deltaP = new Point(((r1.X - cam1TeachingRotated.X) + (r2.X - cam2TeachingRotated.X)) / 2.0
                , ((r1.Y - cam1TeachingRotated.Y) + (r2.Y - cam2TeachingRotated.Y)) / 2.0
                );


            
            // -1 is rotated coordinate to camera coordinate.
            Point delraPWithCamRotation = GetVirtualRotatedPoint(deltaP, -1 * radForCamCoordThera * 180.0 / Math.PI);


            AlignRCoord.X = delraPWithCamRotation.X;
            AlignRCoord.Y = delraPWithCamRotation.Y;

            return AlignRCoord;

        }

        static Point4D CalculateAlignValue(Point cam1TeachingImage, Point cam1NewMarkPos, Point cam1RotateRefImagePoint, Point cam1RotateRefRotatedPoint,
            Point cam2TeachingImage, Point cam2NewMarkPos, Point cam2RotateRefImagePoint, Point cam2RotateRefRotatedPoint,
            double cameraResolutionMMPerPixel
            )
        {



            Point4D AlignRCoord = new Point4D();
            Point cam1TeachingRotated = ChangeToRotateCoordinatePoint(cam1TeachingImage, cam1RotateRefImagePoint, cam1RotateRefRotatedPoint,  cameraResolutionMMPerPixel);
            Point cam2TeachingRotated = ChangeToRotateCoordinatePoint(cam2TeachingImage, cam2RotateRefImagePoint, cam2RotateRefRotatedPoint,  cameraResolutionMMPerPixel);

            Point cam1NewMarkRotated = ChangeToRotateCoordinatePoint(cam1NewMarkPos, cam1RotateRefImagePoint, cam1RotateRefRotatedPoint,  cameraResolutionMMPerPixel);
            Point cam2NewMarkRotated = ChangeToRotateCoordinatePoint(cam2NewMarkPos, cam2RotateRefImagePoint, cam2RotateRefRotatedPoint,  cameraResolutionMMPerPixel);

            double originalT = GetTheta(false, cam1TeachingRotated, cam2TeachingRotated, cam1NewMarkRotated, cam2NewMarkRotated);
            double angleDirection = -1.0;
            AlignRCoord.T = angleDirection * originalT;


            Point r1 = GetVirtualRotatedPoint(cam1NewMarkRotated, AlignRCoord.T);
            Point r2 = GetVirtualRotatedPoint(cam2NewMarkRotated, AlignRCoord.T);
            Console.WriteLine($"  r1:({r1.X:F2},{r1.Y:F2}), dTP:{r1.X - cam1TeachingRotated.X:f2}, {r1.Y - cam1TeachingRotated.Y:f2}");
            Console.WriteLine($"  r2:({r2.X:F2},{r2.Y:F2}), dTP:{r2.X - cam2TeachingRotated.X:f2}, {r2.Y - cam2TeachingRotated.Y:f2}");

            Point deltaP = new Point(((r1.X - cam1TeachingRotated.X) + (r2.X - cam2TeachingRotated.X)) / 2.0
                , ((r1.Y - cam1TeachingRotated.Y) + (r2.Y - cam2TeachingRotated.Y)) / 2.0
                );

            AlignRCoord.X = deltaP.X;
            AlignRCoord.Y = deltaP.Y;

            return AlignRCoord;

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

        public static Point GetVirtualRotatedPoint(Point point, double dT)
        {
            double radT = dT * Math.PI / 180.0;
            return new Point(Math.Cos(radT) * point.X - Math.Sin(radT) * point.Y,
                Math.Sin(radT) * point.X + Math.Cos(radT) * point.Y
                );
        }
    }
}
