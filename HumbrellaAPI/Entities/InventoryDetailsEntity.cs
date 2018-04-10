using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Entities
{
    public class InventoryDetailsEntity
    {
        public int StationID { get; set; }

        public int PartnerID { get; set; }

        public string StationName { get; set; }

        public string PartnerName { get; set; }

        public int StationLevel { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int StationCapacity { get; set; }

        public bool IsOnline { get; set; }
    }
}
