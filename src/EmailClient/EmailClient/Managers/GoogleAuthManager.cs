using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        private string _code;
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

            var proc = new Process
            {
                StartInfo =
                    {
                        UseShellExecute = true,
                        FileName = authorizationRequest
                    }
            };
            proc.Start();


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
            _code = context.Request.QueryString.Get("code");
            var incomingState = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incomingState != state)
            {
                return;
            }

            // Starts the code exchange at the Token Endpoint.
            await PerformCodeExchange(_code, codeVerifier, redirectUri);
        }

        private async Task PerformCodeExchange(string code, string code_verifier, string redirectURI)
        {
            // builds the  request
            string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
            string tokenRequestBody =
                $"code={code}&redirect_uri={System.Uri.EscapeDataString(redirectURI)}&client_id={_clientId}&code_verifier={code_verifier}&client_secret={_clientSecret}&scope=&grant_type=authorization_code";

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                var tokenResponse = await tokenRequest.GetResponseAsync();
                using StreamReader reader = new StreamReader(tokenResponse.GetResponseStream());
                // reads response body
                var responseText = await reader.ReadToEndAsync();

                // converts to dictionary
                Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                var access_token = tokenEndpointDecoded["access_token"];

                if(access_token == null)
                    throw new AuthenticationException("Token is Null");

                await UserinfoCall(access_token);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        using StreamReader reader = new StreamReader(response.GetResponseStream());
                        // reads response body
                        var responseText = await reader.ReadToEndAsync();
                    }
                }
            }
        }

        private async Task UserinfoCall(string access_token)
        {
            // builds the  request
            var userinfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";

            // sends the request
            var userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            // gets the response
            var userinfoResponse = await userinfoRequest.GetResponseAsync();
            using StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream());
            // reads response body
            var userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
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
