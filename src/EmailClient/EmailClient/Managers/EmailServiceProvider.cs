using System;

namespace EmailClient.Managers
{
    public static class EmailServiceProvider
    {
        public static EmailServiceInfo GetEmailServiceInfo(EmailService emailService)
        {
            return emailService switch
            {
                EmailService.Gmail => new EmailServiceInfo("smtp.gmail.com", 587, "imap.gmail.com", 993),
                EmailService.Yandex => new EmailServiceInfo("smtp.yandex.ru", 465, "imap.yandex.ru", 993),
                EmailService.Mailru => new EmailServiceInfo("smtp.mail.ru", 465, "imap.mail.ru", 993),
                _ => throw new ArgumentOutOfRangeException(nameof(emailService), emailService, null),
            };
        }
    }
}