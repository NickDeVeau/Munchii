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
        private List<RestaurantType> dealBreakers; // Declare dealBreakers at the class level
        private FirebaseClient firebase;
        public ObservableCollection<Restaurant> Restaurants { get; set; }

        public RankRestaurantsPage(string roomCode, string userId, List<RestaurantType> allRestaurants, List<RestaurantType> dealBreakers)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.roomCode = roomCode;
            this.userId = userId;
            this.dealBreakers = dealBreakers; // Assign dealBreakers here
            this.firebase = new FirebaseClient("https://munchii-8986a-default-rtdb.firebaseio.com");

            // Initialize the Restaurants list with all available restaurants
            Restaurants = new ObservableCollection<Restaurant>(allRestaurants.Select(r => new Restaurant { Name = r.Name, Rating = 0 }));

            // Remove the deal breakers
            foreach (var dealBreaker in dealBreakers)
            {
                Restaurants.Remove(Restaurants.FirstOrDefault(r => r.Name == dealBreaker.Name));
            }

            RestaurantsList.ItemsSource = Restaurants;
        }


        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            // Now you can use dealBreakers here
            var quizData = new
            {
                Rankings = Restaurants,
                DealBreakers = dealBreakers // This will now be in scope
            };

            // Save the quiz data to Firebase
            await SubmitQuizData();

            // Mark the quiz as submitted.
            MarkQuizAsSubmitted();

            // Navigate to the PostQuizWaitingPage.
            await Navigation.PushAsync(new PostQuizWaitingPage(roomCode, userId));
        }

        private async Task SubmitQuizData()
        {
            var quizData = new
            {
                Rankings = Restaurants, // assuming Restaurants contains the ranking data
                DealBreakers = dealBreakers // Add your dealbreakers here
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
