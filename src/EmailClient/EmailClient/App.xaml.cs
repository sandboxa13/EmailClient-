using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EmailClient.Managers;
using EmailClient.ViewModels;

namespace EmailClient
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var navigationManager = new NavigationManager();
                var googleApiManager = new MailKitApiManager();
                var selectedMessageManager = new SelectedMessageManager();
                var deleteMessageManager = new DeleteMessageManager(googleApiManager);

                var vm = new MainWindowViewModel(navigationManager, googleApiManager, selectedMessageManager, deleteMessageManager);
                var window = new MainWindow {DataContext = vm};

                desktop.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
