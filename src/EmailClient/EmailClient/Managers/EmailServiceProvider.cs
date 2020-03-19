using System;

namespace EmailClient.Managers
{
    public static class EmailServiceProvider
    {
        public static EmailServiceInfo GetEmailServiceInfo(EmailService emailService)
        {
            switch (emailService)
            {
                case EmailService.Gmail:
                    return new EmailServiceInfo("smtp.gmail.com", 587, "imap.gmail.com", 993);
                case EmailService.Yandex:
                    break;
                case EmailService.Mailru:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(emailService), emailService, null);
            }

            return EmailServiceInfo.Default();
        }
    }
}