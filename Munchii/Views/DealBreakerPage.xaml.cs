using System;
using Munchii.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;

namespace Munchii
{
    public partial class DealBreakerPage : ContentPage
    {
        private string roomCode;
        private string userId;
        public ObservableCollection<RestaurantType> RestaurantTypes { get; set; }

        public DealBreakerPage(string roomCode, string userId)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.roomCode = roomCode;
            this.userId = userId;

            // Example data - replace with your actual list of restaurant types
            RestaurantTypes = new ObservableCollection<RestaurantType>
            {
                new RestaurantType { Name = "Italian" },
                new RestaurantType { Name = "Chinese" },
                new RestaurantType { Name = "Mexican" },
                new RestaurantType { Name = "American" },
                new RestaurantType { Name = "Japanese" },
                new RestaurantType { Name = "Thai" },
                new RestaurantType { Name = "Fast Food" },
                new RestaurantType { Name = "Buffet" },
                new RestaurantType { Name = "Spanish" },
                new RestaurantType { Name = "French" },
                new RestaurantType { Name = "BBQ" },
                new RestaurantType { Name = "Pizza" },
                new RestaurantType { Name = "Russian" },
                new RestaurantType { Name = "Cafeteria" },
                new RestaurantType { Name = "Cafe" },
                new RestaurantType { Name = "Bar" },
                new RestaurantType { Name = "Winery" },
                new RestaurantType { Name = "Sports Bar" },
                new RestaurantType { Name = "Southern" },
                new RestaurantType { Name = "TexMex" },
                new RestaurantType { Name = "Ice Cream" },
                new RestaurantType { Name = "CheeseCake" },
                new RestaurantType { Name = "Breakfast" }
            };

            RestaurantsList.ItemsSource = RestaurantTypes;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is RestaurantType restaurant)
            {
                restaurant.IsSelected = !restaurant.IsSelected;
                ((ListView)sender).SelectedItem = null; // Deselect the item
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                // If the search text is empty, show all restaurant types
                RestaurantsList.ItemsSource = RestaurantTypes;
            }
            else
            {
                // Filter the restaurant types based on the search text
                var filteredRestaurantTypes = RestaurantTypes.Where(rt => rt.Name.IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                RestaurantsList.ItemsSource = filteredRestaurantTypes;
            }
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            // Getting all the selected deal breakers
            var dealBreakers = RestaurantTypes.Where(rt => rt.IsSelected).ToList();

            // Navigating to RankRestaurantsPage and passing both the complete list and the deal breakers
            await Navigation.PushAsync(new RankRestaurantsPage(roomCode, userId, RestaurantTypes.ToList(), dealBreakers));
        }


    }
}
