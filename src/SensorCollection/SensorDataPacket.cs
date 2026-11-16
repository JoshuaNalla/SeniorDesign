using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign.src.SensorCollection
{
    internal class SensorDataPacket
    {
        public string SensorID { get; set; } = string.Empty;
        public DateTime? SampleTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public double Roll { get; set; }
        public double Pitch { get; set; }
        public double Yaw { get; set; }
        public double AccelerationX { get; set; }
        public double AccelerationY { get; set; }
        public double AccelerationZ { get; set; }

        //Expand as necessary
    }
}
