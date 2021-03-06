﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EmailClient.Managers;
using MailKit;
using ReactiveUI.Fody.Helpers;

namespace EmailClient.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel(
            INavigationManager navigationManager,
            IMailKitApiManager mailKitApiManager,
            ISelectedMessageManager selectedMessageManager, 
            IDeleteMessageManager deleteMessageManager) 
            : base("MainWindowViewModel")
        {
            Pages = new List<BaseViewModel>
            {
                new AuthorizationViewModel(navigationManager, mailKitApiManager), 
                new MainPageViewModel(navigationManager, mailKitApiManager, selectedMessageManager, deleteMessageManager), 
                new ErrorAuthViewModel(navigationManager),
                new NewMessageViewModel(navigationManager, mailKitApiManager), 
                new ExtendedMessageViewModel(UniqueId.Invalid, navigationManager, mailKitApiManager),
                new CustomAuthorizationViewModel(navigationManager, mailKitApiManager)
            };

            navigationManager.CurrentPage()
                .Select(type => Pages.First(x => x.GetType() == type))
                .Subscribe(viewModel =>
                {
                    if (viewModel.Name == "ExtendedMessageViewModel")
                    {
                        Pages.RemoveAt(Pages.Count - 1);

                        Pages.Add(new ExtendedMessageViewModel(selectedMessageManager.SelectedMessage,
                            navigationManager, mailKitApiManager));

                        CurrentPage = Pages.Last();
                        return;
                    }

                    CurrentPage = viewModel;
                });
        }

        [Reactive] public List<BaseViewModel> Pages { get; private set; }

        [Reactive] public BaseViewModel CurrentPage { get; private set; }
    }
}
