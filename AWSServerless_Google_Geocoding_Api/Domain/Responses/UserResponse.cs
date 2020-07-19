using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_Google_Geocoding_Api.Domain.Responses
{
    public class UserResponse : BaseResponse
    {
        public User user { get; set; }

        private UserResponse(Boolean success, string message, User user) : base(success, message)
        {
            this.user = user;
        }

        //success

        public UserResponse(User user) : this(true, String.Empty, user)
        {
        }

        ////fail

        public UserResponse(string message) : this(false, message, null)
        {
        }
    }
}
