using AWSServerless_Google_Geocoding_Api.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_Google_Geocoding_Api.Domain.Security
{
    public interface IAuthenticationService
    {
        AccessTokenResponse CreateAccessToken(string username, string password);
    }
}
