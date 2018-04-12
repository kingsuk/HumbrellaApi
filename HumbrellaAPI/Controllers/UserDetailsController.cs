﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public IActionResult PushUserDetails([FromBody]UpdateUserDetailsEntity updateUserDetailsEntity)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, ModelState);
            }
            else
            {
                try
                {
                    string userId = HttpContext.User.Claims.Single(claim => claim.Type == ClaimTypes.Name).Value;
                    UserDetails userDetails = new UserDetails();
                    ResponseEnity response = userDetails.pushUserDetails(updateUserDetailsEntity, userId);

                    if (response.StatusCode == 1)
                    {
                        return StatusCode(200, response);
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