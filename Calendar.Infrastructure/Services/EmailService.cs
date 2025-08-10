using System.Diagnostics;

namespace Calendar.Infrastructure.Services
{
    public interface IEmailService
    {
        void SendEmail(string recipientEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        public void SendEmail(string recipientEmail, string subject, string body)
        {
            // Replace with real email service implementation
            Debug.WriteLine($"Email sent to {recipientEmail}\nSubject: {subject}\nBody: {body}");
        }
    }
}
