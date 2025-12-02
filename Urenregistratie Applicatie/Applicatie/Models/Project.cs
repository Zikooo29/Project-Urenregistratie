using System;
using SQLite;

namespace Urenregistratie_Applicatie.Models
{
    public class Project
    {
        [PrimaryKey, AutoIncrement]
        public int project_id { get; set; }

        public string projectnaam { get; set; } = string.Empty;
        public string beschrijving { get; set; } = string.Empty;
        public string startdatum { get; set; } = string.Empty;
        public string einddatum { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string aanmaakdatum { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public bool actief { get; set; } = true;

        [Indexed]
        public int bedrijf_id { get; set; }
    }
}
