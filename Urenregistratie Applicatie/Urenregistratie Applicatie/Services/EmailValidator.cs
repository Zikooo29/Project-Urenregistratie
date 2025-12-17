namespace Urenregistratie_Applicatie.Helpers;

/// <summary>
/// Helper class voor email validatie
/// </summary>
public static class EmailValidator
{
    /// <summary>
    /// Controleert of een email-adres geldig is
    /// </summary>
    /// <param name="email">Het te valideren email-adres</param>
    /// <returns>True als het email-adres geldig is, anders false</returns>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return email.Contains("@") && email.Contains(".");
    }

    /// <summary>
    /// Controleert of een email-adres al bestaat in een lijst van gebruikers
    /// </summary>
    /// <param name="email">Het te controleren email-adres</param>
    /// <param name="existingEmails">Bestaande email-adressen</param>
    /// <param name="currentUserId">Optioneel: het ID van de huidige gebruiker (bij wijzigen)</param>
    /// <returns>True als het email-adres al bestaat (en niet van de huidige gebruiker is)</returns>
    public static bool EmailExists(string email, IEnumerable<(string Email, string UserId)> existingEmails, string? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return existingEmails.Any(u =>
            u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase) &&
            u.UserId != currentUserId);
    }
}