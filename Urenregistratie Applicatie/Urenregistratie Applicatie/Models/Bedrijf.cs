using System;
using SQLite;

namespace Urenregistratie_Applicatie.Models
{
    public class Bedrijf
    {
        [PrimaryKey, AutoIncrement]
        public int bedrijf_id { get; set; }

        public string bedrijfsnaam { get; set; } = string.Empty;
        public string contactpersoon { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string telefoonnummer { get; set; } = string.Empty;
        public string adres { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;

        public string aanmaakdatum { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public bool actief { get; set; } = true;
    }
}
