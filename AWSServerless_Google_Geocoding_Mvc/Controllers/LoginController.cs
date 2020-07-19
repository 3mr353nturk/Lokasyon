using AWSServerless_Google_Geocoding_Mvc.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AWSServerless_Google_Geocoding_Mvc.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return Redirect("Login");
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> AddUser(FormCollection form)
        {
            User user = new User();
            user.FirstName = form["name"];
            user.LastName = form["surname"];
            user.Email = form["email"];
            user.Username = form["username"];
            user.Password = form["password"];

            var json = JsonConvert.SerializeObject(user);

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync("api/Login/AddUser", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Login");
                }
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        public ActionResult Token(User User)
        {
            if (ModelState.IsValid)
            {
                using (LocationDBEntities db = new LocationDBEntities())
                {
                    var obj = db.User.Where(a => a.Username.Equals(User.Username) && a.Password.Equals(User.Password)).FirstOrDefault();

                    if (obj != null)
                    {
                        Session.Add("UserID", obj.UserId.ToString());
                        Session.Add("FirstName", obj.FirstName.ToString() + " " + obj.LastName.ToString());

                        return RedirectToAction("Index", "Admin");
                    }
                }

                //    LocationDBEntities db = new LocationDBEntities();
                //    var obj = db.User.Where(a => a.Username.Equals(User.Username) && a.Password.Equals(User.Password)).FirstOrDefault();

                //    if (obj != null)
                //    {
                //        HttpContext.Session.Add("UserID", obj.UserId.ToString());
                //        HttpContext.Session.Add("FirstName", obj.FirstName.ToString() + " " + obj.LastName.ToString());


                //        HttpCookie userIdCookie = new HttpCookie("UserID", obj.UserId.ToString());
                //        userIdCookie.Expires = DateTime.Now.AddDays(365);
                //        HttpContext.Response.Cookies.Add(userIdCookie);

                //        HttpCookie userNameCookie = new HttpCookie("FirstName", obj.FirstName.ToString());
                //        userNameCookie.Expires = DateTime.Now.AddDays(365);
                //        HttpContext.Response.Cookies.Add(userNameCookie);
                //    }



                //    User user = new User();
                //    user.Username = User.Username;
                //    user.Password = User.Password;

                //    var json = JsonConvert.SerializeObject(user);

                //    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                //    using (var client = new HttpClient())
                //    {
                //        client.BaseAddress = new Uri("https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod/");
                //        client.DefaultRequestHeaders.Accept.Clear();
                //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //        HttpResponseMessage response = client.PostAsync("api/login/accesstoken", stringContent).Result;
                //        if (response.IsSuccessStatusCode)
                //        {
                //            return RedirectToAction("Index", "Admin");
                //        }

                //        return RedirectToAction("Login", "Login");

                //    }
                //}
            }
            return RedirectToAction("Login", "Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login(FormCollection formCollection)
        {
            var username = formCollection["Username"];
            var password = formCollection["Password"];
            if (Request.Cookies["Username"] != null && Request.Cookies["Password"] != null)
            {
                username = Request.Cookies["Username"].Value;
                password = Request.Cookies["Password"].Value;
            }
            return View();
        }
    }
}