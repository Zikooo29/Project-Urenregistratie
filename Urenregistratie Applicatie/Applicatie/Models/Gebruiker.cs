using System;
using SQLite;

namespace Urenregistratie_Applicatie.Models
{
    public class Gebruiker
    {
        [PrimaryKey, AutoIncrement]
        public int gebruiker_id { get; set; }

        public string voornaam { get; set; } = string.Empty;
        public string achternaam { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string wachtwoord_hash { get; set; } = string.Empty;
        public string rol { get; set; } = string.Empty;
        public string aanmaakdatum { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public bool actief { get; set; } = true;

        [Indexed]
        public int bedrijf_id { get; set; }
    }
}
