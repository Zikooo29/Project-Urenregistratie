using System;
using System.Collections.Generic;
using System.Linq;

namespace Urenregistratie_Applicatie.Core.Helpers
{
    public static class EmailHelper
    {
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            email = email.Trim();

            int atIndex = email.IndexOf('@');
            if (atIndex <= 0) return false;
            if (atIndex != email.LastIndexOf('@')) return false;

            string domain = email[(atIndex + 1)..];
            if (string.IsNullOrWhiteSpace(domain)) return false;

            int dotIndex = domain.IndexOf('.');
            if (dotIndex <= 0) return false;
            if (domain.EndsWith(".")) return false;

            if (email.Contains(" ")) return false;

            return true;
        }

        public static bool EmailExists(
            string? email,
            IEnumerable<(string Email, string UserId)> existingEmails,
            string? currentUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return existingEmails.Any(u =>
                u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase) &&
                u.UserId != currentUserId);
        }
    }
}
