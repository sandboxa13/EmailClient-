using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MimeKit;

namespace EmailClient.Managers
{
    public class MailKitApiManager : IMailKitApiManager
    {
        private ImapClient _imapClient;
        private SmtpClient _smtpClient;

        public Task AuthorizeAsync(string userName, string password, EmailService emailService = EmailService.Gmail)
        {
            return Task.Run( async () =>
            {
                var emailServiceInfo = EmailServiceProvider.GetEmailServiceInfo(emailService);

                _imapClient = new ImapClient { ServerCertificateValidationCallback = (s, c, h, e) => true };

                await _imapClient.ConnectAsync(emailServiceInfo.ImapHost, emailServiceInfo.ImapPort, true);

                await _imapClient.AuthenticateAsync(userName, password);

                //_smtpClient = new SmtpClient { ServerCertificateValidationCallback = (s, c, h, e) => true };

                //await _smtpClient.ConnectAsync(emailServiceInfo.SmtpHost, emailServiceInfo.SmtpPort, false);

                //await _smtpClient.AuthenticateAsync(userName, password);
            });
        }

        public  Task<IEnumerable<MimeMessage>> GetAllMessagesAsync()
        {
            return Task.Run(async () =>
            {
                var clientInbox = _imapClient.Inbox;

                await clientInbox.OpenAsync(FolderAccess.ReadOnly);

                return await GetMessagesInternal(clientInbox);
            });
        }

        private async Task<IEnumerable<MimeMessage>> GetMessagesInternal(IMailFolder clientInbox)
        {
            var messages = new List<MimeMessage>();

            for (var i = clientInbox.Count - 1; i > clientInbox.Count - 10; i--)
            {
                messages.Add(await clientInbox.GetMessageAsync(i));
            }

            return messages;
        }
    }

    public interface IMailKitApiManager
    {
        Task AuthorizeAsync(string userName, string password, EmailService emailService = EmailService.Gmail);

        Task<IEnumerable<MimeMessage>> GetAllMessagesAsync();
    }
}
