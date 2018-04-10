using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumbrellaAPI.Models
{
    public class Helper
    {
        public static string hashValue(string pwd)
        {
            byte[] salt = Encoding.ASCII.GetBytes("NZsP6NnmfBuYeJrrAKNuVQ==");
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: pwd,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}
