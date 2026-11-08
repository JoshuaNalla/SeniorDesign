using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign.src.SensorProcessing;

internal interface ISensorProcessor : IDisposable 
{
    public static abstract ISensorProcessor Instance { get; }

    /// <summary>
    /// Connect to all sensors within bluetooth range.
    /// </summary>
    public Task<bool> ConnectNearbySensorsAsync();

    /// <summary>
    /// Begin live data streaming from sensors.
    /// </summary>
    public Task<bool> BeginDataLivestreamAsync();

    /// <summary>
    /// Disconnect from all connected sensors.
    /// </summary>
    public Task<bool> DisconnectSensorsAsync();
}
