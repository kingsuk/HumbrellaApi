using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Entities
{
    public class RegistrationEntity
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string EmailId { get; set; }

        [Required]
        public string Pwd { get; set; }
    }
}
