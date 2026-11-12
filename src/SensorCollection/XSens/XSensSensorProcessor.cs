using MovellaDotPcSdk;
using SeniorDesign.src.SensorCollection;
using SeniorDesign.src.SensorCollection.XSens;
using System.Diagnostics;

namespace SeniorDesign.src.SensorProcessing.XSens
{
    internal partial class XSensSensorProcessor : XsDotCallback, ISensorProcessor
    {
        private static XSensSensorProcessor _instance;
        private XsDotConnectionManager Manager { get; }
        private List<XsDotDevice> ConnectedBluetoothSensors { get; }

        public Queue<SensorDataPacket> Packets { get; }

        public static ISensorProcessor Instance
        {
            get { return _instance ??= new XSensSensorProcessor(); }
        }

        #region [ constructor ]

        private XSensSensorProcessor()
        {
            Manager = new XsDotConnectionManager();
            Manager.addXsDotCallbackHandler(this);

            ConnectedBluetoothSensors = [];
            Packets = [];
        }

        #endregion

        #region [ interface methods ]

        public async Task<bool> ConnectNearbySensorsAsync()
        {
            //Detect available sensors.
            Manager.enableDeviceDetection();
            Thread.Sleep(ISensorProcessor.CONNECTION_TIME);
            Manager.disableDeviceDetection();

            //Connect to all detected sensors.
            foreach (XsPortInfo sensor in DetectedBluetoothSensors)
            {
                bool isConnected = false;
                for (int attempt = 0; attempt < ISensorProcessor.CONNECTION_ATTEMPTS; attempt++)
                {
                    isConnected = Manager.openPort(sensor);
                    if (isConnected) break;
                    Thread.Sleep(200);
                }
                if (!isConnected) continue;

                XsDotDevice device = Manager.device(sensor.deviceId());
                if (device == null) continue;

                ConnectedBluetoothSensors.Add(device);
            }

            //Synchronize all sensors.
            XsDotDevice root = ConnectedBluetoothSensors.FirstOrDefault();
            foreach (XsDotDevice device in ConnectedBluetoothSensors)
            {
                device.startSync(root.bluetoothAddress());
            }

            return ConnectedBluetoothSensors.Any();
        }

        public async Task<bool> BeginDataLivestreamAsync()
        {
            bool result = true;
            foreach (XsDotDevice device in ConnectedBluetoothSensors)
            {
                result = result && device.startMeasurement(XsPayloadMode.HighFidelity);
            }
            return result;
        }

        public async Task<bool> EndDataLivestreamAsync()
        {
            bool result = true;
            foreach (XsDotDevice device in ConnectedBluetoothSensors)
            {
                result = result && device.stopMeasurement();
            }
            return result;
        }

        public async Task<bool> DisconnectSensorsAsync()
        {
            //This not sure if there is actual disconnection logic but I couldn't find it so this works for now
            bool result = true;
            foreach (XsDotDevice device in ConnectedBluetoothSensors)
            {
                result = result && device.powerOff();
            }
            ConnectedBluetoothSensors.Clear();
            return result;
        }

        #endregion

        #region [ button ]
        private readonly Lock _buttonLock = new Lock();
        public bool IsNewButton { get; private set; }
        public int ButtonTimestamp
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
            Utils.ErrorIfNull(dev, timestamp);
            ButtonTimestamp = (int)timestamp;
        }
        #endregion

        #region [ device state ]

        protected override void onDeviceStateChanged(XsDotDevice dev, XsDeviceState newState, XsDeviceState oldState)
        {
            Utils.ErrorIfNull(dev, newState, oldState);
            if (newState == XsDeviceState.XDS_Destructing)
                ConnectedBluetoothSensors.Remove(dev);
        }

        #endregion

        #region [ packets ]
        private readonly Lock _packetLock = new Lock();

        /// <summary>
        /// Callback event for when new live data is available from a sensor.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="packet"></param>
        protected override void onLiveDataAvailable(XsDotDevice device, XsDataPacket packet)
        {
            Utils.ErrorIfNull(device, packet);
            lock (_packetLock)
            {
                Packets.Enqueue(XSensSensorUtils.TransmutePacket(packet));
            }
        }
        #endregion

        #region [ advertisement ]
        private List<XsPortInfo> DetectedBluetoothSensors { get; } = [];

        protected override void onAdvertisementFound(XsPortInfo portInfo)
        {
            Utils.ErrorIfNull(portInfo);
            if (portInfo.isBluetooth())
                DetectedBluetoothSensors.Add(portInfo);
        }
        #endregion
    }
}
