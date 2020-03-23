using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;

namespace EmailClient.Managers
{
    public class MailKitApiManager : IMailKitApiManager
    {
        private ImapClient _imapClient;
        private SmtpClient _smtpClient;
        private string _username;
        private string _password;
        private EmailService _emailService;
        public Task AuthorizeAsync(string userName, string password, EmailService emailService = EmailService.Gmail)
        {
            _username = userName;
            _password = password;
            _emailService = emailService;

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

            var index = Math.Max(_imapClient.Inbox.Count - 100, 0);

            var messagesSummary = await _imapClient.Inbox
                .FetchAsync(index, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.All);

            return messagesSummary.Reverse();
        }

        public Task SendMessageAsync(string from, string to, string message, IEnumerable<string> attachmentsPaths)
        {
            if (_smtpClient == null)
            {
                return Task.Run(async () =>
                {
                    var emailServiceInfo = EmailServiceProvider.GetEmailServiceInfo(_emailService);

                    _smtpClient = new SmtpClient { ServerCertificateValidationCallback = (s, c, h, e) => true };

                    await _smtpClient.ConnectAsync(emailServiceInfo.SmtpHost, emailServiceInfo.SmtpPort, false);

                    await _smtpClient.AuthenticateAsync(_username, _password);

                    return SendMessageInternal(@from, to, message, attachmentsPaths);
                });
            }

            return SendMessageInternal(@from, to, message, attachmentsPaths);
        }

        public async Task<MimeMessage> GetMessageAsync(UniqueId selectedMessage)
        {
            if(_imapClient == null)
                return new MimeMessage();

            var message = await _imapClient.Inbox.GetMessageAsync(selectedMessage);

            return message;
        }

        private async Task SendMessageInternal(string from, string to, string message, IEnumerable<string> attachmentsPaths)
        {
            var messageToSent = new MimeMessage();
            messageToSent.From.Add(new MailboxAddress(@from, @from));
            messageToSent.To.Add(new MailboxAddress(to, to));
            messageToSent.Subject = message;

            var body = new TextPart("plain")
            {
                Text = message
            };

            var attachments = new List<MimePart>();

            foreach (var attachmentsPath in attachmentsPaths)
            {
                var attachment = new MimePart("image", "gif")
                {
                    Content = new MimeContent(File.OpenRead(attachmentsPath)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(attachmentsPath)
                };

                attachments.Add(attachment);
            }

            var multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.AddRange(attachments);

            messageToSent.Body = multipart;

            await _smtpClient.SendAsync(messageToSent);
        }
    }

    public interface IMailKitApiManager
    {
        Task AuthorizeAsync(string userName, string password, EmailService emailService = EmailService.Gmail);

        Task<IEnumerable<IMessageSummary>> GetAllMessagesAsync();
        Task SendMessageAsync(string from, string to, string message, IEnumerable<string> attachmentsPaths);
        Task<MimeMessage> GetMessageAsync(UniqueId selectedMessage);
    }
}
