using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Urenregistratie_Applicatie.Models
{
    public class Bedrijf
    {
        [PrimaryKey, AutoIncrement]
        public int bedrijf_id { get; set; }

        public string bedrijfsnaam { get; set; }
        public string contactpersoon { get; set; }
        public string email { get; set; }
        public string telefoonnummer { get; set; }
        public string adres { get; set; }

        public string type { get; set; }

        public string aanmaakdatum { get; set; }
        public bool actief { get; set; }
    }
}
