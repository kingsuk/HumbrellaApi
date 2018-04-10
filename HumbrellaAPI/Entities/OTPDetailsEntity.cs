using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Entities
{
    public class OTPDetailsEntity
    {
        public string ID { get; set; }

        public string OTP { get; set; }

        [JsonIgnore]
        public DateTime SentTime { get; set; }
    }
}
