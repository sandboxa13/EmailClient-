using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Managers
{
    public class GoogleAuthManager : IAuthManager
    {
        // client configuration
        private readonly string _clientId = "581786658708-elflankerquo1a6vsckabbhn25hclla0.apps.googleusercontent.com";
        private readonly string _authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private string _clientSecret = "3f6NggMbPtrmIBpgx-MK2xXK";
        private string _tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        private string _userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

        public GoogleAuthManager()
        {
            
        }

        public async Task Authenticate()
        {
            var state = RandomDataBase64Url(32);
            var codeVerifier = RandomDataBase64Url(32);
            var codeChallenge = Base64UrlencodeNoPadding(Sha256(codeVerifier));
            const string codeChallengeMethod = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            var redirectUri = $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);

            http.Start();

            // Creates the OAuth 2.0 authorization request.
            var authorizationRequest = $"{_authorizationEndpoint}?response_type=code&scope=openid%20profile&redirect_uri={System.Uri.EscapeDataString(redirectUri)}&client_id={_clientId}&state={state}&code_challenge={codeChallenge}&code_challenge_method={codeChallengeMethod}";

            // Opens request in the browser.

            try
            {
                var proc = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = true,
                        FileName = authorizationRequest
                    }
                };
                proc.Start();

            }
            catch (Exception e)
            {
                    
            }


            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();


            // Sends an HTTP response to the browser.
            var response = context.Response;
            var responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            var responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                return;
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incomingState = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incomingState != state)
            {
                return;
            }

            // Starts the code exchange at the Token Endpoint.
            //performCodeExchange(code, codeVerifier, redirectUri);
        }

        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static string RandomDataBase64Url(uint length)
        {
            var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64UrlencodeNoPadding(bytes);
        }

        private static string Base64UrlencodeNoPadding(byte[] buffer)
        {
            var base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        private static byte[] Sha256(string inputString)
        {
            var bytes = Encoding.ASCII.GetBytes(inputString);
            var sha256 = new SHA256Managed();

            return sha256.ComputeHash(bytes);
        }
    }
}
