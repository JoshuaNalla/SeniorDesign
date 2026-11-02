using MovellaDotPcSdk;
using SeniorDesign.src.SensorProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign.src.SensorCollection.XSens
{
    internal partial class XSensSensorProcessor : XsDotCallback
    {
        #region [ button ]
        private readonly Lock _buttonLock = new Lock();
        public bool IsNewButton { get; private set; }
        public uint ButtonTimestamp { 
            get
            {
                lock (_buttonLock)
                {
                    IsNewButton = false;
                    return field;
                }
            }
            private set
            {
                IsNewButton = true;
                field = value;
            }
        }

        /// <summary>
        /// Callback event for when a sensor button is clicked.
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="timestamp"></param>
        protected override void onButtonClicked(XsDotDevice dev, uint timestamp)
        {
            lock (_buttonLock)
            {
                ButtonTimestamp = timestamp;
            }
        }
        #endregion

        #region [ packets ]
        private readonly Lock _packetLock = new Lock();
        private Queue<XsDataPacket> Packets = new Queue<XsDataPacket>();
        public XsDataPacket? NextPacket { 
            get
            {
                lock (_packetLock)
                {
                    return Packets.TryDequeue(out XsDataPacket? packet) ? packet : null;
                }
            }
        }

        /// <summary>
        /// Callback event for when new live data is available from a sensor.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="packet"></param>
        protected override void onLiveDataAvailable(XsDotDevice device, XsDataPacket packet)
        {
            lock (_packetLock)
            {
                Packets.Enqueue(packet);
            }
        }
        #endregion

        #region [ advertisement ]
        private readonly Lock _advertisementLock = new Lock();
        public List<XsPortInfo> ConnectedSensors { 
            get
            {
                lock (_advertisementLock)
                {
                    return field;
                }
            }
        } = new List<XsPortInfo>();

        protected override void onAdvertisementFound(XsPortInfo portInfo)
        {
            lock (_advertisementLock)
            {
                ConnectedSensors.Add(portInfo);
            }
        }
        #endregion
    }
}
