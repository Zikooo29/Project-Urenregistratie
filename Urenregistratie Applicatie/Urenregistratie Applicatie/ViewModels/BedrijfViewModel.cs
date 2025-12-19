using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urenregistratie_Applicatie.ViewModels
{
    public class BedrijfViewModel
    {
        public string Id { get; init; } = string.Empty;

        public string Naam { get; init; } = string.Empty;

        public string Contactpersoon { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Telefoonnummer { get; init; } = string.Empty;
    }
}