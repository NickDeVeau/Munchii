using System;
using Xamarin.Forms;
using Firebase.Database;
using Firebase.Database.Query;

namespace Munchii
{
    public partial class JoinPage : ContentPage
    {
        private string roomCode;
        private string clientUserId; // Use the unique Id as the key, not the username

        public JoinPage()
        {
            InitializeComponent();

            // Get the behaviors
            var behavior1 = Digit1.Behaviors[0] as EntryLineBehavior;
            var behavior2 = Digit2.Behaviors[0] as EntryLineBehavior;
            var behavior3 = Digit3.Behaviors[0] as EntryLineBehavior;
            var behavior4 = Digit4.Behaviors[0] as EntryLineBehavior;

            

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnJoinRoomClicked(object sender, EventArgs e)
        {
            // Get the room code from the input field.
            roomCode = Digit1.Text + Digit2.Text + Digit3.Text + Digit4.Text;

            // Check if the room code is null or empty.
            if (string.IsNullOrEmpty(roomCode))
            {
                // Show an error message and return.
                await DisplayAlert("Error", "Please enter a room code.", "OK");
                return;
            }

            // Try to find the room with the entered code in Firebase.
            var room = await App.Database
                .Child("rooms")
                .Child(roomCode)
                .OnceSingleAsync<Room>();

            if (room != null)
            {
                // The room exists, so join the room.

                // For simplicity, let's assume that a user just has a username.
                clientUserId = Guid.NewGuid().ToString();  // Generate a new unique Id for the client
                var username = "User_" + clientUserId; // Generate a unique username using clientUserId.

                var clientUser = new User
                {
                    Id = clientUserId,
                    Username = username,  // Set the username 
                    QuizSubmitted = false
                };

                // Add the user to the room in Firebase.
                try
                {
                    await App.Database
                        .Child("rooms")
                        .Child(roomCode)
                        .Child("Users")
                        .Child(clientUserId)
                        .PutAsync(clientUser);

                    // Update keepAlive timestamp for the room.
                    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    await App.Database
                        .Child("rooms")
                        .Child(roomCode)
                        .Child("KeepAlive")
                        .PutAsync(timestamp);
                }
                catch (Exception ex)
                {
                    // Log the error or alert the user.
                    System.Diagnostics.Debug.WriteLine($"Error adding user: {ex.Message}");
                }


                // Navigate to the client lobby page.
                await Navigation.PushAsync(new ClientLobbyPage(roomCode, clientUserId));
            }
            else
            {
                // The room does not exist, so display an error message.
                await DisplayAlert("Error", "Invalid room code.", "OK");
            }
        }


        private async void OnBackClicked(object sender, EventArgs e)
        {
            RemoveUserFromRoom();
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            RemoveUserFromRoom();
            return base.OnBackButtonPressed();
        }

        private async void RemoveUserFromRoom()
        {
            try
            {
                // Remove the user from Firebase.
                await App.Database
                    .Child("rooms")
                    .Child(roomCode)
                    .Child("Users")
                    .Child(clientUserId)
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                // Log or handle the exception here.
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private async void OnLeaveRoomClicked(object sender, EventArgs e)
        {
            // Remove the user from the room.
            RemoveUserFromRoom();

            // Navigate back to the main page.
            await Navigation.PopAsync();
        }
    }
}
