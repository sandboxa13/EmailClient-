using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EmailClient.Managers;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel(
            INavigationManager navigationManager,
            IMailKitApiManager mailKitApiManager) 
            : base("MainWindowViewModel")
        {
            Pages = new List<BaseViewModel>
            {
                new AuthorizationViewModel(navigationManager, mailKitApiManager), 
                new MainPageViewModel(navigationManager, mailKitApiManager), 
                new ErrorAuthViewModel(navigationManager),
                new NewMessageViewModel(navigationManager, mailKitApiManager)
            };

            navigationManager.CurrentPage()
                .Select(type => Pages.First(x => x.GetType() == type))
                .Subscribe(viewModel => CurrentPage = viewModel);
        }

        [Reactive] public IEnumerable<BaseViewModel> Pages { get; private set; }

        [Reactive] public BaseViewModel CurrentPage { get; private set; }
    }
}
