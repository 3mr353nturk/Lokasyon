using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWSServerless_Google_Geocoding_Api.Domain;
using AWSServerless_Google_Geocoding_Api.Domain.ModelStateExtensions;
using AWSServerless_Google_Geocoding_Api.Domain.Resources;
using AWSServerless_Google_Geocoding_Api.Domain.Responses;
using AWSServerless_Google_Geocoding_Api.Domain.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWSServerless_Google_Geocoding_Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly IAuthenticationService authenticationService;
        private readonly LocationDBContext db;
        public LoginController(IAuthenticationService authenticationService, LocationDBContext _db)
        {
            this.authenticationService = authenticationService;
            db = _db;
        }

        [HttpPost]
        public IActionResult Accesstoken(LoginResource loginResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            else
            {
                AccessTokenResponse accessTokenResponse = authenticationService.CreateAccessToken(loginResource.Username, loginResource.Password);

                if (accessTokenResponse.Success)
                {
                    return Ok(accessTokenResponse.accesstoken);
                }
                else
                {
                    return BadRequest(accessTokenResponse.Message);
                }
            }
        }


        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {

            db.User.Add(new User()
            {

                Username = user.Username,
                Password = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email

            });

            db.SaveChanges();
            return Ok("Kayıt Başarıyla eklendi");
        }

    }
}