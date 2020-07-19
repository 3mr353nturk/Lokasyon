using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSServerless_Google_Geocoding_Mvc.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}