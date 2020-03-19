using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using EmailClient.Managers;
using MimeKit;
using ReactiveUI;

namespace EmailClient.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly IMailKitApiManager _mailKitApiManager;
        private SourceList<MessageViewModel> _messages { get; set; } = new SourceList<MessageViewModel>();
        private readonly ReadOnlyObservableCollection<MessageViewModel> _messageCollection;

        public MainPageViewModel(
            INavigationManager navigationManager,
            IMailKitApiManager mailKitApiManager)
            : base("MainPageViewModel")
        {
            _mailKitApiManager = mailKitApiManager;

            navigationManager.CurrentPage()
                .Subscribe(async vm => await LoadMessages(vm));

            _messages.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _messageCollection)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<MessageViewModel> PhotoCollection => _messageCollection;

        private async Task LoadMessages(Type viewModel)
        {
            if (viewModel.Name != "MainPageViewModel")
                return;

            var messages = await _mailKitApiManager.GetAllMessagesAsync();

            _messages.AddRange(ConvertToViewModel(messages));
        }

        private IEnumerable<MessageViewModel> ConvertToViewModel(IEnumerable<MimeMessage> messages)
        {
            return messages.Select(message => new MessageViewModel(message));
        }
    }
}
