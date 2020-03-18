﻿using System;
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

        public AuthorizationViewModel(
            INavigationManager navigationManager,
            IGoogleApiManager googleApiManager)
        {
            _authenticate = ReactiveCommand.CreateFromTask(googleApiManager.AuthorizeAsync);

            _authenticate.Select(unit => typeof(MainPageViewModel))
                .Subscribe(navigationManager.Navigate);

            _authenticate.ThrownExceptions
                .Select(exception => typeof(ErrorAuthViewModel))
                .Subscribe(navigationManager.Navigate);
        }

        public ICommand AuthenticateCommand => _authenticate;
    }
}
