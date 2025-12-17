using System.Collections.Generic;
using Xunit;
using Urenregistratie_Applicatie.Core.Helpers;

namespace Urenregistratie_Applicatie.Tests.Helpers
{
    public class EmailHelperTests
    {
        #region IsValidEmail Tests

        [Fact]
        public void IsValidEmail_ValidEmail_ReturnsTrue()
        {
            // Arrange
            string validEmail = "gebruiker@example.com";

            // Act
            bool result = EmailHelper.IsValidEmail(validEmail);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidEmail_ValidEmailWithSubdomain_ReturnsTrue()
        {
            // Arrange
            string validEmail = "test.user@mail.example.com";

            // Act
            bool result = EmailHelper.IsValidEmail(validEmail);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidEmail_EmailWithoutAtSymbol_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "gebruikerexample.com";

            // Act
            bool result = EmailHelper.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidEmail_EmailWithoutDot_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "gebruiker@examplecom";

            // Act
            bool result = EmailHelper.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidEmail_EmailWithoutAtAndDot_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "gebruikerexamplecom";

            // Act
            bool result = EmailHelper.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidEmail_EmptyString_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "";

            // Act
            bool result = EmailHelper.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidEmail_NullString_ReturnsFalse()
        {
            // Arrange
            string? invalidEmail = null;

            // Act
            bool result = EmailHelper.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidEmail_WhitespaceOnly_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "   ";

            // Act
            bool result = EmailHelper.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test.user@example.com")]
        [InlineData("user+tag@example.co.uk")]
        [InlineData("user_name@example.com")]
        [InlineData("123@example.com")]
        public void IsValidEmail_ValidEmails_ReturnsTrue(string email)
        {
            // Act
            bool result = EmailHelper.IsValidEmail(email);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("userexample.com")]      // Geen @
        [InlineData("user@examplecom")]      // Geen .
        [InlineData("@example.com")]         // Geen lokaal deel
        [InlineData("user@")]                // Geen domein
        [InlineData("user")]                 // Geen @ en .
        [InlineData("")]                     // Leeg
        [InlineData(null)]                   // Null
        [InlineData("   ")]                  // Alleen whitespace
        public void IsValidEmail_InvalidEmails_ReturnsFalse(string? email)
        {
            // Act
            bool result = EmailHelper.IsValidEmail(email);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region EmailExists Tests

        [Fact]
        public void EmailExists_EmailDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var existingEmails = new List<(string Email, string UserId)>
            {
                ("jan@example.com", "1"),
                ("piet@example.com", "2")
            };

            // Act
            bool result = EmailHelper.EmailExists("nieuw@example.com", existingEmails);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EmailExists_EmailExists_ReturnsTrue()
        {
            // Arrange
            var existingEmails = new List<(string Email, string UserId)>
            {
                ("jan@example.com", "1"),
                ("piet@example.com", "2")
            };

            // Act
            bool result = EmailHelper.EmailExists("jan@example.com", existingEmails);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EmailExists_EmailExistsCaseInsensitive_ReturnsTrue()
        {
            // Arrange
            var existingEmails = new List<(string Email, string UserId)>
            {
                ("jan@example.com", "1"),
                ("piet@example.com", "2")
            };

            // Act
            bool result = EmailHelper.EmailExists("JAN@EXAMPLE.COM", existingEmails);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EmailExists_SameEmailButCurrentUser_ReturnsFalse()
        {
            // Arrange
            var existingEmails = new List<(string Email, string UserId)>
            {
                ("jan@example.com", "1"),
                ("piet@example.com", "2")
            };

            // Act - gebruiker met ID "1" wijzigt zijn eigen email (blijft hetzelfde)
            bool result = EmailHelper.EmailExists("jan@example.com", existingEmails, currentUserId: "1");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EmailExists_EmptyEmail_ReturnsFalse()
        {
            // Arrange
            var existingEmails = new List<(string Email, string UserId)>
            {
                ("jan@example.com", "1")
            };

            // Act
            bool result = EmailHelper.EmailExists("", existingEmails);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EmailExists_NullEmail_ReturnsFalse()
        {
            // Arrange
            var existingEmails = new List<(string Email, string UserId)>
            {
                ("jan@example.com", "1")
            };

            // Act
            bool result = EmailHelper.EmailExists(null, existingEmails);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
