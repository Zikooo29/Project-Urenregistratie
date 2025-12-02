using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urenregistratie_Applicatie.Models;

namespace Urenregistratie_Applicatie.Services;

public class UserService
{
    // Voorbeeld gebruikersgegevens (mock data)
    private readonly List<UserAccount> _seedUsers = new()
    {
        new UserAccount
        {
            Id = "#4",
            FirstName = "Naam1",
            LastName = "Achternaam1",
            Email = "test1@hotmail.nl",
            Role = "Rol",
        },
        new UserAccount
        {
            Id = "#2",
            FirstName = "Naam2",
            LastName = "Achternaam2",
            Email = "test2@hotmail.nl",
            Role = "Rol",
        },
        new UserAccount
        {
            Id = "#3",
            FirstName = "Naam3",
            LastName = "Achternaam3",
            Email = "test3@hotmail.nl",
            Role = "Rol",
        },
    };

    // Haalt testgebruikers op uit een lokale mock lijst en doet alsof het van een server komt door een korte vertraging toe te voegen.
    public async Task<IReadOnlyList<UserAccount>> GetUsersAsync(bool simulateFailure = false)
    {
        await Task.Delay(250);

        return _seedUsers
            .Select(user => new UserAccount
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
            })
            .ToList();
    }
    public Task DeleteUserAsync(string id)
    {
        var user = _seedUsers.FirstOrDefault(u => u.Id == id);
        if (user is not null)
        {
            _seedUsers.Remove(user);  // verwijdert uit de mock-lijst
        }

        
        // Urenregistraties worden dan NIET gecascade-deleted (alleen user zelf).
        return Task.CompletedTask;
    }
}

