using System.Collections.Generic;
using System.Reactive;
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
    public class NewMessageViewModel : BaseViewModel
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

            _sendMessage = ReactiveCommand.CreateFromTask(() => SendMessageAsync(mailKitApiManager, navigationManager));
            _attach = ReactiveCommand.CreateFromTask(AttachHandler);
        }

        public ICommand SendMessageCommand => _sendMessage;
        public ICommand AttachCommand => _attach;

        [Reactive] public string From { get; private set; }
        [Reactive] public string To { get; private set; }
        [Reactive] public string Message { get; private set; }
        [Reactive] public string ErrorMessage { get; private set; }

        private async Task SendMessageAsync(IMailKitApiManager mailKitApiManager, INavigationManager navigationManager)
        {
            if (string.IsNullOrWhiteSpace(From))
            {
                ErrorMessage = "no sender specified";
                return;
            }

            if (string.IsNullOrWhiteSpace(To))
            {
                ErrorMessage = "no recipient specified";
                return;
            }

            await mailKitApiManager.SendMessageAsync(From, To, Message ?? "", _attachmentsPaths);

            From = string.Empty;
            To = string.Empty;
            Message = string.Empty;
            _attachmentsPaths.Clear();
            ErrorMessage = string.Empty;

            navigationManager.Navigate(typeof(MainPageViewModel));
        }

        private async Task AttachHandler()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var dialog = new OpenFileDialog();
                dialog.Filters.Add(new FileDialogFilter { Name = "Text", Extensions = { "txt" } });
                dialog.Filters.Add(new FileDialogFilter { Name = "Png", Extensions = { "png" } });
                dialog.Filters.Add(new FileDialogFilter { Name = "Jpg", Extensions = { "jpg" } });

                var results = await dialog.ShowAsync(desktop.MainWindow);

                _attachmentsPaths.AddRange(results);
            }

            await Task.CompletedTask;
        }
    }
}
