using AWSServerless_Google_Geocoding_Api.Domain.Security;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_Google_Geocoding_Api.Domain.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly TokenOptions tokenOptions;
        public UserRepository(LocationDBContext context, IOptions<TokenOptions> tokenOptions) : base(context)
        {
            this.tokenOptions = tokenOptions.Value;
        }

        public User FindById(int userId)
        {
            return context.User.Find(userId);
        }

        public User FindByUsernameandPassword(string username, string password)
        {
            return context.User.Where(u => u.Username == username && u.Password == password).FirstOrDefault();
        }
    }
}
