using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;

namespace EmailClient.Managers
{
    public class GoogleApiManager : IGoogleApiManager
    {
        private readonly IAuthManager _authManager;
        private GmailService _gmailService;
        public GoogleApiManager()
        {
            _authManager = new GoogleAuthManager();
        }

        public async Task AuthorizeAsync()
        {
            var credential = await _authManager.Authenticate();

            CreateService(credential);
        }

        private void CreateService(UserCredential credential)
        {
            _gmailService = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Email client",
            });
        }
    }

    public interface IGoogleApiManager
    {
        Task AuthorizeAsync();
    }
}
