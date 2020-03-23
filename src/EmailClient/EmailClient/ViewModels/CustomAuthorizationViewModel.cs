using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using EmailClient.Managers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class CustomAuthorizationViewModel : BaseViewModel
    {
        private readonly ReactiveCommand<Unit, Unit> _authenticate;
        private readonly ReactiveCommand<Unit, Unit> _default;

        public CustomAuthorizationViewModel(
            INavigationManager navigationManager,
            IMailKitApiManager mailKitApiManager)
            : base("AuthorizationViewModel")
        {
            _authenticate = ReactiveCommand.CreateFromTask(() => mailKitApiManager.AuthorizeCustomAsync(UserName, Password, ImapHost, ImapPort, SmtpHost, SmtpPort));
            _default = ReactiveCommand.Create(() => navigationManager.Navigate(typeof(AuthorizationViewModel)));

            _authenticate.Select(unit => typeof(MainPageViewModel))
                .Subscribe(navigationManager.Navigate);

            _authenticate.ThrownExceptions
                .Select(exception => typeof(ErrorAuthViewModel))
                .Subscribe(navigationManager.Navigate);

            //todo check email correctness by subscribing on it
            //todo add support for other email services
        }

        public ICommand AuthenticateCommand => _authenticate;
        public ICommand DefaultCommand => _default;

        [Reactive] public string UserName { get; set; }
        [Reactive] public string Password { get; set; }

        [Reactive] public string ImapHost { get; set; }
        [Reactive] public string ImapPort { get; set; }

        [Reactive] public string SmtpHost { get; set; }
        [Reactive] public string SmtpPort { get; set; }
    }
}
