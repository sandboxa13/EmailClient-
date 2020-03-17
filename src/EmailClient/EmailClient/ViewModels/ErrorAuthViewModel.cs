using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using EmailClient.Managers;
using ReactiveUI;

namespace EmailClient.ViewModels
{
    public class ErrorAuthViewModel : BaseViewModel
    {
        private readonly ReactiveCommand<Unit, Unit> _retry;

        public ErrorAuthViewModel(INavigationManager navigationManager)
        {
            _retry = ReactiveCommand.Create(() => { });

            _retry.Select(unit => typeof(AuthorizationViewModel))
                .Subscribe(navigationManager.Navigate);
        }

        public ICommand RetryCommand => _retry;
    }
}
