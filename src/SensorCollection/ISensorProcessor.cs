using SeniorDesign.src.SensorCollection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign.src.SensorProcessing;

internal interface ISensorProcessor : IDisposable 
{
    public static readonly int CONNECTION_TIME = 20000;
    public static readonly int CONNECTION_ATTEMPTS = 3;
    public static abstract ISensorProcessor Instance { get; }
    public abstract Queue<SensorDataPacket> Packets { get; }

    /// <summary>
    /// Connect to all sensors within bluetooth range.
    /// </summary>
    /// <returns></returns>
    public Task<bool> ConnectNearbySensorsAsync();

    /// <summary>
    /// Begin live data streaming from sensors.
    /// </summary>
    /// <returns></returns>
    public Task<bool> BeginDataLivestreamAsync();

    /// <summary>
    /// End live data streaming from sensors
    /// </summary>
    /// <returns></returns>
    public Task<bool> EndDataLivestreamAsync();

    /// <summary>
    /// Disconnect from all connected sensors.
    /// </summary>
    /// <returns></returns>
    public Task<bool> DisconnectSensorsAsync();
}
