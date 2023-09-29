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

        private void OnFrameTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is RestaurantType restaurant)
            {
                restaurant.IsSelected = !restaurant.IsSelected;
                // Refresh the specific item to update its appearance
                RestaurantsList.SelectedItem = null;
            }
        }
    }

   

}
