using System;
using SQLite;

namespace Urenregistratie_Applicatie.Models
{
    public class Urenregistratie
    {
        [PrimaryKey, AutoIncrement]
        public int registratie_id { get; set; }

        [Indexed]
        public int gebruiker_id { get; set; }

        [Indexed]
        public int project_id { get; set; }

        public string datum { get; set; } = string.Empty;
        public double aantal_uren { get; set; } = 0.0;

        public string starttijd { get; set; } = string.Empty;

        public string eindtijd { get; set; } = string.Empty;

        public string omschrijving { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;

        public string aanmaakdatum { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public string laatst_gewijzigd { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Indexed]
        public int? goedgekeurd_door_id { get; set; }

        public string goedkeuringsdatum { get; set; } = string.Empty;
        public string goedkeuring_toelichting { get; set; } = string.Empty;
    }
}
