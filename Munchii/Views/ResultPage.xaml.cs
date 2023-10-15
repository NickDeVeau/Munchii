using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System.Net.Http;
using Munchii.Models;

namespace Munchii
{
    public partial class ResultPage : ContentPage, INotifyPropertyChanged
    {
        // Fields
        private readonly string roomCode;
        private readonly FirebaseClient firebase;
        private int miles = 1;
        private string _winningRestaurantType;

        // Properties
        public string WinningRestaurantType
        {
            get => _winningRestaurantType;
            set
            {
                if (_winningRestaurantType != value)
                {
                    _winningRestaurantType = value;
                    OnPropertyChanged(nameof(WinningRestaurantType));
                }
            }
        }

        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor
        public ResultPage(string roomCode)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            this.roomCode = roomCode;
            this.firebase = new FirebaseClient("https://munchii-8986a-default-rtdb.firebaseio.com");
            LoadResults();
        }

        // Methods
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void LoadResults()
        {
            // Fetch user data from Firebase
            var usersSnapshot = await firebase
                .Child("rooms")
                .Child(roomCode)
                .Child("Users")
                .OnceAsync<User>();

            // Initialize restaurant rankings and deal breakers
            var restaurantRankings = new Dictionary<string, double>();
            var dealBreakers = new HashSet<string>();

            // Populate deal breakers
            foreach (var userSnapshot in usersSnapshot)
            {
                var userDealBreakers = userSnapshot.Object.QuizData?.DealBreakers;
                if (userDealBreakers == null) continue;

                foreach (var dealBreaker in userDealBreakers)
                {
                    if (dealBreaker.IsSelected)
                    {
                        dealBreakers.Add(dealBreaker.Name);
                    }
                }
            }

            // Populate restaurant rankings
            foreach (var userSnapshot in usersSnapshot)
            {
                var rankings = userSnapshot.Object.QuizData?.Rankings;
                if (rankings == null) continue;

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

            // Determine the winning restaurant type
            var winningRestaurant = restaurantRankings.OrderByDescending(r => r.Value).FirstOrDefault();
            if (winningRestaurant.Key == null) return;

            WinningRestaurantType = winningRestaurant.Key;

            // Fetch and display nearby restaurants
            float milesToMeters = miles * 1609;
            string radius = milesToMeters.ToString();
            var allNearbyRestaurants = await FetchNearbyRestaurants(winningRestaurant.Key, radius);
            var topThree = FetchTopThreeFromList(allNearbyRestaurants);
            RestaurantList.ItemsSource = topThree.Concat(allNearbyRestaurants.Except(topThree));
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

        // Event Handlers
        private void OnRadiusButtonClicked(object sender, EventArgs e)
        {
            miles = miles * 2;
            if (miles > 25)
            {
                miles = 1;
            }
            RadiusButton.Text = $"Matches within {miles} miles";
            LoadResults();
        }

        private async void OnRestaurantItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Place place)
            {
                // Define the destination name (e.g., the restaurant's name)
                var destinationName = place.Name;

                // Check which platform we're on
                var isiOS = Device.RuntimePlatform == Device.iOS;

                string url;

                if (isiOS)
                {
                    // Open in Apple Maps with the name of the restaurant for more details using the maps:// scheme
                    url = $"maps://?q={Uri.EscapeUriString(destinationName)}";
                }
                else
                {
                    // If needed, you can have a fallback for non-iOS devices.
                    // The following example uses Google Maps:
                    url = $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeUriString(destinationName)}";
                }

                await Launcher.OpenAsync(new Uri(url));
            }

            // Deselect the tapped item
            RestaurantList.SelectedItem = null;
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
