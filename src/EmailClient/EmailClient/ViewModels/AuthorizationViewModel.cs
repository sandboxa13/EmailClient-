using System.Reactive;
using System.Windows.Input;
using Avalonia;
using EmailClient.Managers;
using ReactiveUI;

namespace EmailClient.ViewModels
{
    public class AuthorizationViewModel : AvaloniaObject
    {
        private readonly ReactiveCommand<Unit, Unit> _authenticate;

        public AuthorizationViewModel()
        {
            AuthManager = new GoogleAuthManager();

            _authenticate = ReactiveCommand.CreateFromTask(
                () => AuthManager.Authenticate());

            _authenticate.Execute();
        }

        public ICommand AuthenticateCommand => _authenticate;

        public IAuthManager AuthManager { get; private set; }
    }
}
