using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EmailClient
{
    public class CustomAuthoriaztionView : UserControl
    {
        public CustomAuthoriaztionView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
