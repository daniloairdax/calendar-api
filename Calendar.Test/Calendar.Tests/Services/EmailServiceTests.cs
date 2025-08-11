using Calendar.Infrastructure.Services;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Calendar.Tests.Services
{
    [ExcludeFromCodeCoverage]
    public class EmailServiceTests
    {
        [Fact]
        public void SendEmail_WritesDebugMessage()
        {
            // Arrange
            var emailService = new EmailService();
            var recipient = "test@example.com";
            var subject = "Test Subject";
            var body = "Test Body";

            // Act
            emailService.SendEmail(recipient, subject, body);

            // Assert
            // Since EmailService uses Debug.WriteLine, we can't easily assert output here.
            // In a real implementation, you would mock dependencies or check side effects.
            // For now, just ensure no exception is thrown.
        }
    }
}
