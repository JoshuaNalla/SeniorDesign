using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.ImgHash;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace SeniorDesign.ImageProcessing
{
    internal class ImageProcessor : IImageProcessor
    {
        private static readonly double MIN_BALL_AREA = 0.0;
        private static readonly double MIN_BALL_PERIMETER = 0.0;
        private static readonly double ROUNDNESS_SENSITIVITY = 0.0;
        private static readonly BackgroundSubtractorMOG2 bgsub = new();
        public static int counter = 0;

        /// <summary>
        /// Processes a video clip into the average speed of a ball moving in the video.
        /// </summary>
        /// <param name="capture"></param>
        /// <returns></returns>
        public double GetBallSpeed(VideoCapture capture)
        {
            Utils.ThrowIfNull(capture);

            ClearImages();

            Mat frame = new();
            List<System.Drawing.Point> points = [];
            while (capture.Read(frame) && !frame.IsEmpty)
            {
                frame = ProcessFrame(frame);
                VectorOfPoint contour = FindCircularContour(frame);
                points.Add(FindContourPosition(contour));
            }

            double totalDistance = 0.0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                totalDistance += GetDistance(points[i], points[i + 1]);
            }

            double velocity = totalDistance / (points.Count - 1);
            return totalDistance / (points.Count - 1);
        }

        /// <summary>
        /// Processes a frame into a binary image by subtracting the background.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private Mat ProcessFrame(Mat frame)
        {
            Utils.ThrowIfNull(frame);

            CvInvoke.CvtColor(frame, frame, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            SaveImage(frame, $"grayscale_{counter++}");
            CvInvoke.GaussianBlur(frame, frame, new System.Drawing.Size(3, 3), 4);
            SaveImage(frame, $"blur_{counter++}");
            bgsub.Apply(frame, frame);
            SaveImage(frame, $"bgsub_{counter++}");
            return frame;
        }

        /// <summary>
        /// Takes a binary image and returns the most circular contour.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private VectorOfPoint FindCircularContour(Mat frame)
        {
            Utils.ThrowIfNull(frame);

            VectorOfPoint returnContour = new();
            double returnContourRoundness = 0.0;

            VectorOfVectorOfPoint contours = new();
            CvInvoke.FindContours(frame, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; i++)
            {
                VectorOfPoint contour = contours[i];

                double area = CvInvoke.ContourArea(contour);
                if (area < MIN_BALL_AREA) continue;

                double perimeter = CvInvoke.ArcLength(contour, true);
                if (perimeter < MIN_BALL_PERIMETER) continue;

                double roundness = (4 * Math.PI * area) / (perimeter * perimeter);
                if (roundness <= ROUNDNESS_SENSITIVITY || roundness <= returnContourRoundness) continue;

                returnContour = contour;
                returnContourRoundness = roundness;
            }

            Mat printFrame = Mat.Zeros(frame.Rows, frame.Cols, DepthType.Cv8U, 1);
            CvInvoke.DrawContours(printFrame, returnContour, -1, new MCvScalar(255), 2);
            SaveImage(printFrame, $"contour_{counter++}");

            return returnContour;
        }

        /// <summary>
        /// Finds the center of a contour.
        /// </summary>
        /// <param name="contour"></param>
        /// <returns></returns>
        private System.Drawing.Point FindContourPosition(VectorOfPoint contour)
        {
            Utils.ThrowIfNull(contour);

            System.Drawing.Point contourPosition = new System.Drawing.Point();
            Moments moments = CvInvoke.Moments(contour, true);
            if (moments.M00 != 0)
            {
                contourPosition.X = (int)(moments.M10 / moments.M00);
                contourPosition.Y = (int)(moments.M01 / moments.M00);
            }
            Console.WriteLine($"X: {contourPosition.X}, Y: {contourPosition.Y}");
            return contourPosition;
        }

        /// <summary>
        /// Gets the distance between two points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private double GetDistance(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            Utils.ThrowIfNull([p1, p2]);

            double diff1 = p1.X - p2.X;
            double diff2 = p1.Y - p2.Y;
            return Math.Sqrt(diff1 * diff1 + diff2 * diff2);
        }

        //Temp function for debug
        private void SaveImage(Mat img, string imgname)
        {
            if (!img.IsEmpty && !string.IsNullOrEmpty(imgname))
                img.Save($"C:\\Users\\mitch\\source\\repos\\SeniorDesign\\DataAndDebug\\Photos\\{imgname}.png");
        }

        //Temp function for debug
        private void ClearImages()
        {
            DirectoryInfo di = new DirectoryInfo($"C:\\Users\\mitch\\source\\repos\\SeniorDesign\\DataAndDebug\\Photos");

            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
        }
    }
}
