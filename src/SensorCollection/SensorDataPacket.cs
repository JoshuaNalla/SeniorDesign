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


    }
}
