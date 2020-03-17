using System;
using System.Reactive.Subjects;
using EmailClient.ViewModels;

namespace EmailClient.Managers
{
    public class NavigationManager : INavigationManager
    {
        private readonly BehaviorSubject<Type> _currentPage;

        public NavigationManager() => _currentPage = new BehaviorSubject<Type>(typeof(AuthorizationViewModel));

        public void Navigate(Type viewModel) => _currentPage.OnNext(viewModel);

        public IObservable<Type> CurrentPage() => _currentPage;
    }

    public interface INavigationManager
    {
        void Navigate(Type viewModelType);

        IObservable<Type> CurrentPage();
    }
}
