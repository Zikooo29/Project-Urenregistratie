using Urenregistratie_Applicatie.ViewModels;
using Urenregistratie_Applicatie.Models;

namespace Urenregistratie_Applicatie.Helpers
{
    public static class ViewModelMapper
    {
        // Gebruiker → GebruikerViewModel
        public static GebruikerViewModel ToViewModel(Gebruiker gebruiker) => new()
        {
            Id = $"#{gebruiker.gebruiker_id}",
            FirstName = gebruiker.voornaam,
            LastName = gebruiker.achternaam,
            Email = gebruiker.email,
            Role = gebruiker.rol
        };

        // Project → ProjectViewModel
        public static ProjectViewModel ToViewModel(Project project) => new()
        {
            Id = $"#{project.project_id}",
            Naam = project.projectnaam,
            Status = project.status,
            Startdatum = project.startdatum,
            Einddatum = project.einddatum
        };

        // Bedrijf → BedrijfViewModel
        public static BedrijfViewModel ToViewModel(Bedrijf bedrijf) => new()
        {
            Id = $"#{bedrijf.bedrijf_id}",
            Naam = bedrijf.bedrijfsnaam,
            Contactpersoon = bedrijf.contactpersoon,
            Email = bedrijf.email,
            Telefoonnummer = bedrijf.telefoonnummer
        };
    }
}