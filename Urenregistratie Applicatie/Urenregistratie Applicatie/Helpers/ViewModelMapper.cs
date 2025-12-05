using Urenregistratie_Applicatie.ViewModels;
using Urenregistratie_Applicatie.Models;

namespace Urenregistratie_Applicatie.Helpers
{
    public static class ViewModelMapper
    {
        public static GebruikerViewModel ToViewModel(Gebruiker gebruiker) => new()
        {
            Id = gebruiker.gebruiker_id.ToString(),
            FirstName = gebruiker.voornaam,
            LastName = gebruiker.achternaam,
            Email = gebruiker.email,
            Role = gebruiker.rol
        };
    }
}
