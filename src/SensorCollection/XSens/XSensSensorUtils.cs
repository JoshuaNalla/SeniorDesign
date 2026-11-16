using MovellaDotPcSdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign.src.SensorCollection.XSens
{
    internal class XSensSensorUtils
    {
        public static SensorDataPacket TransmutePacket(XsDataPacket? packet)
        {
            XsEuler? orientation = packet?.orientationEuler();
            XsVector? acceleration = packet?.freeAcceleration();
            Utils.ErrorIfNull(packet, orientation, acceleration);

            return new SensorDataPacket
            {
                SensorID = packet.deviceId()?.ToString() ?? string.Empty,
                SampleTime = XSensSensorUtils.DateTimeFromSensorTime(packet.estimatedTimeOfSampling().toTimeInfo()),
                ArrivalTime = XSensSensorUtils.DateTimeFromSensorTime(packet.timeOfArrival().toTimeInfo()),
                Roll = orientation.roll(),
                Pitch = orientation.pitch(),
                Yaw = orientation.yaw(),
                AccelerationX = acceleration.value(0),
                AccelerationY = acceleration.value(1),
                AccelerationZ = acceleration.value(2)
            };
        }

        private static DateTime DateTimeFromSensorTime(XsTimeInfo timeinfo)
        {
            DateTime time = new DateTime(
                year: timeinfo.m_year,
                month: timeinfo.m_month,
                day: timeinfo.m_day,
                hour: timeinfo.m_hour,
                minute: timeinfo.m_minute,
                second: timeinfo.m_second,
                kind: DateTimeKind.Utc
                );

            long ticks = timeinfo.m_nano / 100;
            return time.AddTicks( ticks );
        }
    }
}
