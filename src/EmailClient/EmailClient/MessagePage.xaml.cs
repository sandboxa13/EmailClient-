using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EmailClient
{
    public class MessagePage : UserControl
    {
        public MessagePage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
