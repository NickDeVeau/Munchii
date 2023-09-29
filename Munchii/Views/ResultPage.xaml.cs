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
using System.ComponentModel;

namespace Munchii
{
    public partial class ResultPage : ContentPage, INotifyPropertyChanged
    {

        private readonly string roomCode;
        private readonly FirebaseClient firebase;
        int miles = 1;


        private string _winningRestaurantType;
        public string WinningRestaurantType
        {
            get { return _winningRestaurantType; }
            set
            {
                if (_winningRestaurantType != value)
                {

                    _winningRestaurantType = value;
                    OnPropertyChanged(nameof(WinningRestaurantType));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ResultPage(string roomCode)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            this.roomCode = roomCode;

            this.firebase = new FirebaseClient("https://munchii-8986a-default-rtdb.firebaseio.com");

            LoadResults();
        }

        private List<Place> FetchTopThreeFromList(List<Place> allRestaurants)
        {
            if (allRestaurants == null || allRestaurants.Count == 0)
                return new List<Place>();

            var topThree = new List<Place>();
            var cheapRestaurant = allRestaurants.FirstOrDefault(r => r.PriceLevel == 1);
            var mediumRestaurant = allRestaurants.FirstOrDefault(r => r.PriceLevel == 2);
            var expensiveRestaurant = allRestaurants.FirstOrDefault(r => r.PriceLevel == 3);

            if (cheapRestaurant != null)
            {
                cheapRestaurant.IsTopThree = true;
                topThree.Add(cheapRestaurant);
            }
            if (mediumRestaurant != null)
            {
                mediumRestaurant.IsTopThree = true;
                topThree.Add(mediumRestaurant);
            }
            if (expensiveRestaurant != null)
            {
                expensiveRestaurant.IsTopThree = true;
                topThree.Add(expensiveRestaurant);
            }

            return topThree;
        }


        private async Task<List<Place>> FetchTopThreeRestaurants(string restaurantType, string radius)
        {
            var allRestaurants = await FetchNearbyRestaurants(restaurantType, radius);
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

        private async Task<List<Place>> FetchNearbyRestaurants(string restaurantType, string radius)
        {
            string apiKey = "AIzaSyC7PV_5w9rS6LX3wNetqR-CSrmxrR1zxg8";

            var locationObj = await GetCurrentLocationAsync();
            if (locationObj == null)
            {
                Console.WriteLine("Couldn't fetch the current location.");
                return new List<Place>();
            }

            string location = $"{locationObj.Latitude},{locationObj.Longitude}";
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
            BindingContext = this;
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
                WinningRestaurantType = winningRestaurant.Key;  // Update the property

                float milesToMeters = miles * 1609;
                string radius = milesToMeters.ToString();

                var allNearbyRestaurants = await FetchNearbyRestaurants(winningRestaurant.Key, radius);
                var topThree = FetchTopThreeFromList(allNearbyRestaurants);  // Extracting top three

                foreach (var restaurant in allNearbyRestaurants)
                {
                    restaurant.Name = restaurant.Name.ToUpper();
                }

                RestaurantList.ItemsSource = topThree.Concat(allNearbyRestaurants.Except(topThree));  // Display top three followed by the rest
            }
        }

        private void OnRadiusButtonClicked(object sender, EventArgs e)
        {
            miles = miles*2;
            if (miles > 25)
            {
                miles = 1;
            }
            RadiusButton.Text = $"Matches within {miles} miles";
            LoadResults();
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

        private async void OnDescriptionTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Place place)
            {
                // Define the destination
                var destination = place.Address;

                // Check which platform we're on
                var isiOS = Device.RuntimePlatform == Device.iOS;

                // iOS doesn't like spaces in its URLs
                destination = isiOS ? Uri.EscapeUriString(destination) : destination;

                string url;

                if (isiOS)
                {
                    // Open in Apple Maps
                    url = $"http://maps.apple.com/maps?q={destination}";
                }
                else
                {
                    // Open in Google Maps
                    url = $"https://www.google.com/maps/dir/?api=1&destination={destination}";
                }

                await Launcher.OpenAsync(new Uri(url));
            }
        }


        private async void OnHomeClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
