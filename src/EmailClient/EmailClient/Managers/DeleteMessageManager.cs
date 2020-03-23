using System;
using System.Reactive.Subjects;
using MailKit;

namespace EmailClient.Managers
{
    public class DeleteMessageManager : IDeleteMessageManager
    {
        private readonly IMailKitApiManager _mailKitApiManager;
        private readonly ISubject<UniqueId> _messageDeletedSubject;
        public DeleteMessageManager(IMailKitApiManager mailKitApiManager)
        {
            _mailKitApiManager = mailKitApiManager;
            _messageDeletedSubject = new BehaviorSubject<UniqueId>(UniqueId.Invalid);
        }
        public IObservable<UniqueId> MessageDeletedObservable => _messageDeletedSubject;

        public void Delete(UniqueId uniqueId)
        {
            _mailKitApiManager.DeleteMessage(uniqueId);

            _messageDeletedSubject.OnNext(uniqueId);
        }
    }

    public interface IDeleteMessageManager
    {
        void Delete(UniqueId uniqueId);
        IObservable<UniqueId> MessageDeletedObservable { get; }
    }
}
