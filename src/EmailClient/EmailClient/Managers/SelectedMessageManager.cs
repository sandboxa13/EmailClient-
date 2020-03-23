using MailKit;

namespace EmailClient.Managers
{
    public class SelectedMessageManager : ISelectedMessageManager
    {
        public UniqueId SelectedMessage { get; private set; }

        public void SelectNewMessage(UniqueId id)
        {
            SelectedMessage = id;
        }
    }

    public interface ISelectedMessageManager
    {
        UniqueId SelectedMessage { get; }
        void SelectNewMessage(UniqueId id);
    }
}
