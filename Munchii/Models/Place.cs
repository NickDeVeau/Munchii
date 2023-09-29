using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Munchii.Models
{
    public class Place
    {

        public bool IsTopThree { get; set; } = false;

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vicinity")]
        public string Address { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("opening_hours")]
        public OpeningHoursData OpeningHours { get; set; }



        public string Description
        {
            get
            {
                string openStatus = OpeningHours?.OpenNow == true ? "Open" : "Closed";
                return $"{Address}.\nRating: {Rating}.\nCurrently: {openStatus}.";
            }
        }

        [JsonProperty("price_level")]
        public int PriceLevel { get; set; }

        public class OpeningHoursData  // And rename the nested class here
        {
            [JsonProperty("open_now")]
            public bool OpenNow { get; set; }
        }


        public string PriceSign
        {
            get
            {
                if (PriceLevel >= 1 && PriceLevel <= 3)
                    return new string('$', PriceLevel);
                return "N/A"; // Handle other values as necessary
            }
        }

        [JsonProperty("photos")]
        public List<Photo> Photos { get; set; }

        public string ImageUrl { get; set; } // This property will be set in your FetchNearbyRestaurants method

        // Add other necessary properties based on the Places API result.
    }

    public class Photo
    {
        [JsonProperty("photo_reference")]
        public string PhotoReference { get; set; }

        // Add other properties if necessary.
    }

    public class PlacesApiResponse
    {
        [JsonProperty("results")]
        public List<Place> Results { get; set; }

        // Add other necessary properties based on the Places API result.
    }
}
