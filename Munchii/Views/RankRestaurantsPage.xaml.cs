using System;
using Xamarin.Forms;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using Munchii.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Munchii
{
    public partial class RankRestaurantsPage : ContentPage
    {
        private string roomCode;
        private string userId;
        private List<RestaurantType> dealBreakers;
        private FirebaseClient firebase;
        public ObservableCollection<Restaurant> Restaurants { get; set; }

        public RankRestaurantsPage(string roomCode, string userId, List<RestaurantType> allRestaurants, List<RestaurantType> dealBreakers)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.roomCode = roomCode;
            this.userId = userId;
            this.dealBreakers = dealBreakers;
            this.firebase = new FirebaseClient("https://munchii-8986a-default-rtdb.firebaseio.com");

            Restaurants = new ObservableCollection<Restaurant>(allRestaurants.Select(r => new Restaurant { Name = r.Name, Rating = 0 }));
            foreach (var dealBreaker in dealBreakers)
            {
                Restaurants.Remove(Restaurants.FirstOrDefault(r => r.Name == dealBreaker.Name));
            }
            RestaurantsList.ItemsSource = Restaurants;
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            try
            {
                await SubmitQuizData();
                await MarkQuizAsSubmitted();
                await Navigation.PushAsync(new PostQuizWaitingPage(roomCode, userId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnSubmitClicked: {ex.Message}");
                await DisplayAlert("Error", "There was an error submitting the quiz. Please try again.", "OK");
            }
        }

        private async Task SubmitQuizData()
        {
            var quizData = new
            {
                Rankings = Restaurants,
                DealBreakers = dealBreakers
            };
            await firebase
                .Child("rooms")
                .Child(roomCode)
                .Child("Users")
                .Child(userId)
                .Child("QuizData")
                .PutAsync(quizData);
        }

        private async Task MarkQuizAsSubmitted()
        {
            var postQuizData = new { QuizSubmitted = true };
            await firebase
                .Child("rooms")
                .Child(roomCode)
                .Child("Users")
                .Child(userId)
                .PatchAsync(postQuizData);
        }
    }
}
