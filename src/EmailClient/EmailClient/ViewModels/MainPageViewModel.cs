﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using EmailClient.Managers;
using MailKit;
using ReactiveUI;

namespace EmailClient.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly INavigationManager _navigationManager;
        private readonly IMailKitApiManager _mailKitApiManager;
        private readonly ISelectedMessageManager _selectedMessageManager;
        private readonly IDeleteMessageManager _deleteMessageManager;
        private SourceList<MessageViewModel> Messages { get; set; } = new SourceList<MessageViewModel>();
        private readonly ReadOnlyObservableCollection<MessageViewModel> _messageCollection;
        private readonly ReactiveCommand<Unit, Unit> _writeMessage;

        public MainPageViewModel(
            INavigationManager navigationManager,
            IMailKitApiManager mailKitApiManager, 
            ISelectedMessageManager selectedMessageManager,
            IDeleteMessageManager deleteMessageManager)
            : base("MainPageViewModel")
        {
            _navigationManager = navigationManager;
            _mailKitApiManager = mailKitApiManager;
            _selectedMessageManager = selectedMessageManager;
            _deleteMessageManager = deleteMessageManager;

            _writeMessage = ReactiveCommand.Create(() => {});

            _writeMessage.Select(unit => typeof(NewMessageViewModel))
                .Subscribe(navigationManager.Navigate);

            navigationManager.CurrentPage()
                .Subscribe(async vm => await LoadMessages(vm));

            _deleteMessageManager.MessageDeletedObservable.Subscribe(id =>
            {
                var message = Messages.Items.FirstOrDefault(model => model.Id == id);

                if(message == null)
                    return;

                Messages.Edit(list => list.Remove(message));
            });

            Messages.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _messageCollection)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<MessageViewModel> MessagesCollection => _messageCollection;
        public ICommand WriteMessageCommand => _writeMessage;

        private async Task LoadMessages(Type viewModel)
        {
            if (viewModel.Name != "MainPageViewModel")
                return;

            if(_messageCollection.Any())
                return;

            var messages = await _mailKitApiManager.GetAllMessagesAsync();

            Messages.AddRange(ConvertToViewModel(messages));
        }

        private IEnumerable<MessageViewModel> ConvertToViewModel(IEnumerable<IMessageSummary> messages)
        {
            return messages.Select(message => new MessageViewModel(message, _navigationManager, _selectedMessageManager, _deleteMessageManager));
        }
    }
}
