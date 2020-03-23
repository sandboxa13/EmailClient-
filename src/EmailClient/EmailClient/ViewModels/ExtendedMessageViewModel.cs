using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using EmailClient.Managers;
using MailKit;
using MimeKit;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class ExtendedMessageViewModel : BaseViewModel
    {
        private readonly ReactiveCommand<Unit, Unit> _back;
        private readonly ReactiveCommand<Unit, Unit> _downloadFiles;
        private MimeMessage _message;
        public ExtendedMessageViewModel(
            UniqueId selectedMessage,
            INavigationManager navigationManager,
            IMailKitApiManager mailKitApiManager) 
            : base("ExtendedMessageViewModel")
        {
            _back = ReactiveCommand.Create(() => navigationManager.Navigate(typeof(MainPageViewModel)));
            _downloadFiles = ReactiveCommand.CreateFromTask(DownloadFilesHandlerAsync);

            Initialize(mailKitApiManager, selectedMessage);
        }

        [Reactive] public string From { get; private set; }
        [Reactive] public string To { get; private set; }
        [Reactive] public string Message { get; private set; }

        public ICommand BackCommand => _back;
        public ICommand DownloadFilesCommand => _downloadFiles;

        private void Initialize(IMailKitApiManager mailKitApiManager, UniqueId selectedMessage)
        {
            Task.Run(async () =>
            {
                _message = await mailKitApiManager.GetMessageAsync(selectedMessage);

                From = _message.From.ToString();
                To = _message.To.ToString();
                Message = _message.TextBody;
            });
        }

        private async Task DownloadFilesHandlerAsync()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var dialog = new OpenFolderDialog();

                var results = await dialog.ShowAsync(desktop.MainWindow);

                foreach (var attachment in _message.Attachments)
                {
                    var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                    var combined = Path.Combine(results, fileName);

                    using var stream = File.Create(combined);
                    if (attachment is MessagePart rfc822)
                    {
                        rfc822.Message.WriteTo(stream);
                    }
                    else
                    {
                        var part = (MimePart)attachment;

                        part.Content.DecodeTo(stream);
                    }
                }
            }

            await Task.CompletedTask;
        }
    }
}
