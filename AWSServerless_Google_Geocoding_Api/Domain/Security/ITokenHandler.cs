using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_Google_Geocoding_Api.Domain.Security
{
    public interface ITokenHandler
    {
        AccessToken CreateAccessToken(User user);
    }
}
