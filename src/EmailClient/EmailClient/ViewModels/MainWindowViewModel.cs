using Avalonia;

namespace EmailClient.ViewModels
{
    public class MainWindowViewModel : AvaloniaObject
    {
        public MainWindowViewModel()
        {
            AuthorizationViewModel = new AuthorizationViewModel();
        }

        public AuthorizationViewModel AuthorizationViewModel { get; private set; }
    }
}
