using System;
using System.Collections.Generic;

namespace AWSServerless_Google_Geocoding_Api.Domain
{
    public partial class User
    {
        public User()
        {
            Map = new HashSet<Map>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Map> Map { get; set; }
    }
}
