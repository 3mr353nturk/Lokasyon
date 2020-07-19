using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWSServerless_Google_Geocoding_Mvc.Models
{
    public class Location
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
    }
}