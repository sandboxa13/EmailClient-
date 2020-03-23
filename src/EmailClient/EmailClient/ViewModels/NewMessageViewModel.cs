using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using EmailClient.Managers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class NewMessageViewModel :BaseViewModel
    {
        private readonly ReactiveCommand<Unit, Unit> _sendMessage;
        private readonly ReactiveCommand<Unit, Unit> _attach;
        private readonly List<string> _attachmentsPaths;
        public NewMessageViewModel(
            INavigationManager navigationManager, 
            IMailKitApiManager mailKitApiManager)
            : base("NewMessageViewModel")
        {
            _attachmentsPaths = new List<string>();

            _sendMessage = ReactiveCommand.CreateFromTask(() => mailKitApiManager.SendMessageAsync(From, To, Message ?? "", _attachmentsPaths));
            _attach = ReactiveCommand.CreateFromTask(AttachHandler);

            _sendMessage.Select(unit => typeof(MainPageViewModel))
                .Subscribe(navigationManager.Navigate);
        }

        private async Task AttachHandler()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var dialog = new OpenFileDialog();
                dialog.Filters.Add(new FileDialogFilter { Name = "Text", Extensions = { "txt" } });
                dialog.Filters.Add(new FileDialogFilter { Name = "Png", Extensions = { "png" } });
                dialog.Filters.Add(new FileDialogFilter { Name = "Jpeg", Extensions = { "jpg" } });

                var results = await dialog.ShowAsync(desktop.MainWindow);

                _attachmentsPaths.AddRange(results);
            }

            await Task.CompletedTask;
        }

        public ICommand SendMessageCommand => _sendMessage;
        public ICommand AttachCommand => _attach;

        [Reactive] public string From { get; private set; }
        [Reactive] public string To { get; private set; }
        [Reactive] public string Message { get; private set; }
    }
}
