using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EmailClient
{
    public class LoginPage : UserControl
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
