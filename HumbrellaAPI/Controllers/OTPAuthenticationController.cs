using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumbrellaAPI.Entities;
using HumbrellaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HumbrellaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/OTPAuthentication")]
    public class OTPAuthenticationController : Controller
    {
        IConfiguration configuration;
        public OTPAuthenticationController(IConfiguration iconfiguration)
        {
            configuration = iconfiguration;
        }
        //[AppAuthorize]
        [HttpPost]
        [Route("SendMobileOTP")]
        public IActionResult SendMobileOTP([FromBody]string mobileNumber)
        {
            Int64 number;
            if (mobileNumber == null || mobileNumber.Trim() == "" || mobileNumber.Length!=10 || !Int64.TryParse(mobileNumber, out number))
            {
                ResponseEnity response = new ResponseEnity();
                response.StatusCode = 0;
                response.StatusDesc = "Mobile number invalid";
                return StatusCode(400, response);
            }
            else
            {
                try
                {
                    OTPAuthentication otpAuth = new OTPAuthentication(configuration);
                    ResponseEnity response = otpAuth.SendMobileOTP(mobileNumber);

                    if (response.StatusCode == 1)
                    {
                        return StatusCode(200, response);
                    }
                    else if (response.StatusCode == 0)
                    {
                        return StatusCode(400, response);
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
        [Route("VerifyMobileOTP")]
        public IActionResult VerifyMobileOTP([FromBody]OTPDetailsEntity otpDetails)
        {
            if (!ModelState.IsValid)
            {
                ResponseEnity response = new ResponseEnity();
                response.StatusCode = 0;
                response.StatusDesc = "Invalid parameter";
                response.Result = BadRequest(ModelState).Value;
                return StatusCode(400, response);
            }
            else
            {
                try
                {
                    OTPAuthentication otpAuth = new OTPAuthentication(configuration);
                    ResponseEnity response = otpAuth.VerifyMobileOTP(otpDetails.ID, otpDetails.OTP);

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
        [Route("SendEmailOTP")]
        public IActionResult SendEmailOTP([FromBody]string email)
        {
            if (email == null || email.Trim() == "")
            {
                ResponseEnity response = new ResponseEnity();
                response.StatusCode = 0;
                response.StatusDesc = "Email id invalid";
                return StatusCode(400, response);
            }
            else
            {
                try
                {
                    OTPAuthentication otpAuth = new OTPAuthentication(configuration);
                    ResponseEnity response = otpAuth.SendEmailOTP(email);

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

        //[AppAuthorize]
        [HttpPost]
        [Route("VerifyEmailOTP")]
        public IActionResult VerifyEmailOTP([FromBody]OTPDetailsEntity otpDetails)
        {
            if (!ModelState.IsValid)
            {
                ResponseEnity response = new ResponseEnity();
                response.StatusCode = 0;
                response.StatusDesc = "Invalid parameter";
                response.Result = BadRequest(ModelState).Value;
                return StatusCode(400, response);
            }
            else
            {
                try
                {
                    OTPAuthentication otpAuth = new OTPAuthentication(configuration);
                    ResponseEnity response = otpAuth.VerifyEmailOTP(otpDetails.ID, otpDetails.OTP);

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
    }
}