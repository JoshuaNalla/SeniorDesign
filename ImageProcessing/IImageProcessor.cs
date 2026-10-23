using Emgu.CV;

namespace SeniorDesign.ImageProcessing
{
    internal interface IImageProcessor
    {
        /// <summary>
        /// Returns the speed of the traveling ball in pixels per frame.
        /// </summary>
        /// <param name="capture"></param>
        /// <returns></returns>
        double GetBallSpeed(VideoCapture capture);
    }
}
