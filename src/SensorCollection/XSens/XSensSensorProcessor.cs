using MovellaDotPcSdk;
using SeniorDesign.src.SensorCollection.XSens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign.src.SensorProcessing.XSens
{
    internal partial class XSensSensorProcessor : ISensorProcessor
    {
        private XsDotConnectionManager Manager { get; set; } = new XsDotConnectionManager();
        private static XSensSensorProcessor _instance;
        public static ISensorProcessor Instance
        {
            get { return _instance ??= new XSensSensorProcessor(); }
        }

        private XSensSensorProcessor()
        {

        }

        public void ConnectNearbySensors()
        {
            Manager.enableDeviceDetection();

            do
            {
                // for each sensor in the sensor list, connect it
                break;
            } while (true);

            Manager.disableDeviceDetection();
            throw new NotImplementedException();
        }

        public void BeginDataLivestream()
        {
            throw new NotImplementedException();
        }

        public void DisconnectSensors()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Manager.close();
        }
    }
}
