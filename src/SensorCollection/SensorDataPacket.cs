using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign.src.SensorCollection
{
    internal class SensorDataPacket
    {
        public string SensorID { get; set; }
        public DateTime? SampleTime { get; set; }
        public DateTime? ArrivalTime { get; set; }


    }
}
