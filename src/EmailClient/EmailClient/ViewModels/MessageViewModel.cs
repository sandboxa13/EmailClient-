using System.Reactive;
using System.Windows.Input;
using EmailClient.Managers;
using MailKit;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        private readonly ReactiveCommand<Unit, Unit> _deleteMessage;
        private readonly ReactiveCommand<Unit, Unit> _doubleClick;

        public MessageViewModel(
            IMessageSummary mimeMessage, 
            INavigationManager navigationManager, 
            ISelectedMessageManager selectedMessageManager) 
            : base("MessageViewModel")
        {
            _doubleClick = ReactiveCommand.Create( () =>
            {
                selectedMessageManager.SelectNewMessage(Id);

                navigationManager.Navigate(typeof(ExtendedMessageViewModel));
            });

            Snippet = mimeMessage.NormalizedSubject;
            From = mimeMessage.Envelope.From.ToString();
            Date = mimeMessage.Envelope.Date.ToString();
            Id = mimeMessage.UniqueId;
        }

        [Reactive] public UniqueId Id { get; private set; }
        [Reactive] public string Snippet { get; private set; }
        [Reactive] public string From { get; private set; }
        [Reactive] public string Date { get; private set; }

        public ICommand DoubleClickCommand => _doubleClick;
        public ICommand DeleteMessageCommand => _deleteMessage;
    }
}
