using MailKit;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        public MessageViewModel(IMessageSummary mimeMessage) 
            : base("MessageViewModel")
        {
            Snippet = mimeMessage.NormalizedSubject;
            From = mimeMessage.Envelope.From.ToString();
            Date = mimeMessage.Envelope.Date.ToString();
            Id = mimeMessage.Id;
        }

        [Reactive] public string Id { get; private set; }
        [Reactive] public string Snippet { get; private set; }
        [Reactive] public string From { get; private set; }
        [Reactive] public string Date { get; private set; }
    }
}
