using System.Web;
using System.Web.Mvc;

namespace AWSServerless_Google_Geocoding_Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
