using System;
using SQLite;

namespace Urenregistratie_Applicatie.Models
{
    public class GebruikerBedrijf
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [Indexed]
        public int gebruiker_id { get; set; }

        [Indexed]
        public int bedrijf_id { get; set; }

        public string rol { get; set; } = string.Empty;
    }
}
