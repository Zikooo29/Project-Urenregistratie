using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Urenregistratie_Applicatie.Models
{

    public class Gebruiker
    {
        [PrimaryKey, AutoIncrement]
        public int gebruiker_id { get; set; }

        public string voornaam { get; set; }
        public string achternaam { get; set; }
        public string email { get; set; }
        public string wachtwoord_hash { get; set; }
        public string rol { get; set; }
        public string aanmaakdatum { get; set; }
        public bool actief { get; set; }

        [Indexed]
        public int bedrijf_id { get; set; }
    }
}