using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using EmailClient.Managers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private readonly ReactiveCommand<Unit, Unit> _authenticate;

        public AuthorizationViewModel(
            INavigationManager navigationManager,
            IMailKitApiManager mailKitApiManager) 
            : base("AuthorizationViewModel")
        {
            _authenticate = ReactiveCommand.CreateFromTask(() => mailKitApiManager.AuthorizeAsync(UserName, Password, GetEmailService()));

            _authenticate.Select(unit => typeof(MainPageViewModel))
                .Subscribe(navigationManager.Navigate);

            _authenticate.ThrownExceptions
                .Select(exception => typeof(ErrorAuthViewModel))
                .Subscribe(navigationManager.Navigate);

            //todo check email correctness by subscribing on it
            //todo add support for other email services

            SelectedEmailService = 0;
        }

        public ICommand AuthenticateCommand => _authenticate;

        [Reactive] public string UserName { get; set; }
        [Reactive] public string Password { get; set; }
        [Reactive] public int SelectedEmailService { get; set; }

        private EmailService GetEmailService()
        {
            return SelectedEmailService switch
            {
                0 => EmailService.Gmail,
                1 => EmailService.Yandex,
                2 => EmailService.Mailru,
                _ => EmailService.Gmail,
            };
        }
    }
}
