using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string datum { get; set; }
        public double aantal_uren { get; set; }

        public string omschrijving { get; set; }
        public string status { get; set; }

        public string aanmaakdatum { get; set; }
        public string laatst_gewijzigd { get; set; }

        // FK naar gebruiker (goedkeurder) — nullable!
        [Indexed]
        public int? goedgekeurd_door_id { get; set; }

        public string goedkeuringsdatum { get; set; }
        public string goedkeuring_toelichting { get; set; }
    }
}
