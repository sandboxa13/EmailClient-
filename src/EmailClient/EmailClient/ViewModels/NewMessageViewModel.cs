using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using EmailClient.Managers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class NewMessageViewModel :BaseViewModel
    {
        private readonly ReactiveCommand<Unit, Unit> _sendMessage;
        private readonly ReactiveCommand<Unit, Unit> _attach;

        public NewMessageViewModel(
            INavigationManager navigationManager, 
            IMailKitApiManager mailKitApiManager)
            : base("NewMessageViewModel")
        {
            _sendMessage = ReactiveCommand.CreateFromTask(mailKitApiManager.SendMessageAsync);
            _attach = ReactiveCommand.CreateFromTask(mailKitApiManager.SendMessageAsync);

            _sendMessage.Select(unit => typeof(MainPageViewModel))
                .Subscribe(navigationManager.Navigate);
        }

        public ICommand SendMessageCommand => _sendMessage;
        public ICommand AttachCommand => _attach;

        [Reactive] public string From { get; private set; }
        [Reactive] public string To { get; private set; }
        [Reactive] public string Message { get; private set; }
    }
}
