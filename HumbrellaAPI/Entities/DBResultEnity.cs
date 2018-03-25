using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Entities
{
    public class DBResultEnity
    {
        public int StatusCode { get; set; }

        public string StatusDesc { get; set; }

        public object Result { get; set; }
    }
}
