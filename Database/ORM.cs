using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;

// <summary>
// The database structure in a class format using Entity Framework Core, this starts with the Db Contexts which are the connection
// between the database and the application, then the data models which are the tables in the database.
// Each DbContext represents a table in the database and each data model represents a row in that table.
// </summary>
namespace SeniorDesign.ORM
{
    // <summary>
    // Represents the player database connection.
    // </summary>
    public class Player : DbContext{
        public DbSet<PlayerData> Players { get; set; }

        public string DbPath { get; }

        public Player()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "players.db");
        }
    }

    // <summary>
    // Represents the session database connection.
    // </summary>
    public class Session : DbContext{
        public DbSet<SessionData> Sessions { get; set; }
        public string DbPath { get; }
        public Session()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "sessions.db");
        }
    }

    //<summary>
    // Represents the movement database connection.
    //</summary>
    public class Movement : DbContext{
        public DbSet<MovementData> Movements { get; set; }
        public string DbPath { get; }
        public Movement()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "movements.db");
        }
    }



    // <summary>
    //Below is the data models for the database, these are the tables in the database.
    // We start with the PlayerData model.
    // </summary>
    public class PlayerData{
        public int Player_ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public double Height { get; set; }
        public string Sex { get; set; }
    }

    // <summary>
    // Below is the SessionData model.  
    // </summary>
    public class SessionData{
        public int Session_ID { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double Length_s { get; set; }
        public DateTime Date { get; set; }
        public string Threshold_ID { get; set; }
        public string Player_ID { get; set; }
    }

    // <summary>
    // Below is the MovementData model.
    // </summary>
    public class MovementData
    {
        public int Motion_ID { get; set; }
        public double Length_s { get; set; }

        public string Session_ID { get; set; }
    }
}
