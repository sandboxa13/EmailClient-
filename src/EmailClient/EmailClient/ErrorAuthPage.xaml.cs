using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EmailClient
{
    public class ErrorAuthPage : UserControl
    {
        public ErrorAuthPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
