using MimeKit;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        public MessageViewModel(MimeMessage mimeMessage) 
            : base("MessageViewModel")
        {
            Snippet = mimeMessage.Subject;
            From = mimeMessage.From.ToString();
        }

        [Reactive] public string Id { get; private set; }
        [Reactive] public string Snippet { get; private set; }
        [Reactive] public string From { get; private set; }
    }
}
