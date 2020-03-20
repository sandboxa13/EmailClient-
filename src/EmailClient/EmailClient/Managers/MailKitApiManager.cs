using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;

namespace EmailClient.Managers
{
    public class MailKitApiManager : IMailKitApiManager
    {
        private ImapClient _imapClient;
        private SmtpClient _smtpClient;

        public Task AuthorizeAsync(string userName, string password, EmailService emailService = EmailService.Gmail)
        {
            return Task.Run(async () =>
           {
               var emailServiceInfo = EmailServiceProvider.GetEmailServiceInfo(emailService);

               _imapClient = new ImapClient { ServerCertificateValidationCallback = (s, c, h, e) => true };

               await _imapClient.ConnectAsync(emailServiceInfo.ImapHost, emailServiceInfo.ImapPort, true);

               await _imapClient.AuthenticateAsync(userName, password);
           });
        }

        public async Task<IEnumerable<IMessageSummary>> GetAllMessagesAsync()
        {
            await _imapClient.Inbox.OpenAsync(FolderAccess.ReadOnly);

            var messagesSummary = await _imapClient.Inbox
                .FetchAsync(Enumerable.Range(_imapClient.Inbox.Count - 100, _imapClient.Inbox.Count)
                .ToList(), MessageSummaryItems.All);

            return messagesSummary;
        }
    }

    public interface IMailKitApiManager
    {
        Task AuthorizeAsync(string userName, string password, EmailService emailService = EmailService.Gmail);

        Task<IEnumerable<IMessageSummary>> GetAllMessagesAsync();
    }
}
