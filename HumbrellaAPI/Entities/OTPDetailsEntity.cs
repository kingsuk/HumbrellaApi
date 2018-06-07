using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Entities
{
    public class OTPDetailsEntity
    {
        [Required]
        public string ID { get; set; }

        [Required]
        public string OTP { get; set; }

        [JsonIgnore]
        public DateTime SentTime { get; set; }
    }
}
