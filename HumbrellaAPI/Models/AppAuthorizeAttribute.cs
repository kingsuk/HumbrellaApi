using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Models
{
    public class AppAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private string AppToken;
        public AppAuthorizeAttribute()
        {
            AppToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.XbPfbIHMI6arZ3Y922BhjWgQzWXcXNrz0ogtVhfEd2o";
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers["AppAuthorize"];
            if(token != AppToken)
            {
                context.Result = new BadRequestObjectResult("App not authorized");
            }
        }
    }
}
