using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Munchii.Models;

namespace Munchii
{
    public partial class DealBreakerPage : ContentPage
    {
        // Private fields
        private readonly string roomCode;
        private readonly string userId;

        // Public properties
        public ObservableCollection<RestaurantType> RestaurantTypes { get; private set; }

        // Constructor
        public DealBreakerPage(string roomCode, string userId)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            this.roomCode = roomCode;
            this.userId = userId;

            InitializeRestaurantTypes();
            RestaurantsList.ItemsSource = RestaurantTypes;
        }

        // Initialize restaurant types
        private void InitializeRestaurantTypes()
        {
            // Example data - replace with your actual list of restaurant types
            RestaurantTypes = new ObservableCollection<RestaurantType>
            {
                new RestaurantType { Name = "Italian" },
                new RestaurantType { Name = "Chinese" },
                new RestaurantType { Name = "Mexican" },
                new RestaurantType { Name = "American" },
                new RestaurantType { Name = "Japanese" },
                new RestaurantType { Name = "Thai" },
                new RestaurantType { Name = "Indian" },
                new RestaurantType { Name = "Korean" },
                new RestaurantType { Name = "Mediterranean" },
                new RestaurantType { Name = "Greek" },
                new RestaurantType { Name = "French" },
                new RestaurantType { Name = "Spanish" },
                new RestaurantType { Name = "Middle Eastern" },
                new RestaurantType { Name = "African" },
                new RestaurantType { Name = "Caribbean" },
                new RestaurantType { Name = "Vietnamese" },
                new RestaurantType { Name = "Brazilian" },
                new RestaurantType { Name = "Argentinian" },
                new RestaurantType { Name = "Peruvian" },
                new RestaurantType { Name = "Cuban" },
                new RestaurantType { Name = "Steakhouse" },
                new RestaurantType { Name = "Seafood" },
                new RestaurantType { Name = "Fast Food" },
                new RestaurantType { Name = "Deli" },
                new RestaurantType { Name = "Café" },
                new RestaurantType { Name = "Bakery" },
                new RestaurantType { Name = "Buffet" },
                new RestaurantType { Name = "Tapas" },
                new RestaurantType { Name = "Sushi" },
                new RestaurantType { Name = "Ethiopian" },
                new RestaurantType { Name = "Halal" },
                new RestaurantType { Name = "Kosher" },
                new RestaurantType { Name = "Barbecue" },
                new RestaurantType { Name = "Soul Food" },
                new RestaurantType { Name = "Gastropub" },
                new RestaurantType { Name = "Pizzeria" },
                new RestaurantType { Name = "Dim Sum" },
                new RestaurantType { Name = "Tex-Mex" },
                new RestaurantType { Name = "British" },
                new RestaurantType { Name = "Irish" },
                new RestaurantType { Name = "German" },
                new RestaurantType { Name = "Russian" },
                new RestaurantType { Name = "Nepali" },
                new RestaurantType { Name = "Turkish" },
                new RestaurantType { Name = "Polish" },
                new RestaurantType { Name = "Hungarian" },
                new RestaurantType { Name = "Portuguese" },
                new RestaurantType { Name = "Filipino" },
                new RestaurantType { Name = "Indonesian" },
                new RestaurantType { Name = "Malaysian" },
                new RestaurantType { Name = "Singaporean" }
            };
        }

        // Event Handlers
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
                RestaurantsList.ItemsSource = RestaurantTypes;
            }
            else
            {
                var filteredRestaurantTypes = RestaurantTypes
                    .Where(rt => rt.Name.IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                RestaurantsList.ItemsSource = new ObservableCollection<RestaurantType>(filteredRestaurantTypes);
            }
        }


        private async void OnNextClicked(object sender, EventArgs e)
        {
            var dealBreakers = RestaurantTypes.Where(rt => rt.IsSelected).ToList();
            await Navigation.PushAsync(new RankRestaurantsPage(roomCode, userId, RestaurantTypes.ToList(), dealBreakers));
        }

        private void OnFrameTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is RestaurantType restaurant)
            {
                restaurant.IsSelected = !restaurant.IsSelected;
                RestaurantsList.SelectedItem = null; // Refresh the specific item to update its appearance
            }
        }
    }
}
