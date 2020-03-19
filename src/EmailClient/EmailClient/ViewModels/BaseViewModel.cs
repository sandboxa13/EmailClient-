using ReactiveUI;

namespace EmailClient.ViewModels
{
    public class BaseViewModel : ReactiveObject
    {
        public BaseViewModel(string name)
        {
            Name = name;
        }
        
        public string Name { get; }
    }
}
