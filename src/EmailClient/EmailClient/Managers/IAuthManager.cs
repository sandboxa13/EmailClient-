using System.Threading.Tasks;

namespace EmailClient.Managers
{
    public interface IAuthManager
    {
        Task Authenticate();
    }
}
