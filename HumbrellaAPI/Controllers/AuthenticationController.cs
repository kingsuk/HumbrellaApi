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
        //[AppAuthorize]
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody]AuthenticationEntity authEntity)
        {
            if (!ModelState.IsValid)
            {
                ResponseEntity response = new ResponseEntity();
                response.StatusCode = 0;
                response.StatusDesc = "Invalid parameter";
                response.Result = BadRequest(ModelState).Value;
                return StatusCode(400, response);
            }
            else
            {
                try
                {
                    Authentication auth = new Authentication();
                    ResponseEntity response = auth.login(authEntity);

                    if (response.StatusCode == 1)
                    {
                        return StatusCode(200, response);
                    }
                    else if (response.StatusCode == 0)
                    {
                        return StatusCode(401, response);
                    }
                    else
                    {
                        return StatusCode(501);
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
        }

        //[AppAuthorize]
        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody]RegistrationEntity registrationEntity)
        {
            if (!ModelState.IsValid)
            {
                ResponseEntity response = new ResponseEntity();
                response.StatusCode = 0;
                response.StatusDesc = "Invalid parameter";
                response.Result = BadRequest(ModelState).Value;
                return StatusCode(400, response);
            }
            else
            {
                try
                {
                    Authentication auth = new Authentication();
                    ResponseEntity response = auth.register(registrationEntity);

                    if (response.StatusCode == 1)
                    {
                        return StatusCode(200, response);
                    }
                    else if (response.StatusCode == 0)
                    {
                        return StatusCode(409, response);
                    }
                    else
                    {
                        return StatusCode(501);
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
        }

        //[AppAuthorize]
        [HttpGet]
        [Route("CheckUserIDAvailability")]
        public IActionResult CheckUserIDAvailability(string userId)
        {
            if (userId == null || userId.Trim() == "")
            {
                ResponseEntity response = new ResponseEntity();
                response.StatusCode = 0;
                response.StatusDesc = "User Id invalid";
                return StatusCode(400, response);
            }
            else
            {
                try
                {
                    Authentication auth = new Authentication();
                    ResponseEntity response = auth.checkUserIDAvailability(userId);

                    if (response.StatusCode == 1)
                    {
                        return StatusCode(200, response);
                    }
                    else if (response.StatusCode == 0)
                    {
                        return StatusCode(409, response);
                    }
                    else
                    {
                        return StatusCode(501);
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
        }

        //[AppAuthorize]
        [HttpGet]
        [Route("CheckEmailAvailability")]
        public IActionResult CheckEmailAvailability(string email)
        {
            if (email == null || email.Trim() == "")
            {
                ResponseEntity response = new ResponseEntity();
                response.StatusCode = 0;
                response.StatusDesc = "Email Id invalid";
                return StatusCode(400, response);
            }
            else
            {
                try
                {
                    Authentication auth = new Authentication();
                    ResponseEntity response = auth.checkUserEmailAvailability(email);

                    if(response.StatusCode == 1)
                    {
                        return StatusCode(200, response);
                    }
                    else if(response.StatusCode == 0)
                    {
                        return StatusCode(409, response);
                    }
                    else
                    {
                        return StatusCode(501);
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
        }
    }
}