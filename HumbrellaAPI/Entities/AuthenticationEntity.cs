﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Entities
{
    public class AuthenticationEntity
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        public string Pwd { get; set; }
    }
}
