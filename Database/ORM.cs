using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;

// <summary>
// The database structure in a class format using Entity Framework Core, this starts with the Db Contexts which are the connection
// between the database and the application, then the data models which are the tables in the database.
// Each DbContext represents a table in the database and each data model represents a row in that table.
// </summary>
namespace SeniorDesign.ORM;

// <summary>
// Represents the player database connection.
// </summary>
public class Player : DbContext
{
    public DbSet<PlayerData> Players { get; set; }

    public string DbPath { get; }

    public Player()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Database.db");
    }
}

// <summary>
// Represents the session database connection.
// </summary>
public class Session : DbContext
{
    public DbSet<SessionData> Sessions { get; set; }
    public string DbPath { get; }
    public Session()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Database.db");
    }
}

//<summary>
// Represents the movement database connection.
//</summary>
public class Motion : DbContext
{
    public DbSet<MotionData> Motions { get; set; }
    public string DbPath { get; }
    public Motion()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Database.db");
    }
}

// <summary>
// Represents the threshold database connection.
// </summary>
public class Threshold : DbContext
{
    public DbSet<ThresholdData> Thresholds { get; set; }
    public string DbPath { get; }
    public Threshold()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Database.db");
    }
}

// <summary>
// Represents the sensor file database connection.
// </summary>
public class SensorFile : DbContext
{
    public DbSet<SensorFileData> SensorFiles { get; set; }
    public string DbPath { get; }
    public SensorFile()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Database.db");
    }
}

// <summary>
// Represents the video file database connection.
// </summary>
public class VideoFile : DbContext
{
    public DbSet<VideoFileData> VideoFiles { get; set; }
    public string DbPath { get; }
    public VideoFile()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Database.db");
    }
}

// <summary>
// Represents the settings database connection.
// </summary>
public class Settings : DbContext
{
    public DbSet<SettingsData> UserSettings { get; set; }
    public string DbPath { get; }
    public Settings()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "Database.db");
    }
}



// <summary>
//Below is the data models for the database, these are the tables in the database.
// We start with the PlayerData model.
// </summary>
public class PlayerData
{
    public int Player_ID { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public double Height { get; set; }
    public string Sex { get; set; }
    public SettingsData Settings_ID { get; set; }
}

// <summary>
// Below is the SessionData model.  
// </summary>
public class SessionData
{
    public int Session_ID { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public double Duration_s { get; set; }
    public DateTime Date { get; set; }
    public ThresholdData Threshold_ID { get; set; }
    public PlayerData Player_ID { get; set; }
}

// <summary>
// Below is the MovementData model.
// </summary>
public class MotionData
{
    public int Motion_ID { get; set; }
    public double IncomingBallSpeed_m_s { get; set; }
    public double OutgoingBallSpeed_m_s { get; set; }
    public SessionData Session_ID { get; set; }
}

// <summary>
// Below is the ThresholdData model.
// </summary>
public class ThresholdData
{
    public int Threshold_ID { get; set; }
    public double XPositionCap { get; set; }
    public double YPositionCap { get; set; }
    public double ZPositionCap { get; set; }
    public double XVelocityCap { get; set; }
    public double YVelocityCap { get; set; }
    public double ZVelocityCap { get; set; }
    public double XAccelerationCap { get; set; }
    public double YAccelerationCap { get; set; }
    public double ZAccelerationCap { get; set; }
}

// <summary>
// Below is the SensorFileData model.
// </summary>
public class SensorFileData
{
    public int File_ID { get; set; }
    public string FilePath { get; set; }
    public double Size_KB { get; set; }
    public string FileType { get; set; }
    public MotionData Motion_ID { get; set; }
}

// <summary>
// Below is the VideoFileData model.
// </summary>
public class VideoFileData
{
    public int File_ID { get; set; }
    public double Length_s { get; set; }
    public int resolutionWidth { get; set; }
    public int resolutionHeight { get; set; }
    public double FrameRate { get; set; }
    public string FilePath { get; set; }
    public double Size_KB { get; set; }
    public string FileType { get; set; }
    public MotionData Motion_ID { get; set; }
}

// <summary>
// Below is the SettingsData model.
// </summary>
public class SettingsData
{
    public int Setting_ID { get; set; }
    public string BeepStyle { get; set; }
    public string UnitSystem { get; set; }
}