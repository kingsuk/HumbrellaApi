﻿using System;
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
    [Route("api/v1/QRCodeAuthentication")]
    public class QRCodeAuthenticationController : Controller
    {
        IConfiguration configuration;
        public QRCodeAuthenticationController(IConfiguration iconfiguration)
        {
            configuration = iconfiguration;
        }

        //[AppAuthorize]
        [HttpPost]
        [Route("GetQRCode")]
        public IActionResult GetQRCode([FromBody]string stationId)
        {
            Int32 number;
            if (stationId == null || stationId.Trim() == "" || !Int32.TryParse(stationId, out number))
            {
                return new BadRequestObjectResult("Station Id invalid");
            }
            else
            {
                try
                {
                    QRCodeAuthentication qRCode = new QRCodeAuthentication(configuration);
                    ResponseEnity response = qRCode.getQRCode(stationId);

                    if (response.StatusCode == 1)
                    {
                        return StatusCode(200, response.Result);
                    }
                    else if (response.StatusCode == 0)
                    {
                        return StatusCode(400, response.Result);
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