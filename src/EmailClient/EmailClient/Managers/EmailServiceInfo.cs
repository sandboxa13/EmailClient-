namespace EmailClient.Managers
{
    public class EmailServiceInfo
    {
        public EmailServiceInfo(
            string smtpHost, 
            int smtpPort,
            string imapHost,
            int imapPort)
        {
            SmtpHost = smtpHost;
            SmtpPort = smtpPort;

            ImapHost = imapHost;
            ImapPort = imapPort;
        }

        public string SmtpHost { get; private set; }
        public int SmtpPort { get; private set; }

        public string ImapHost { get; private set; }
        public int ImapPort { get; private set; }

        public static EmailServiceInfo Default()
        {
            return new EmailServiceInfo("", 0, "", 0);
        }
    }
}