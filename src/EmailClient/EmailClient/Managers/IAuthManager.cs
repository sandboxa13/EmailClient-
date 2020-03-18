﻿using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace EmailClient.Managers
{
    public interface IAuthManager
    {
        Task<UserCredential> Authenticate();
    }
}
