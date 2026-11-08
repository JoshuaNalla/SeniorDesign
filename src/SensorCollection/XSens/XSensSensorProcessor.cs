using MovellaDotPcSdk;
using System.Runtime.InteropServices;

namespace SeniorDesign.src.SensorProcessing.XSens
{
    internal partial class XSensSensorProcessor : XsDotCallback, ISensorProcessor
    {
        private static XSensSensorProcessor _instance;
        private XsDotConnectionManager Manager { get; }
        private List<XsDotDevice> ConnectedBluetoothSensors { get; }
        public static ISensorProcessor Instance
        {
            get { return _instance ??= new XSensSensorProcessor(); }
        }

        private XSensSensorProcessor()
        {
            Manager = new XsDotConnectionManager();
            ConnectedBluetoothSensors = new List<XsDotDevice>();
            Manager.addXsDotCallbackHandler(this);
        }

        public async Task<bool> ConnectNearbySensorsAsync()
        {
            //Detect one nearby Bluetooth sensor. Dies after 20 seconds.
            //This needs to be updated to detect all sensors, not super difficult.
            Manager.enableDeviceDetection();
            int sensorCount = 0;
            long starttime = XsTimeStamp.nowMs();
            do
            {
                if (sensorCount != DetectedBluetoothSensors.Count)
                {
                    sensorCount = DetectedBluetoothSensors.Count;
                    break;
                }
            } while (XsTimeStamp.nowMs() - starttime < 20000);
            Manager.disableDeviceDetection();

            //Connect to all detected sensors.
            XsDotDevice device;
            foreach (XsPortInfo sensor in DetectedBluetoothSensors)
            {
                if (!Manager.openPort(sensor)) continue;        //This often fails. Need to figure out why
                device = Manager.device(sensor.deviceId());
                if (device == null) continue;
                ConnectedBluetoothSensors.Add(device);
            }

            return ConnectedBluetoothSensors.Any();
        }

        public async Task<bool> BeginDataLivestreamAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DisconnectSensorsAsync()
        {
            //This should be replaced with actual disconnect logic, but I couldn't find it
            foreach (XsDotDevice device in ConnectedBluetoothSensors)
            {
                device.powerOff();
            }
            ConnectedBluetoothSensors.Clear();
            return true;
        }

        #region [ button ]
        private readonly Lock _buttonLock = new Lock();
        public bool IsNewButton { get; private set; }
        public uint ButtonTimestamp
        {
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
                lock (_buttonLock)
                {
                    IsNewButton = true;
                    field = value;
                }
            }
        }

        /// <summary>
        /// Callback event for when a sensor button is clicked.
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="timestamp"></param>
        protected override void onButtonClicked(XsDotDevice dev, uint timestamp)
        {
            ButtonTimestamp = timestamp;
        }
        #endregion

        #region [ device state ]

        protected override void onDeviceStateChanged(XsDotDevice dev, XsDeviceState newState, XsDeviceState oldState)
        {
            if (newState == XsDeviceState.XDS_Destructing)
                ConnectedBluetoothSensors.Remove(dev);
        }

        #endregion

        #region [ packets ]
        private readonly Lock _packetLock = new Lock();
        private Queue<XsDataPacket> Packets = new Queue<XsDataPacket>();
        public XsDataPacket? NextPacket
        {
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
        private List<XsPortInfo> DetectedBluetoothSensors { get; } = new List<XsPortInfo>();

        protected override void onAdvertisementFound(XsPortInfo portInfo)
        {
            Utils.ErrorIfNull(portInfo);
            if (portInfo.isBluetooth())
                DetectedBluetoothSensors.Add(portInfo);
        }
        #endregion
    }
}
