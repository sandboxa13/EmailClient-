using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using EmailClient.Managers;
using ReactiveUI;

namespace EmailClient.ViewModels
{
    public class NewMessageViewModel :BaseViewModel
    {
        private readonly ReactiveCommand<Unit, Unit> _sendMessage;

        public NewMessageViewModel(
            INavigationManager navigationManager, 
            IMailKitApiManager mailKitApiManager)
            : base("NewMessageViewModel")
        {
            _sendMessage = ReactiveCommand.CreateFromTask(mailKitApiManager.SendMessageAsync);

            _sendMessage.Select(unit => typeof(MainPageViewModel))
                .Subscribe(navigationManager.Navigate);
        }

        public ICommand SendMessageCommand => _sendMessage;
    }
}
