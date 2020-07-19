using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_Google_Geocoding_Api.Domain.Repositories
{
    public interface IUserRepository
    {
        User FindById(int userId);
        User FindByUsernameandPassword(string username, string password);
    }
}
