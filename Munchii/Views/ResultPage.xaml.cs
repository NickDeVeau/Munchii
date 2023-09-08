using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Firebase.Database;
using Munchii.Models;
using Firebase.Database.Query;
using Xamarin.Forms.Maps;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Xamarin.Essentials;

namespace Munchii
{
    public partial class ResultPage : ContentPage
    {
        private readonly string roomCode;
        private readonly FirebaseClient firebase;

        public ResultPage(string roomCode)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.roomCode = roomCode;

            this.firebase = new FirebaseClient("https://munchii-8986a-default-rtdb.firebaseio.com");

            LoadResults();
        }

        private async Task<List<Place>> FetchTopThreeRestaurants(string restaurantType)
        {
            var allRestaurants = await FetchNearbyRestaurants(restaurantType);
            if (allRestaurants == null || allRestaurants.Count == 0)
                return new List<Place>();

            var topThree = new List<Place>();
            var cheapRestaurant = allRestaurants.FirstOrDefault(r => r.PriceLevel == 1);
            var mediumRestaurant = allRestaurants.FirstOrDefault(r => r.PriceLevel == 2);
            var expensiveRestaurant = allRestaurants.FirstOrDefault(r => r.PriceLevel == 3);

            if (cheapRestaurant != null)
                topThree.Add(cheapRestaurant);
            if (mediumRestaurant != null)
                topThree.Add(mediumRestaurant);
            if (expensiveRestaurant != null)
                topThree.Add(expensiveRestaurant);

            return topThree;
        }

        private async Task<List<Place>> FetchNearbyRestaurants(string restaurantType)
        {
            string apiKey = "AIzaSyC7PV_5w9rS6LX3wNetqR-CSrmxrR1zxg8";

            var locationObj = await GetCurrentLocationAsync();
            if (locationObj == null)
            {
                Console.WriteLine("Couldn't fetch the current location.");
                return new List<Place>();  // Return an empty list or handle this scenario appropriately
            }

            string location = $"{locationObj.Latitude},{locationObj.Longitude}";
            string radius = "2000"; // 2 km for example
            string type = "restaurant";
            string keyword = restaurantType;

            string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={location}&radius={radius}&type={type}&keyword={keyword}&key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var placesResponse = JsonConvert.DeserializeObject<PlacesApiResponse>(response);
                return placesResponse.Results;
            }
        }


        private async void LoadResults()
        {
            var usersSnapshot = await firebase
                .Child("rooms")
                .Child(roomCode)
                .Child("Users")
                .OnceAsync<User>();

            Dictionary<string, double> restaurantRankings = new Dictionary<string, double>();
            HashSet<string> dealBreakers = new HashSet<string>();

            foreach (var userSnapshot in usersSnapshot)
            {
                var rankings = userSnapshot.Object.QuizData?.Rankings;
                var userDealBreakers = userSnapshot.Object.QuizData?.DealBreakers;

                if (userDealBreakers != null)
                {
                    foreach (var dealBreaker in userDealBreakers)
                    {
                        if (dealBreaker.IsSelected)
                        {
                            dealBreakers.Add(dealBreaker.Name);
                        }
                    }
                }
            }


            foreach (var userSnapshot in usersSnapshot)
            {
                var rankings = userSnapshot.Object.QuizData?.Rankings;
                if (rankings != null)
                {
                    foreach (var ranking in rankings)
                    {
                        if (!dealBreakers.Contains(ranking.Name))
                        {
                            if (restaurantRankings.ContainsKey(ranking.Name))
                            {
                                restaurantRankings[ranking.Name] += ranking.Rating;
                            }
                            else
                            {
                                restaurantRankings[ranking.Name] = ranking.Rating;
                            }
                        }
                    }
                }
            }

            var winningRestaurant = restaurantRankings.OrderByDescending(r => r.Value).FirstOrDefault();

            if (winningRestaurant.Key != null)
            {
               

                var nearbyRestaurants = await FetchTopThreeRestaurants(winningRestaurant.Key);

                foreach (var restaurant in nearbyRestaurants)
                {
                    restaurant.Name = restaurant.Name.ToUpper();
                }

                // Updated line: Removing unnecessary 'Take(3)'
                RestaurantList.ItemsSource = nearbyRestaurants;
            }
           
        
        }

        int miles = 5;

        private void OnRadiusButtonClicked(object sender, EventArgs e)
        {
            miles += 5;
            if (miles > 15)
            {
                miles = 5;
            }
            RadiusButton.Text = $"Matches within {miles} miles";
        }

        public async Task<Location> GetCurrentLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    return location;
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                location = await Geolocation.GetLocationAsync(request);

                return location;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to get location: {ex.Message}");
                return null;
            }
        }



        private void AddPinToMap(string label, string address, Position position)
        {
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = label,
                Address = address
            };
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
