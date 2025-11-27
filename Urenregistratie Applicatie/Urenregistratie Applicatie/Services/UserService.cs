using Urenregistratie_Applicatie.Models;

namespace Urenregistratie_Applicatie.Services;

public class UserService
{
    private readonly List<UserAccount> _seedUsers = new()
    {
        new UserAccount
        {
            Id = "USR-001",
            FirstName = "Lotte",
            LastName = "Jansen",
            Email = "lotte.jansen@company.nl",
            Role = "Administrator",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-1), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-002",
            FirstName = "Ruben",
            LastName = "Vermeulen",
            Email = "ruben.vermeulen@company.nl",
            Role = "Manager",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-6), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-003",
            FirstName = "Sara",
            LastName = "de Vries",
            Email = "sara.devries@company.nl",
            Role = "Medewerker",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-3), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-004",
            FirstName = "Finn",
            LastName = "Visser",
            Email = "finn.visser@company.nl",
            Role = "Medewerker",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-10), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-005",
            FirstName = "Nora",
            LastName = "Smit",
            Email = "nora.smit@company.nl",
            Role = "Administrator",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-4), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-006",
            FirstName = "Milan",
            LastName = "Vos",
            Email = "milan.vos@company.nl",
            Role = "Manager",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-30), DateTimeKind.Utc)
        }
    };

    public async Task<IReadOnlyList<UserAccount>> GetUsersAsync(bool simulateFailure = false)
    {
        await Task.Delay(250);

        if (simulateFailure)
        {
            throw new HttpRequestException("Gebruikersservice is niet beschikbaar.");
        }

        // Simuleer database query door een nieuwe lijst te retourneren
        return _seedUsers
            .Select(user => new UserAccount
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                LastLogin = user.LastLogin
            })
            .ToList();
    }
}
