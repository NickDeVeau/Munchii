using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Munchii.Models
{
    public class Place
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        [JsonProperty("price_level")]
        public int PriceLevel { get; set; }
        public string PriceSign
        {
            get
            {
                if (PriceLevel >= 1 && PriceLevel <= 3)
                    return new string('$', PriceLevel);
                return string.Empty; // Handle other values as necessary
            }
        }

        // Add other necessary properties based on the Places API result.
    }


    public class PlacesApiResponse
    {
        public List<Place> Results { get; set; }
        // Add other necessary properties based on the Places API result.
    }
}

