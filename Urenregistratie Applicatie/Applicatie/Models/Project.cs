using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Urenregistratie_Applicatie.Models
{
    public class Project
    {
        [PrimaryKey, AutoIncrement]
        public int project_id { get; set; }

        public string projectnaam { get; set; }
        public string beschrijving { get; set; }

        public string startdatum { get; set; }
        public string einddatum { get; set; }

        public string status { get; set; }
        public string aanmaakdatum { get; set; }
        public bool actief { get; set; }


        [Indexed]
        public int bedrijf_id { get; set; }
    }
}
