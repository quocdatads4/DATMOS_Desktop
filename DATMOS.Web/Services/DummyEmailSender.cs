using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace DATMOS.Web.Services
{
    public class DummyEmailSender : IEmailSender
    {
        private readonly ILogger<DummyEmailSender> _logger;

        public DummyEmailSender(ILogger<DummyEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // In development, just log the email instead of actually sending it
            _logger.LogInformation("DummyEmailSender: Would send email to {Email} with subject '{Subject}'", email, subject);
            _logger.LogDebug("Email content: {HtmlMessage}", htmlMessage);
            
            // Return a completed task
            return Task.CompletedTask;
        }
    }
}
