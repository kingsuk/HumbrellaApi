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
        [AppAuthorize]
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
                    DBResultEnity dBResult = auth.login(authEntity);

                    if (dBResult.StatusCode == 1)
                    {
                        return StatusCode(200, dBResult);
                    }
                    else if (dBResult.StatusCode == 0)
                    {
                        return StatusCode(401, dBResult);
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

        [AppAuthorize]
        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody]RegistrationEntity registrationEntity)
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
                    DBResultEnity dBResult = auth.register(registrationEntity);

                    if (dBResult.StatusCode == 1)
                    {
                        return StatusCode(200, dBResult);
                    }
                    else if (dBResult.StatusCode == 0)
                    {
                        return StatusCode(409, dBResult);
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

        [AppAuthorize]
        [HttpGet]
        [Route("CheckUserIDAvailability")]
        public IActionResult CheckUserIDAvailability(string userId)
        {
            if (userId == null || userId.Trim() == "")
            {
                return new BadRequestObjectResult("User Id invalid");
            }
            else
            {
                try
                {
                    Authentication auth = new Authentication();
                    DBResultEnity dBResult = auth.checkUserIDAvailability(userId);

                    if (dBResult.StatusCode == 1)
                    {
                        return StatusCode(200, dBResult);
                    }
                    else if (dBResult.StatusCode == 0)
                    {
                        return StatusCode(409, dBResult);
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

        [AppAuthorize]
        [HttpGet]
        [Route("CheckEmailAvailability")]
        public IActionResult CheckEmailAvailability(string email)
        {
            if (email == null || email.Trim() == "")
            {
                return new JsonResult(BadRequest("User Id invalid"));
            }
            else
            {
                try
                {
                    Authentication auth = new Authentication();
                    DBResultEnity dBResult = auth.checkUserEmailAvailability(email);

                    if(dBResult.StatusCode == 1)
                    {
                        return StatusCode(200, dBResult);
                    }
                    else if(dBResult.StatusCode == 0)
                    {
                        return StatusCode(409, dBResult);
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