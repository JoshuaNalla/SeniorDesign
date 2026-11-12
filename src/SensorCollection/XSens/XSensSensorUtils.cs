using MovellaDotPcSdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign.src.SensorCollection.XSens
{
    internal class XSensSensorUtils
    {
        public static SensorDataPacket TransmutePacket(XsDataPacket packet)
        {
            return new SensorDataPacket
            {
                SensorID = packet.deviceId().ToString(),
                SampleTime = XSensSensorUtils.DateTimeFromTimeInfo(packet.estimatedTimeOfSampling().toTimeInfo()),
                ArrivalTime = XSensSensorUtils.DateTimeFromTimeInfo(packet.timeOfArrival().toTimeInfo())
            };
        }

        private static DateTime DateTimeFromTimeInfo(XsTimeInfo timeinfo)
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
