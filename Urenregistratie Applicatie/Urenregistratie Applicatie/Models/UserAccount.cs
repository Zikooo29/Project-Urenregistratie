namespace Urenregistratie_Applicatie.Models;

public class UserAccount
{
    public string Id { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public DateTime LastLogin { get; init; }
        = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

    public string FullName => $"{FirstName} {LastName}".Trim();
}
