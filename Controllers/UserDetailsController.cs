using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumbrellaAPI.Entities;
using HumbrellaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HumbrellaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/UserDetails")]
    public class UserDetailsController : Controller
    {
        [Authorize]
        [HttpPost]
        [Route("PushUserDetails")]
        public IActionResult PushUserDetails([FromBody]UserDetailsEntity userDetailsEntity)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(BadRequest(ModelState));
            }
            else
            {
                try
                {
                    UserDetails userDetails = new UserDetails();
                    return new JsonResult(userDetails.pushUserDetails(userDetailsEntity));
                }
                catch (Exception e)
                {
                    return new JsonResult(StatusCode(500, e.Message));
                }
            }
        }
    }
}