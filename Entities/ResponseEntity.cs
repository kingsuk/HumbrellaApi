using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Entities
{
    public class ResponseEntity
    {
        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public Object ResponseResult { get; set; }
    }
}
