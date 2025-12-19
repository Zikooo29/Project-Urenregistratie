using System;
using SQLite;

namespace Urenregistratie_Applicatie.Models
{
    public class ProjectMedewerker
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [Indexed]
        public int gebruiker_id { get; set; }

        [Indexed]
        public int project_id { get; set; }

        public string rol { get; set; } = string.Empty;
    }
}
