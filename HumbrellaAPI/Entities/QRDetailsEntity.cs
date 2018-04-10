using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Entities
{
    public class QRDetailsEntity
    {
        public int StationID { get; set; }

        public string QRCode { get; set; }

        public DateTime ScannedDate { get; set; }
    }
}
