using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using EmailClient.Managers;
using ReactiveUI;

namespace EmailClient.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private readonly ReactiveCommand<Unit, Unit> _authenticate;

        public AuthorizationViewModel(INavigationManager navigationManager)
        {
            AuthManager = new GoogleAuthManager();

            _authenticate = ReactiveCommand.CreateFromTask(
                () => AuthManager.Authenticate());

            _authenticate.Select(unit => typeof(MainPageViewModel))
                .Subscribe(navigationManager.Navigate);
        }

        public ICommand AuthenticateCommand => _authenticate;

        public IAuthManager AuthManager { get; private set; }
    }
}
