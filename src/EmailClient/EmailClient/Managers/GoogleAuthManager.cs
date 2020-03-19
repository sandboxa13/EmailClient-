using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EmailClient.Managers
{
    public class GoogleAuthManager : IAuthManager
    {
        // client configuration
        //    static readonly string[] Scopes = { GmailService.Scope.GmailReadonly };

        //    public Task<UserCredential> Authenticate()
        //    {
        //        UserCredential credential;

        //        var credPath = "token.json";
        //        using (var stream =
        //            new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        //        {
        //            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //                GoogleClientSecrets.Load(stream).Secrets,
        //                Scopes,
        //                "user",
        //                CancellationToken.None,
        //                new FileDataStore(credPath, true)).Result;
        //        }

        //        return Task.FromResult(credential);
        //    }
        //}
    }
}
