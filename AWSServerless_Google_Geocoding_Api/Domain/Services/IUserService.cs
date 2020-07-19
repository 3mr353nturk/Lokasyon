using AWSServerless_Google_Geocoding_Api.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_Google_Geocoding_Api.Domain.Services
{
    public interface IUserService
    {
        UserResponse FindById(int userId);
        UserResponse FindUsernameAndPassword(string username, string password);
    }
}
