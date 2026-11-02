using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign.src.SensorProcessing
{
    internal interface ISensorProcessor
    {
        /// <summary>
        /// Connect to all sensors within bluetooth range.
        /// </summary>
        public void ConnectNearbySensors();

        /// <summary>
        /// Begin live data streaming from sensors.
        /// </summary>
        public void BeginDataLivestream();

        /// <summary>
        /// Disconnect from all connected sensors.
        /// </summary>
        public void DisconnectSensors();
    }
}
