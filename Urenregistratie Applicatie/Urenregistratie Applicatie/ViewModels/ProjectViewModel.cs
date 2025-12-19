using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urenregistratie_Applicatie.ViewModels
{
    public class ProjectViewModel
    {
        public string Id { get; init; } = string.Empty;
        public string Naam { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string Startdatum { get; init; } = string.Empty;
        public string Einddatum { get; init; } = string.Empty;
    }
}