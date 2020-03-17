using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EmailClient
{
    public class MainPage : UserControl
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
