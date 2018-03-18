using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumbrellaAPI.Entities;
using HumbrellaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HumbrellaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/Authentication")]
    public class AuthenticationController : Controller
    {
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody]AuthenticationEntity authEntity)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(BadRequest(ModelState));
            }
            else
            {
                try
                {
                    Authentication auth = new Authentication();
                    return new JsonResult(auth.login(authEntity));
                }
                catch (Exception e)
                {
                    return new JsonResult(StatusCode(500, e.Message));
                }
            }
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody]AuthenticationEntity authEntity)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(BadRequest(ModelState));
            }
            else
            {
                try
                {
                    Authentication auth = new Authentication();
                    return new JsonResult(auth.register(authEntity));
                }
                catch (Exception e)
                {
                    return new JsonResult(StatusCode(500, e.Message));
                }
            }
        }

        [HttpPost]
        [Route("CheckUserIDAvailability")]
        public IActionResult CheckUserIDAvailability([FromBody]string userId)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(BadRequest(ModelState));
            }
            else
            {
                try
                {
                    Authentication auth = new Authentication();
                    return new JsonResult(auth.checkUserIDAvailability(userId));
                }
                catch (Exception e)
                {
                    return new JsonResult(StatusCode(500, e.Message));
                }
            }
        }
    }
}